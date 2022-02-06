import { Component, OnInit } from '@angular/core';
import { DocumentDatasetService, DocumentsService, Document } from 'app/api/generated';
import { GridColumn, RowClickedData } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';
import { ActivatedRoute, Router } from '@angular/router';
import { TextFilterOptions } from 'app/shared/text-filter/text-filter.component';

@Component({
    selector: 'transaction-auto-categories',
    moduleId: module.id,
    template: `
        <app-document-state></app-document-state>
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
    `
})
export class DocumentsComponent implements OnInit{
    data: Document[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];

    constructor (private documentsService: DocumentsService, 
        private router: Router, 
        private route: ActivatedRoute) {}

    ngOnInit(){
        this.documentsService.documentsGet().pipe(take(1)).subscribe((documents: Document[]) => {
            this.data = documents;
            this.columns = [ 
                { title: 'Numer', dataProperty: 'number', component: 'text'},
                { title: 'Data', dataProperty: 'date', component: 'date', noWrap: true, pipe: 'date'},
                { title: 'Firma', dataProperty: 'company', component: 'text'},
                { title: 'Opis', dataProperty: 'description', component: 'text'},
                { title: 'Faktura', dataProperty: 'invoiceNumber', component: 'text', noWrap: true},
                { title: 'Relacje', dataProperty: '', component: 'text', subDataProperty1: 'person', subDataProperty2: 'car', subDataProperty3: 'relatedObject', subDataProperty4: 'guarantee', filterOptions: { additionalPropertyToSearch1: 'person', additionalPropertyToSearch2: 'car', additionalPropertyToSearch3: 'relatedObject', additionalPropertyToSearch4: 'guarantee' } as TextFilterOptions},
                { title: '', dataProperty: 'nc-image', component: 'icon', image: 'nc-image', customEvent: true},
            ];

            this.toolbarElements.push(
                { name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew},
                { name: 'phone', title: 'Telefon'},
                { name: 'internet', title: 'Internet'},
                { name: 'ciklumTools', title: 'Ciklum narzędzia'},
                { name: 'fuel', title: 'Paliwo'},
                { name: 'mazda', title: 'Mazda'},
                { name: 'invoice', title: 'Faktura'},
                );
        });
    }
    
    rowClickedEvent(rowClickedData: RowClickedData) {
        let fileName = rowClickedData.row['number'].toString();
        while (fileName.length < 5)
            fileName = '0' + fileName;
        fileName = 'MX' + fileName + '.' + rowClickedData.row['extension'];
        window.open("http://127.0.0.1:8080/" +fileName, "_blank", "noopener noreferrer");
    }

    toolbarElementClick(toolbarElement: ToolbarElement) {
        this.router.navigate(["new", toolbarElement.name], { relativeTo: this.route});
    }
}
