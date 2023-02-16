import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { BalancesService, DecompressFileResult, Document, DocumentsService, FileService, Settlement, SettlementService, Transaction, TransactionsService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField } from "app/shared/details-view/details-view.component";
import { ToolbarElement, ToolbarElementAction, ToolbarElementWithData } from "app/shared/models/toolbar";
import { forkJoin, Subscription } from "rxjs";
import { take } from "rxjs/operators";
import { Location } from '@angular/common';
import { GridColumn, RowClickedData } from "app/shared/grid/grid.component";
import { ListFilterOptions } from "app/shared/list-filter/list-filter.component";
import { TextFilterOptions } from "app/shared/text-filter/text-filter.component";
import { AmountFilterOptions } from "app/shared/amount-filter/amount-filter.component";
import { SettingsService } from "app/api/settingsService";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
    moduleId: module.id,
    template: `
        <details-view 
        [viewDefinition]="viewDefinition" 
        [data]="data" 
        [toolbarElements]="toolbarElements" 
        (toolbarElementClick)="toolbarElementClick($event)"
        (valueChange)="valueChange($event)"
        ></details-view>
        
        <grid name="invoices" [columns]="invoiceColumns" [data]="invoiceData"
            initialSortColumn="sortOrder" initialSortOrder="1"
            (rowClicked)="invoicesClickedEvent($event)">
        </grid>
        <grid name="money" [columns]="moneyColumns" [data]="moneyData"
            initialSortColumn="sortOrder" initialSortOrder="1"
            (rowClicked)="moneyClickedEvent($event)">
        </grid>
        <grid name="other" [columns]="otherColumns" [data]="otherData"
            initialSortColumn="date" initialSortOrder="1"
            (rowClicked)="otherClickedEvent($event)">
        </grid>
        <ng-template #password let-modal>
            <div class="modal-header">
                <h4 class="modal-title" id="modal-basic-title">Podaj hasło</h4>
            </div>
            <div class="modal-body">
                <input 
                    #password
                    id="password" 
                    type="password" 
                    class="form-control">
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-light" (click)="modal.close('')">Anuluj</button>
                <button type="button" class="btn btn-outline-danger" (click)="modal.close(password.value)">Otwórz</button>
            </div>
        </ng-template>
    `
})

