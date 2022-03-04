import { Component, OnInit } from '@angular/core';
import { DocumentsService, TransactionsService, Document } from 'app/api/generated';
import { GridColumn, RowClickedData } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement } from 'app/shared/models/toolbar';
import { forkJoin } from 'rxjs';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';
import { ListFilterOptions } from 'app/shared/list-filter/list-filter.component';
import { ActivatedRoute, Router } from '@angular/router';
import { Summary } from '../list-page/list-page.component';

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
    `
})
export class CaseListComponent implements OnInit{
    data: any[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];
    summaries: Summary[] = [];
    
    constructor (
        private transactionsService: TransactionsService,
        private documentsService: DocumentsService,
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
                extension: undefined
            }));
            const documentsWithCase = documents.filter(d => d.caseName).map(d => ({ 
                id: d.id,
                date: d.date,
                caseName: d.caseName,
                category: 'document',
                amount: undefined,
                description: d.description,
                showImage: 1,
                number: d.number,
                extension: d.extension
            }));
            this.data = [...transactionsWithCase, ...documentsWithCase];
            this.columns = [ 
                { title: 'Sprawa', dataProperty: 'caseName', component: 'list', filterOptions: { idProperty: 'caseName'  } as ListFilterOptions, customEvent: true},
                { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true, customEvent: true},
                { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amountwithempty', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions, customEvent: true},
                { title: 'Opis', dataProperty: 'description', component: 'text', customEvent: true},
                { title: '', dataProperty: 'showImage', component: 'icon', customEvent: true, image: 'nc-image', conditionalFormatting: 'bool'}
            ];
            // this.summaries.push( { name: 'amount-category', options: { amountProperty: 'amount', categoryProperty: 'category', showDirectioned: false } as SummaryAmountCategoryOptions})            

        });
    }

    rowClickedEvent(rowClickedData: RowClickedData) {
        if (rowClickedData.row['category']==='transaction') {
            this.router.navigate(['transactions', rowClickedData.row['id']], { relativeTo: this.route});
        } else if (rowClickedData.row['category']==='document' && rowClickedData.column.dataProperty!=='showImage') {
            this.router.navigate(['documents', rowClickedData.row['id']]);
        } else if (rowClickedData.row['category']==='document' && rowClickedData.column.dataProperty==='showImage') {
            let fileName = rowClickedData.row['number'].toString();
            while (fileName.length < 5)
                fileName = '0' + fileName;
            fileName = 'MX' + fileName + '.' + rowClickedData.row['extension'];
            window.open("http://127.0.0.1:8080/" +fileName, "_blank", "noopener noreferrer");
        }
    }
        
}
