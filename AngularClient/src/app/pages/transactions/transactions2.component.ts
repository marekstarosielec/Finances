import { Component, OnInit } from '@angular/core';
import { Transaction } from 'app/api/generated';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement, ToolbarElementAction } from '../list-page/list-page.component';
import { forkJoin } from 'rxjs';
import { ListFilterOptions } from 'app/shared/list-filter/list-filter.component';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';

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
            this.transactionsService.transactionsGet()
            .pipe(take(1))
            .subscribe(result => {
                this.data = result;
                this.columns = [ 
                    { title: 'Data', dataProperty: 'date', pipe: 'date', filterComponent: 'date', noWrap: true},
                    { title: 'Konto', dataProperty: 'account', filterComponent: 'list', filterOptions: { idProperty: 'account' } as ListFilterOptions},
                    { title: 'Kategoria', dataProperty: 'category', filterComponent: 'list', filterOptions: { idProperty: 'category', usageIndexPeriodDays: 40, usageIndexThreshold: 5, usageIndexPeriodDateProperty: 'date' } as ListFilterOptions},
                    { title: 'Kwota', dataProperty: 'amount', dataProperty2: 'currency', pipe: 'amount', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', filterComponent: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions},
                    { title: 'Opis', dataProperty: 'description'}
                ];
            });
    }
}
