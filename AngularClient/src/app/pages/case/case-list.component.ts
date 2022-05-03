import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DocumentsService, TransactionsService, Document, FileService, DecompressFileResult } from 'app/api/generated';
import { GridColumn, RowClickedData } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement } from 'app/shared/models/toolbar';
import { forkJoin } from 'rxjs';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';
import { ListFilterOptions } from 'app/shared/list-filter/list-filter.component';
import { ActivatedRoute, Router } from '@angular/router';
import { Summary } from '../list-page/list-page.component';
import { SettingsService } from 'app/api/settingsService';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    moduleId: module.id,
    template: `
        <list-page 
        name="case" 
        [columns]="columns" 
        [data]="data" 
        initialSortColumn="date" 
        initialSortOrder=-1 
        [toolbarElements]="toolbarElements"
        (rowClicked)="rowClickedEvent($event)"
        [summaries]="summaries">
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
export class CaseListComponent implements OnInit{
    data: any[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];
    summaries: Summary[] = [];
    @ViewChild('password', { static: true }) password: ElementRef;
    
    constructor (
        private transactionsService: TransactionsService,
        private documentsService: DocumentsService,
        private fileService: FileService,
        private settingService: SettingsService,
        private modalService: NgbModal,
        private router: Router, 
        private route: ActivatedRoute) {}

    ngOnInit(){
        forkJoin([this.transactionsService.transactionsGet(), this.documentsService.documentsGet()])
        .pipe(take(1)).subscribe(([transactions, documents]) =>{
            const transactionsWithCase = transactions.filter(t => t.caseName).map(t => ({ 
                id: t.id,
                date: t.date,
                caseName: t.caseName,
                category: 'transaction',
                amount: t.amount,
                description: t.description,
                showImage: 0,
                number: undefined,
                account: t.account
            }));
            const documentsWithCase = documents.filter(d => d.caseName).map(d => ({ 
                id: d.id,
                date: d.date,
                caseName: d.caseName,
                category: 'document',
                amount: undefined,
                description: d.description,
                showImage: 1,
                number: d.number
            }));
            this.data = [...transactionsWithCase, ...documentsWithCase];
            this.columns = [ 
                { title: 'Sprawa', dataProperty: 'caseName', component: 'list', filterOptions: { idProperty: 'caseName'  } as ListFilterOptions, customEvent: true},
                { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true, customEvent: true},
                { title: 'Konto', dataProperty: 'account', component: 'text', customEvent: true},
                 { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amountwithempty', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions, customEvent: true},
                { title: 'Opis', dataProperty: 'description', component: 'text', customEvent: true},
                { title: '', dataProperty: 'showImage', component: 'icon', customEvent: true, image: 'nc-image', conditionalFormatting: 'bool'}
            ];
            // this.summaries.push( { name: 'amount-category', options: { amountProperty: 'amount', categoryProperty: 'category', showDirectioned: false } as SummaryAmountCategoryOptions})            

        });
    }

    rowClickedEvent(rowClickedData: RowClickedData) {
        if (rowClickedData.row['category']==='transaction') {
            this.router.navigate(['transactions', rowClickedData.row['id']]);
        } else if (rowClickedData.row['category']==='document' && rowClickedData.column.dataProperty!=='showImage') {
            this.router.navigate(['documents', rowClickedData.row['id']]);
        } else if (rowClickedData.row['category']==='document' && rowClickedData.column.dataProperty==='showImage') {
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
