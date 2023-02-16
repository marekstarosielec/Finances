import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DocumentsService, Document, FileService, DecompressFileResult } from 'app/api/generated';
import { GridColumn, RowClickedData } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';
import { ActivatedRoute, Router } from '@angular/router';
import { TextFilterOptions } from 'app/shared/text-filter/text-filter.component';
import { SettingsService } from 'app/api/settingsService';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';

@Component({
    selector: 'transaction-auto-categories',
    moduleId: module.id,
    template: `
        <list-page 
        name="documents" 
        [columns]="columns" 
        [data]="data" 
        initialSortColumn="number" 
        initialSortOrder=-1 
        [toolbarElements]="toolbarElements" 
        (toolbarElementClick)="toolbarElementClick($event)"
        (rowClicked)="rowClickedEvent($event)"
        ></list-page>
        
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
export class DocumentsComponent implements OnInit{
    data: Document[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];
    @ViewChild('password', { static: true }) password: ElementRef;
    
    constructor (private documentsService: DocumentsService, private fileService: FileService,
        private router: Router, 
        private route: ActivatedRoute,
        private settingService: SettingsService,
        private modalService: NgbModal) {}

    ngOnInit(){
        this.documentsService.documentsGet().pipe(take(1)).subscribe((documents: Document[]) => {
            this.data = documents.map(d => ({
                ...d,
                formattedAmounts: this.formatAmounts(d)
            }));
            this.columns = [ 
                { title: 'Numer', dataProperty: 'number', component: 'text'},
                { title: 'Data', dataProperty: 'date', component: 'date', noWrap: true, pipe: 'date'},
                { title: 'Firma', dataProperty: 'company', component: 'text'},
                { title: 'Opis', dataProperty: 'description', component: 'text'},
                { title: 'Faktura', dataProperty: 'invoiceNumber', component: 'text', noWrap: true, subDataProperty1: 'category', filterOptions: { additionalPropertyToSearch1: 'category'}},
                { title: 'Relacje', dataProperty: '', component: 'text', subDataProperty1: 'person', subDataProperty2: 'car', subDataProperty3: 'relatedObject', subDataProperty4: 'caseName', subDataProperty5: 'settlement', filterOptions: { additionalPropertyToSearch1: 'person', additionalPropertyToSearch2: 'car', additionalPropertyToSearch3: 'relatedObject', additionalPropertyToSearch4: 'caseName', additionalPropertyToSearch5: 'settlement' } as TextFilterOptions},
                { title: 'Kwota', dataProperty: 'formattedAmounts', alignment: 'right', noWrap:true, component: 'text'},
                { title: 'Tranzakcja', dataProperty: 'transactionCategory', component: 'text', subDataProperty1: 'transactionAmount', subDataProperty2: 'transactionBankInfo', subDataProperty3: 'transactionComment', filterOptions: { additionalPropertyToSearch1: 'transactionAmount', additionalPropertyToSearch2: 'transactionBankInfo', additionalPropertyToSearch3: 'transactionComment' } as TextFilterOptions},
                { title: '', dataProperty: '', component: 'icon', image: 'nc-image', customEvent: true},
            ];

            this.toolbarElements.push(
                { name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew},
                );
        });
    }

    formatAmounts(document: Document) : string {
        if (!document.net && !document.vat && !document.gross)
            return "";
        return this.formatAmount(document.net) + ' -> ' + this.formatAmount(document.vat) + ' -> '  + this.formatAmount(document.gross) + ' ' + document.currency;
    }

    formatAmount(amount: Number) : string {
        return amount.toFixed(2);
    }
    
    rowClickedEvent(rowClickedData: RowClickedData) {
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

    toolbarElementClick(toolbarElement: ToolbarElement) {
        this.router.navigate(["new", toolbarElement.name], { relativeTo: this.route});
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
