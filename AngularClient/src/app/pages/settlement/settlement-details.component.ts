import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { BalancesService, DecompressFileResult, DocumentsService, FileService, Settlement, SettlementService, Transaction, TransactionsService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField } from "app/shared/details-view/details-view.component";
import { ToolbarElement, ToolbarElementAction, ToolbarElementWithData } from "app/shared/models/toolbar";
import { forkJoin, Subscription } from "rxjs";
import { take } from "rxjs/operators";
import { Location } from '@angular/common';
import { FormGroup, FormsModule } from "@angular/forms";
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
        
        <grid name="sublist" [columns]="columns" [data]="subList"
            initialSortColumn="date" initialSortOrder="1"
            (rowClicked)="rowClickedEvent($event)"></grid>

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
    public columns: GridColumn[];
    public subList: Transaction[];

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
                    this.subList = [...transactions, ...documents];
                    this.columns = [ 
                        { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true, customEvent: true},
                        { title: 'Konto', dataProperty: 'account', component: 'list', filterOptions: { idProperty: 'account' } as ListFilterOptions, customEvent: true},
                        { title: 'Kategoria', dataProperty: 'category', component: 'list', filterOptions: { idProperty: 'category', usageIndexPeriodDays: 40, usageIndexThreshold: 5, usageIndexPeriodDateProperty: 'date' } as ListFilterOptions, customEvent: true},
                        { title: 'Numer', dataProperty: 'invoiceNumber', component: 'text', customEvent: true},
                        { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amountWithEmpty', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions, customEvent: true},
                        { title: 'Opis', dataProperty: 'description', subDataProperty1: 'comment', component: 'text', filterOptions: { additionalPropertyToSearch1: 'comment' } as TextFilterOptions, customEvent: true},
                        { title: '', dataProperty: 'showImage', component: 'icon', customEvent: true, image: 'nc-image', conditionalFormatting: 'bool'}
                    ];

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

    rowClickedEvent(rowClickedData: RowClickedData) {
        if (rowClickedData.row['rowClick']==='document' && rowClickedData.column.dataProperty!=='showImage') {
            this.router.navigate(['documents', rowClickedData.row['id']]);
        } else if (rowClickedData.row['rowClick']==='document' && rowClickedData.column.dataProperty==='showImage') {
            if (!this.settingService.CurrentPassword)
            {
                this.modalService.open(this.password, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
                    this.settingService.CurrentPassword = result;
                    this.openFile(rowClickedData);
                 }, (reason) => { });
            } else {
                this.openFile(rowClickedData);
            }
        } else {
            this.router.navigate(['transactions', rowClickedData.row['id']]);
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
