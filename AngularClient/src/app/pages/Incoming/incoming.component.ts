import { Component, OnInit } from '@angular/core';
import { DocumentCategory, DocumentCategoryService, Incoming, IncomingService } from 'app/api/generated';
import { GridColumn, RowCheckedData } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';
import { Router } from '@angular/router';

@Component({
    selector: 'incoming',
    moduleId: module.id,
    template: `
        <list-page 
        name="incoming" 
        [columns]="columns" 
        [data]="data" 
        initialSortColumn="sortableFileName" 
        initialSortOrder=0
        showCheckboxes=true
        (rowChecked)="rowCheckedEvent($event)"
        [toolbarElements]="toolbarElements"
        (toolbarElementClick)="toolbarElementClick($event)"
        ></list-page>
    `
})
export class IncomingComponent implements OnInit{
    data: Incoming[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];
    checkedRows: Incoming[] = [];

    constructor (private incomingService: IncomingService, private router: Router) {}

    ngOnInit(){
        this.incomingService.incomingGet().pipe(take(1)).subscribe((incomingList: Incoming[]) =>{
            this.data = incomingList;
        });
        this.columns = [ { title: 'Nazwa', dataProperty: 'fileName', component: 'text'}];
    }

    rowCheckedEvent(rowCheckedData: RowCheckedData) {
        if (rowCheckedData.rows.length === 1 )
            this.toolbarElements = [{ name: 'saveDoc', title: 'Do dokumentów'}];
        else if (rowCheckedData.rows.length >1 && this.onlyJpegs(rowCheckedData.rows))
            this.toolbarElements = [{ name: 'savePdf', title: 'Zapisz jako PDF'}];
        else
            this.toolbarElements = [];
        this.checkedRows = rowCheckedData.rows;
    }

    onlyJpegs(rows: any[]) : boolean
    {
        return rows.filter(r => !r.fileName.toLowerCase().endsWith(".jpg")).length === 0;
    }

    toolbarElementClick(toolbarElement: ToolbarElement) {
        if (toolbarElement.name==="saveDoc"){
            this.incomingService.incomingPost(this.checkedRows[0]).pipe(take(1)).subscribe((result:string) =>
            {
                this.router.navigate(['documents', result]);
            });
        } else if (toolbarElement.name==="savePdf"){
            this.incomingService.incomingConvertToPdfPost(this.checkedRows).pipe(take(1)).subscribe((result:string) =>
            {
                this.router.navigate(['documents', result]);
            });
        }
    }
}
