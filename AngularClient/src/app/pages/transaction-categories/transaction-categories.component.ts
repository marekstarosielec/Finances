import { Component, OnInit } from '@angular/core';
import { TransactionAccount, TransactionCategory } from 'app/api/generated';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';

@Component({
    selector: 'transaction-categories',
    moduleId: module.id,
    template: `
        <list-page name="transaction-categories" [columns]="columns" [data]="data" initialSortColumn="title" initialSortOrder=1 ></list-page>
    `
})
export class TransactionCategoriesComponent implements OnInit{
    data: TransactionAccount[]; 
    columns: GridColumn[];

    constructor (private transactionsService: TransactionsService) {}

    ngOnInit(){
        this.transactionsService.transactionsCategoriesGet().pipe(take(1)).subscribe((transactionCategories: TransactionCategory[]) =>{
            this.data = transactionCategories;
        });
        this.columns = [ { title: 'Nazwa', dataProperty: 'title', component: 'text'}];
    }
}
