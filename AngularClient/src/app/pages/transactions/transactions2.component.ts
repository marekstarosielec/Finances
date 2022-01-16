import { Component, OnInit } from '@angular/core';
import { Transaction } from 'app/api/generated';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement, ToolbarElementAction } from '../list-page/list-page.component';
import { forkJoin } from 'rxjs';

@Component({
    selector: 'transactions',
    moduleId: module.id,
    template: `
        <list-page name="transactions" [columns]="columns" [data]="data" initialSortColumn="date" initialSortOrder=-1></list-page>
    `
})
export class Transactions2Component implements OnInit{
    data: Transaction[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];

    constructor (private transactionsService: TransactionsService) {}

    ngOnInit(){
        forkJoin([
            this.transactionsService.transactionsGet(),
            this.transactionsService.transactionsAccountsGet(),
            this.transactionsService.transactionsCategoriesGet()
            ])
            .pipe(take(1))
            .subscribe(result => {
                this.data = result[0];
                this.columns = [ 
                    { title: 'Data', dataProperty: 'date', pipe: 'date', filterComponent: 'date'},
                    { title: 'Konto', dataProperty: 'account', filterComponent: 'list' , filterComponentData: result[1], filterComponentData2: 'title'},
                    { title: 'Kategoria', dataProperty: 'category', filterComponent: 'list', filterComponentData: result[2], filterComponentData2: 'title'},
                    { title: 'Kwota', dataProperty: 'amount', dataProperty2: 'currency', pipe: 'amount', alignment: 'right', conditionalFormatting: 'amount'}
                ];
            });
    }
}