export class SettlementDetailsComponent implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: Settlement;
    toolbarElements: ToolbarElement[] = [];
    private routeSubscription: Subscription;
    @ViewChild('password', { static: true }) password: ElementRef;   
   
    public invoiceColumns: GridColumn[];
    public invoiceData: any[];
    
    public moneyColumns: GridColumn[];
    public moneyData: any[];

    public otherColumns: GridColumn[];
    public otherData: any[];

    constructor(private settlementService: SettlementService, 
        private balancesService: BalancesService,
        private fileService: FileService,
        private transactionsService: TransactionsService,
        private documentsService: DocumentsService,
        private route: ActivatedRoute, 
        private router: Router, 
        private settingService: SettingsService,
        private modalService: NgbModal,
        private location: Location) {  
    }

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe((params: Params) => {
            forkJoin([
                this.settlementService.settlementGet(), this.balancesService.balancesGet(), this.transactionsService.transactionsGet(), this.documentsService.documentsGet()])
                .pipe(take(1)).subscribe(([settlementList, balancesList, transactionList, documentsList]) => {
                    this.data = settlementList.filter(m => m.id == params['id'])[0];
                    
                    let transactions = transactionList.filter(m => m.settlement === this.data['title']).map(t => ({...t, rowClick:'transaction', description: t.bankInfo}));
                    let documents = documentsList.filter(m => m.settlement === this.data['title']).map(t => ({...t, rowClick:'document', showImage: 1}));

                    this.prepareInvoices(transactions, documents);
                    this.prepareMoney(transactions, documents);
                    this.prepareOther(transactions, documents);

                    if (!this.data.closed)
                    {
                        let eurSold = transactionList.filter(m => m.settlement === this.data['title'] && m.bankInfo=='OBCIĄŻ. NATYCH. TRANSAKCJA WALUT.').map(t => t.amount).reduce((sum, current) => sum + current, 0);
                        let plnBought = transactionList.filter(m => m.settlement === this.data['title'] && m.bankInfo=='UZNANIE NATYCH. TRANSAKCJA WALUT.').map(t => t.amount).reduce((sum, current) => sum + current, 0);
                        this.data['eurSold'] = eurSold;
                        this.data['incomeGrossPln'] = plnBought;
                        this.valueChange(this.data);
                        // this.data['remainingEur'] = this.data.incomeGrossEur + eurSold;
                        // this.data['exchangeRatio'] = plnBought / eurSold * (-1);
                    }
                    

                    this.viewDefinition = {
                        fields: [
                            { title: 'Okres', dataProperty: 'title', component: 'text', required: true, readonly: true} as DetailsViewField,
                            { title: 'Przychód w EUR', dataProperty: 'incomeGrossEur', component: 'amount', readonly: this.data.closed} as DetailsViewField,
                            { title: 'Przychód w PLN', dataProperty: 'incomeGrossPln', component: 'amount', readonly: true} as DetailsViewField,
                            { title: 'Kurs wymiany', dataProperty: 'exchangeRatio', component: 'number', readonly: true} as DetailsViewField,
                            { title: 'Pozostało EUR', dataProperty: 'remainingEur', component: 'amount', readonly: true} as DetailsViewField,
                        //    { title: 'Stan konta PLN', dataProperty: 'balanceAccountPln', component: 'amount', readonly: true} as DetailsViewField,
                            { title: 'Pit', dataProperty: 'pit', component: 'amount', readonly: this.data.closed} as DetailsViewField,
                            { title: 'Vat', dataProperty: 'vat', component: 'amount', readonly: this.data.closed} as DetailsViewField,
                            { title: 'Zus', dataProperty: 'zus', component: 'amount', readonly: this.data.closed} as DetailsViewField,
                         //   { title: 'Rezerwa', dataProperty: 'reserve', component: 'amount', readonly: this.data.closed} as DetailsViewField,
                         //   { title: 'Niedopłata/nadpłata na koncie PLN', dataProperty: 'total', component: 'amount', readonly: true} as DetailsViewField,
                            { title: 'Zysk', dataProperty: 'revenue', component: 'amount', readonly: true} as DetailsViewField,
                            { title: 'Komentarz', dataProperty: 'comment', component: 'text'} as DetailsViewField
                        ]
                    };

                    if (this.data.closed) {
                        var i = this.viewDefinition.fields.length;
                        while (i--) {
                            if (this.viewDefinition.fields[i].dataProperty === 'balanceAccountPln'
                                || this.viewDefinition.fields[i].dataProperty === 'balanceAccountEur'
                                || this.viewDefinition.fields[i].dataProperty === 'total') {
                                this.viewDefinition.fields.splice(i, 1);
                            }
                        }
                        this.toolbarElements.push(
                            { name: 'save', title: 'Zapisz', defaultAction: ToolbarElementAction.SaveChanges},
                            
                        );
                    } else {
                        const balancesPLN = balancesList.filter(m => m.account === 'KontoFirmowe').sort((a,b) => a['date'] > b['date'] ? -1 : 1);
                        if (balancesPLN.length > 0) {
                            this.data['balanceAccountPln'] = balancesPLN[0].amount;
                        }
                        
                        this.toolbarElements.push(
                            { name: 'save', title: 'Zapisz', defaultAction: ToolbarElementAction.SaveChanges},
                            { name: 'delete', title: 'Usuń', defaultAction: ToolbarElementAction.Delete},
                            { name: 'close', title: 'Zamknij', align: 'right'}
                        );
                    }
                });
        });
    }

    ngOnDestroy(){
        this.routeSubscription.unsubscribe();
    }

    toolbarElementClick(toolbarElementWithData: ToolbarElementWithData) {
        if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.SaveChanges) {
            this.settlementService.settlementSettlementPut(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        } else if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.Delete) {
            this.settlementService.settlementSettlementIdDelete(toolbarElementWithData.data.id).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        } else if (toolbarElementWithData.toolbarElement.name === 'close') {
            toolbarElementWithData.data.closed = true;
            this.settlementService.settlementSettlementPut(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
    }

    valueChange(data: Settlement) {
        data.remainingEur = data.incomeGrossEur + data['eurSold'];
        data.revenue = data.incomeGrossPln - data.pit - data.vat - data.zus; 
        //data.total = data.balanceAccountPln - data.pit - data.vat - data.zus;
        //data.exchangeRatio = data.incomeGrossEur == 0 ? 0 : data.incomeGrossPln / data.incomeGrossEur;
        data.exchangeRatio = !data['eurSold'] || data['eurSold'] == 0 ? undefined : data.incomeGrossPln / data['eurSold'] * (-1)
    }

    prepareInvoices(transactions: Transaction[], documents: Document[]) {
        this.invoiceColumns = [ 
            { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true, customEvent: true},
            { title: 'Kategoria', dataProperty: 'category', component: 'list', filterOptions: { idProperty: 'category', usageIndexPeriodDays: 40, usageIndexThreshold: 5, usageIndexPeriodDateProperty: 'date' } as ListFilterOptions, customEvent: true},
            { title: 'Numer', dataProperty: 'invoiceNumber', component: 'text', customEvent: true},
            { title: 'Opis', dataProperty: 'description', subDataProperty1: 'comment', component: 'text', filterOptions: { additionalPropertyToSearch1: 'comment' } as TextFilterOptions, customEvent: true},
            { title: 'Kwota dokumentu', dataProperty: 'formattedAmounts', alignment: 'right', noWrap:true, component: 'text'},
            { title: 'Kwota zapłacona', dataProperty: 'transactionAmount', additionalDataProperty1: 'transactionCurrency',  pipe: 'amountWithEmpty', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'transactionCurrency'} as AmountFilterOptions, customEvent: true},
            { title: '', dataProperty: 'showImage', component: 'icon', customEvent: true, image: 'nc-image', conditionalFormatting: 'bool'}
        ];
        this.invoiceData = documents
            .filter(d => d.category === "Faktura otrzymana" || d.category === "Faktura wystawiona")
            .map(d => ({
                ...d,
                transaction: transactions.filter(t => t.id === d.transactionId)[0]
            }))
            .map(d => ({
                ...d,
                transactionAmount: d.transaction?.amount,
                transactionCurrency: d.transaction?.currency,
                sortOrder: d.category + d.date,
                formattedAmounts: this.formatAmounts(d)
            }));
    }

    formatAmounts(document: Document) : string {
        if (!document.net && !document.vat && !document.gross)
            return "";
        return this.formatAmount(document.net) + ' -> ' + this.formatAmount(document.vat) + ' -> '  + this.formatAmount(document.gross) + ' ' + document.currency;
    }

    formatAmount(amount: Number) : string {
        return amount.toFixed(2);
    }

    invoicesClickedEvent(rowClickedData: RowClickedData) {
        if (rowClickedData.column.dataProperty==='showImage') {
            this.openFileWithPassword(rowClickedData);
        } else if (rowClickedData.column.dataProperty==='transactionAmount') {
            if (rowClickedData.row['transactionId']) {
                this.router.navigate(['transactions', rowClickedData.row['transactionId']]);
            }
        } else
            this.router.navigate(['documents', rowClickedData.row['id']]);
    }

    prepareMoney(transactions: Transaction[], documents: Document[]) {
        this.moneyColumns = [ 
            { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true, customEvent: true},
            { title: 'Konto z', dataProperty: 'accountFrom', component: 'list', filterOptions: { idProperty: 'account' } as ListFilterOptions, customEvent: true},
            { title: 'Konto do', dataProperty: 'accountTo', component: 'list', filterOptions: { idProperty: 'account' } as ListFilterOptions, customEvent: true},
            { title: 'EUR', dataProperty: 'amountEUR', additionalDataProperty1: 'currencyEUR',  pipe: 'amountWithEmpty', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', skipConditionalFormattingProperty: 'skipFormatting', component: 'amount', filterOptions: { currencyDataProperty: 'currencyEUR'} as AmountFilterOptions, customEvent: true},
            { title: 'PLN', dataProperty: 'amountPLN', additionalDataProperty1: 'currencyPLN',  pipe: 'amountWithEmpty', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'currencyPLN'} as AmountFilterOptions, customEvent: true},
            { title: 'Kurs', dataProperty: 'exchangeRate', alignment: 'right', noWrap:true, component: 'text', customEvent: true},
        ];

        const salaryInvoice = documents.filter(d => d.category === "Faktura wystawiona")[0];
        const salary = transactions.filter(t => t.id === salaryInvoice.transactionId).map(t => (
            {
                ...t,
                accountFrom: t.account,
                currencyEUR: 'EUR',
                amountEUR: t.amount,
                sortOrder: 10
            }));
        const transfers = transactions
            .filter(t => t.category === "Transfer" 
                && !t.description.endsWith(' NATYCH. TRANSAKCJA WALUT.')
                && t.amount < 0)
            .map(t => (
                {
                    ...t,
                    accountFrom: t.account,
                    accountTo: transactions.filter(t2 => t2.date === t.date && t2.amount === t.amount * -1)[0]?.account,
                    currencyEUR: 'EUR',
                    amountEUR: t.amount * -1,
                    sortOrder: t.date,
                    skipFormatting: true
                }));
            const convertions = transactions
                .filter(t => t.category === "Transfer" 
                    && t.description == 'OBCIĄŻ. NATYCH. TRANSAKCJA WALUT.')
                .map(t => ({
                    ...t,
                    matching: transactions.filter(t2 => 
                        t2.date === t.date 
                        && t2.description === 'UZNANIE NATYCH. TRANSAKCJA WALUT.'
                        && t2.comment === t.comment)[0]
                }))
                .map(t => (
                    {
                        ...t,
                        accountFrom: t.account,
                        accountTo: t.matching?.account,
                        currencyEUR: 'EUR',
                        amountEUR: t.amount,
                        currencyPLN: 'PLN',
                        amountPLN: t.matching?.amount,
                        sortOrder: t.date,
                        exchangeRate: Number((t.matching?.amount / t.amount).toFixed(4)) * -1
                    }));       
        this.moneyData = [...salary, ...transfers, ...convertions];

    }

    moneyClickedEvent(rowClickedData: RowClickedData) {
        if (rowClickedData.column.dataProperty==='accountTo' 
        || rowClickedData.column.dataProperty==='amountPLN' 
        || rowClickedData.column.dataProperty==='exchangeRate') {
            this.router.navigate(['transactions', rowClickedData.row['matching']['id']]);
        } else {
            this.router.navigate(['transactions', rowClickedData.row['id']]);
        }
    }

    prepareOther(transactions: Transaction[], documents: Document[]) {
        this.otherColumns = [ 
            { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true, customEvent: true},
            { title: 'Kategoria', dataProperty: 'category', component: 'list', filterOptions: { idProperty: 'category', usageIndexPeriodDays: 40, usageIndexThreshold: 5, usageIndexPeriodDateProperty: 'date' } as ListFilterOptions, customEvent: true},
            { title: 'Numer', dataProperty: 'invoiceNumber', component: 'text', customEvent: true},
            { title: 'Opis', dataProperty: 'description', subDataProperty1: 'comment', component: 'text', filterOptions: { additionalPropertyToSearch1: 'comment' } as TextFilterOptions, customEvent: true},
            { title: '', dataProperty: 'showImage', component: 'icon', customEvent: true, image: 'nc-image', conditionalFormatting: 'bool'}
        ];
        this.otherData = documents.filter(d => d.category !== "Faktura otrzymana" && d.category !== "Faktura wystawiona");
    }
   
    otherClickedEvent(rowClickedData: RowClickedData) {
        if (rowClickedData.column.dataProperty==='showImage') {
            this.openFileWithPassword(rowClickedData);
        } else {
            this.router.navigate(['documents', rowClickedData.row['id']]);
        }
    }

    rowClickedEvent(rowClickedData: RowClickedData) {
        if (rowClickedData.row['rowClick']==='document' && rowClickedData.column.dataProperty!=='showImage') {
            this.router.navigate(['documents', rowClickedData.row['id']]);
        } else if (rowClickedData.row['rowClick']==='document' && rowClickedData.column.dataProperty==='showImage') {
            this.openFileWithPassword(rowClickedData);
        } else {
            this.router.navigate(['transactions', rowClickedData.row['id']]);
        } 
    }

    openFileWithPassword(rowClickedData: RowClickedData){
        if (!this.settingService.CurrentPassword)
        {
            this.modalService.open(this.password, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
                this.settingService.CurrentPassword = result;
                this.openFile(rowClickedData);
             }, (reason) => { });
        } else {
            this.openFile(rowClickedData);
        }
    }

    openFile(rowClickedData: RowClickedData) {
        if (!this.settingService.CurrentPassword){
            return;
        }
        
        let number = rowClickedData.row['number'];
        this.fileService.filePost({ number: number, password: this.settingService.CurrentPassword}).pipe(take(1)).subscribe((result: DecompressFileResult) => {
            window.open("http://127.0.0.1:8080" + result.path, "_blank", "noopener noreferrer");
        });
    }
}
