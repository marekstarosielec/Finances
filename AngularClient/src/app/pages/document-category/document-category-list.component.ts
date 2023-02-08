import { Component, OnInit } from '@angular/core';
import { DocumentCategory, DocumentCategoryService } from 'app/api/generated';
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement } from 'app/shared/models/toolbar';

@Component({
    selector: 'document-category',
    moduleId: module.id,
    template: `
        <list-page name="documentCategory" [columns]="columns" [data]="data" initialSortColumn="name" initialSortOrder=1></list-page>
    `
})
export class DocumentCategoryListComponent implements OnInit{
    data: DocumentCategory[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];

    constructor (private documentCategoryService: DocumentCategoryService) {}

    ngOnInit(){
        this.documentCategoryService.documentCategoryGet().pipe(take(1)).subscribe((documentCategoryList: DocumentCategory[]) =>{
            this.data = documentCategoryList;
        });
        this.columns = [ { title: 'Nazwa', dataProperty: 'name', component: 'text'}];
    }
}
