import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DecompressFileResult, FileService, MBankScrapperService, SantanderScrapperService, Transaction } from 'app/api/generated';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { GridColumn, RowClickedData } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { Summary, SummaryAmountCurrencyOptions} from '../list-page/list-page.component';
import { ListFilterOptions } from 'app/shared/list-filter/list-filter.component';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';
import { TextFilterOptions } from 'app/shared/text-filter/text-filter.component';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SettingsService } from 'app/api/settingsService';
@Component({
    selector: 'transactions',
    moduleId: module.id,
    template: `
        <list-page 
            name="transactions" 
            [columns]="columns" 
            [data]="data" 
            initialSortColumn="date" 
            initialSortOrder=-1 
            [summaries]="summaries" 
            (rowClicked)="rowClickedEvent($event)"
            [toolbarElements]="toolbarElements" 
            (toolbarElementClick)="toolbarElementClick($event)">
        </list-page>
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
export class TransactionsComponent implements OnInit{
    data: Transaction[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];
    summaries: Summary[] = [];
    params: Params;
    @ViewChild('password', { static: true }) password: ElementRef;
    
    constructor (
        private transactionsService: TransactionsService, 
        private mbankScrappingService: MBankScrapperService, 
        private santanderScrapperService: SantanderScrapperService, 
        private fileService: FileService,
        private settingService: SettingsService,
        private modalService: NgbModal,
        private router: Router, 
        private route: ActivatedRoute) {
    }

    ngOnInit(){
        this.route.queryParams.subscribe((qp: Params) => {
            this.params = qp;
            this.transactionsService.transactionsGet()
            .pipe(take(1))
            .subscribe(result => {
                this.data = result.filter(t => this.params?.savings === "1" || t.category!=="Oszczędzanie");     
            });
        });
        this.summaries.push( { name: 'amount-currency', options: { amountProperty: 'amount', currencyProperty: 'currency' } as SummaryAmountCurrencyOptions})
        this.toolbarElements.push({ name: 'mBank', title: 'mBank' });  
        this.toolbarElements.push({ name: 'santander', title: 'Santander' });  
        this.toolbarElements.push({ name: 'savings', title: 'Oszczędzanie' });  
        this.toolbarElements.push({ name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew});
        this.columns = [ 
           // { title: 'id', dataProperty: 'id', component: 'text'},
            { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true},
            { title: 'Konto', dataProperty: 'account', component: 'list', filterOptions: { idProperty: 'account' } as ListFilterOptions},
            { title: 'Kategoria', dataProperty: 'category', component: 'list', filterOptions: { idProperty: 'category', usageIndexPeriodDays: 40, usageIndexThreshold: 5, usageIndexPeriodDateProperty: 'date' } as ListFilterOptions},
            { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amount', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions},
            { title: 'Opis', dataProperty: 'bankInfo', subDataProperty1: 'comment', subDataProperty2:'caseName', subDataProperty3:'settlement', component: 'text', filterOptions: { additionalPropertyToSearch1: 'comment', additionalPropertyToSearch2: 'caseName', additionalPropertyToSearch3: 'settlement' } as TextFilterOptions},
            { title: '', dataProperty: 'documentId', component: 'icon', customEvent: true, image: 'nc-image', conditionalFormatting: 'bool'}
        ];
    }

    toolbarElementClick(toolbarElement: ToolbarElement) {
        if (toolbarElement.name === 'mBank') {
            this.mbankScrappingService.mBankScrapperPost().pipe(take(1)).subscribe(t => {
                console.log(t);
            });
        }
        if (toolbarElement.name === 'savings') {
            let params = { ...this.params, savings : (this.params.savings==="1") ? "0" : "1" };
            this.router.navigate([], { relativeTo: this.route,  
                queryParams: params, 
                queryParamsHandling: "merge" });
        }
        if (toolbarElement.name === 'santander') {
            this.santanderScrapperService.santanderScrapperPost().pipe(take(1)).subscribe(t => {
                console.log(t);
            });
        }
    }

    rowClickedEvent(rowClickedData: RowClickedData) {
        if (rowClickedData.column.dataProperty!='documentId') {
            this.router.navigate(['transactions', rowClickedData.row['id']]);
            return;
        } 
        if (!this.settingService.CurrentPassword)
        {
            this.modalService.open(this.password, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
                this.settingService.CurrentPassword = result;
                this.openFile(rowClickedData);
                }, (reason) => { });
        } else {
            this.openFile(rowClickedData);
        };
    }

    openFile(rowClickedData: RowClickedData) {
        if (!this.settingService.CurrentPassword){
            return;
        }
        let number = rowClickedData.row['documentNumber'];
        this.fileService.filePost({ number: number, password: this.settingService.CurrentPassword}).pipe(take(1)).subscribe((result: DecompressFileResult) => {
            window.open("http://127.0.0.1:8080" + result.path, "_blank", "noopener noreferrer");
        });
    }
}
