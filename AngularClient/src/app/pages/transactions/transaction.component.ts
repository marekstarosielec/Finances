import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { CaseListService, CurrenciesService, DecompressFileResult, DocumentsService, FileService, SettlementService, Transaction, TransactionsService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField, DetailsViewFieldAmountOptions, DetailsViewFieldListOptions } from "app/shared/details-view/details-view.component";
import { ToolbarElement, ToolbarElementAction, ToolbarElementWithData } from "app/shared/models/toolbar";
import { forkJoin, Subscription } from "rxjs";
import { Location } from '@angular/common';
import { take } from "rxjs/operators";
import { Document } from "app/api/generated";
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
            ></details-view>
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

export class TransactionComponent implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: any;
    private routeSubscription: Subscription;
    @ViewChild('password', { static: true }) password: ElementRef;
    toolbarElements: ToolbarElement[] = [];
    
    constructor(private transactionsService: TransactionsService, 
        private currenciesService: CurrenciesService, 
        private route: ActivatedRoute, 
        private caseListService: CaseListService,
        private settlementService: SettlementService,
        private documentsService: DocumentsService,
        private location: Location,
        private settingService: SettingsService,
        private modalService: NgbModal,
        private fileService: FileService,
        private router: Router) {  
    }

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe((params: Params) => {
            forkJoin([
                this.transactionsService.transactionsGet(),
                this.transactionsService.transactionsAccountsGet(),
                this.transactionsService.transactionsCategoriesGet(),
                this.currenciesService.currenciesGet(), 
                this.caseListService.caseListGet(),
                this.settlementService.settlementGet(),
                ])
                .pipe(take(1)).subscribe(([transactions, accounts, categories, currencies, caseList, settlementList]) => {
                    this.documentsService.documentsGet().pipe(take(1)).subscribe(documentsList => {
                        const documentsInDropdown = documentsList.map(d => ({ 
                            id: d.id,
                            date: d.date,
                            category: d.category,
                            invoiceNumber: d.invoiceNumber,
                            number: d.number,
                            text: this.buildInvoiceText(d)
                        }));
                        this.toolbarElements.push(
                            { name: 'save', title: 'Zapisz', defaultAction: ToolbarElementAction.SaveChanges} as ToolbarElement,
                            { name: 'delete', title: 'Usuń', defaultAction: ToolbarElementAction.Delete} as ToolbarElement,
                            { name: 'open', image: 'nc-image'},
                            { name: 'document', title: 'Dokument'} as ToolbarElement,
                            { name: 'auto-category', title: 'Autokategoria'} as ToolbarElement);
                        this.viewDefinition = {
                            fields: [
                                { title: 'Data', dataProperty: 'date', component: 'date', required: true} as DetailsViewField,
                                { title: 'Konto', dataProperty: 'account', component: 'list', required: true, options: { referenceList: accounts, referenceListIdField: 'title'} as DetailsViewFieldListOptions} as DetailsViewField,
                                { title: 'Kategoria', dataProperty: 'category', component: 'list', required: true, options: { referenceList: categories, referenceListIdField: 'title', usageIndexData: transactions, usageIndexPeriodDays: 40, usageIndexPeriodDateProperty: 'date', usageIndexThreshold: 5} as DetailsViewFieldListOptions} as DetailsViewField,
                                { title: 'Kwota', dataProperty: 'amount', component: 'amount', required: true, options: { currencyList: currencies, currencyListIdField: 'code', currencyDataProperty: 'currency'} as DetailsViewFieldAmountOptions} as DetailsViewField,
                                { title: 'Opis w banku', dataProperty: 'bankInfo', component: 'multiline-text', readonly: true} as DetailsViewField,
                                { title: 'Komentarz', dataProperty: 'comment', component: 'text'} as DetailsViewField,
                            //  { title: 'Szczegóły', dataProperty: 'details', component: 'text'} as DetailsViewField,
                                { title: 'Osoba', dataProperty: 'person', component: 'text'} as DetailsViewField,
                                { title: 'Sprawa', dataProperty: 'caseName', component: 'list', required: false, options: { referenceList: caseList, referenceListIdField: 'name'} as DetailsViewFieldListOptions} as DetailsViewField,
                                { title: 'Rozliczenie', dataProperty: 'settlement', component: 'list', required: false, options: { referenceList: settlementList, referenceListIdField: 'title', referenceListSortField: 'title', referenceListSortDescending: true} as DetailsViewFieldListOptions} as DetailsViewField,
                                { title: 'Dokument', dataProperty: 'documentId', component: 'list', required: false, options: { referenceList: documentsInDropdown, referenceListIdField: 'id', referenceListTextField: 'text', referenceListSortField: 'date', referenceListSortDescending: true} as DetailsViewFieldListOptions} as DetailsViewField,
                            ]
                        };
                    
                        this.data = transactions.filter(t => t.id == params['id'])[0];    
                    });
                });
        });
    }

    ngOnDestroy(){
        this.routeSubscription.unsubscribe();
    }

    buildInvoiceText(document : Document) : string {
        let result: string = '';
        const date = new Date(document.date);
        let month = (date.getMonth()+1).toString();
        if (month.length < 2) {
            month = '0' + month;
        }
        let day = (date.getDate()).toString();
        if (day.length < 2) {
            day = '0' + day;
        }
        result += date.getFullYear().toString()+'-'+month+'-'+day;

        result += ' ' + document.number;

        if (document.category && document.invoiceNumber)
            result += ` (${document.category} - ${document.invoiceNumber})`;
        if (document.category && !document.invoiceNumber)
            result += ` (${document.category})`;
        if (!document.category && document.invoiceNumber)
            result += ` ${document.invoiceNumber})`;

        result += ' ';

        if (document.company) result += document.company + '/';
        if (document.description) result += document.description + '/';
        if (document.person) result += document.person + '/';
        if (document.car) result += document.car + '/';
        if (document.relatedObject) result += document.relatedObject + '/';
        if (result.endsWith('/')) result = result.substring(0, result.length-1);
        return result;
    }
    
    toolbarElementClick(toolbarElementWithData: ToolbarElementWithData) {
        if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.SaveChanges) {
            if (this.data) {
                
                this.transactionsService.transactionsTransactionPut(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            } else {

                this.transactionsService.transactionsTransactionPost(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            }
        } else if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.Delete) {
            this.transactionsService.transactionsTransactionIdDelete(toolbarElementWithData.data.id).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        } else if (toolbarElementWithData.toolbarElement.name==='auto-category') {
            this.router.navigate(['transaction-auto-categories','new'], { queryParams: {  bankInfo: encodeURIComponent(this.data.bankInfo) }})
        } else if (toolbarElementWithData.toolbarElement.name === 'document') {
            this.openDocument(toolbarElementWithData.data?.documentId);
        } else if (toolbarElementWithData.toolbarElement.name === 'open') {
            if (!toolbarElementWithData.data?.documentId)
                return;
            
            this.documentsService.documentsIdGet(toolbarElementWithData.data['documentId']).pipe(take(1)).subscribe(document => {
            if (!document.number || document.number == 0)
                return;

            if (!this.settingService.CurrentPassword)
            {
                this.modalService.open(this.password, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
                    this.settingService.CurrentPassword = result;
                    this.openFile(document.number);
                }, (reason) => { });
            } else {
                this.openFile(document.number);
            }});
            
        }
    }

    openDocument(documentId: string) {
        if (!documentId)
            return;
        
        const url = '/#' + this.router.createUrlTree(['/documents/' + documentId]).toString();
        window.open(url, '_blank');
    }

    openFile(documentNumber: number) {
        if (!this.settingService.CurrentPassword){
            return;
        }
        this.fileService.filePost({ number: documentNumber, password: this.settingService.CurrentPassword}).pipe(take(1)).subscribe((result: DecompressFileResult) => {
            window.open("http://127.0.0.1:8080" + result.path, "_blank", "noopener noreferrer");
        });
    }
}
