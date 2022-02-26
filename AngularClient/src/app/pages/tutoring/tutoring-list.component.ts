import { Component, OnInit } from '@angular/core';
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { forkJoin } from 'rxjs';
import { TransactionsService, TutoringListService, TutoringService } from 'app/api/generated';
import { ListFilterOptions } from 'app/shared/list-filter/list-filter.component';
import { Summary, SummaryTotalNumberOptions } from '../list-page/list-page.component';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';

@Component({
    moduleId: module.id,
    template: `
        <list-page 
        name="tutoring" 
        [columns]="columns" 
        [data]="data" 
        initialSortColumn="date" 
        initialSortOrder=-1
        [summaries]="summaries">
        </list-page>
    `
})
export class TutoringListComponent implements OnInit{
    data: any[]; 
    columns: GridColumn[];
    summaries: Summary[] = [];
    
    constructor (private tutoringService: TutoringService, private transactionsService: TransactionsService, private tutoringListService: TutoringListService) {}

    ngOnInit(){
        forkJoin([this.tutoringService.tutoringGet(), this.transactionsService.transactionsGet(), this.tutoringListService.tutoringListGet()])
        .pipe(take(1)).subscribe(([tutoringList, transactions, tutoringListList]) =>{
            const tutoringCategoryMap = tutoringListList.map(t => t.transactionCategory.toUpperCase());
            const tutorTransactions = transactions.filter(t => tutoringCategoryMap.indexOf(t.category?.toUpperCase()) > -1).map(t => {
                return  { 
                    id: t.id, 
                    title: tutoringListList.find(l => l.transactionCategory.toUpperCase() === t.category?.toUpperCase()).title,
                    date: t.date,
                    count: null,
                    comment: t.comment,
                    amount: t.amount,
                    currency: t.currency
                }
            });
           
            const allData = [...tutoringList, ...tutorTransactions];
            this.data = allData;
            this.summaries.push( { name: 'total-number', options: { numberProperty: 'count' } as SummaryTotalNumberOptions})
           
            this.columns = [ 
                { title: 'Tytuł', dataProperty: 'title', component: 'list', filterOptions: { idProperty: 'title' } as ListFilterOptions},
                { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true},
                { title: 'Ilość', dataProperty: 'count', pipe: 'number', component: 'number', alignment: 'right'},
                { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amountwithempty', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions, customEvent: true},
                { title: 'Komentarz', dataProperty: 'comment', component: 'text'}
            ];
         });
    }
}
