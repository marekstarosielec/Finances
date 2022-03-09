import { Component, OnInit } from '@angular/core';
import { Balance, BalancesService, TransactionAccount, TransactionsService } from 'app/api/generated';
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement } from 'app/shared/models/toolbar';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';
import { SubviewDefinition, SubViewService } from 'app/services/subview.service';
import { forkJoin } from 'rxjs';
import * as fs from 'fast-sort';
import { BalanceSummaryService } from 'app/api/balanceSummaryService';

@Component({
    moduleId: module.id,
    template: `
        <list-page name="balances" [columns]="columns" [data]="data" initialSortColumn="date" initialSortOrder=-1 [toolbarElements]="toolbarElements" ></list-page>
    `
})
export class BalanceSummaryListComponent implements OnInit{
    data: Balance[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];

    constructor (private balanceSummaryService: BalanceSummaryService, private subviewService: SubViewService, private transactionsService: TransactionsService) {}

    ngOnInit(){
        
        this.subviewService.subviews.next([
            { title: '', icon: 'nc-tile-56', link: 'balancesummary'} as SubviewDefinition,
            { title: '', icon: 'nc-single-copy-04', link: 'balances' } as SubviewDefinition
        ]);
        forkJoin([
            this.balanceSummaryService.GetSummary(),
            this.transactionsService.transactionsAccountsGet(),
        ]).pipe(take(1)).subscribe(([balances, accounts]) => {
            this.data = balances;
            this.columns = [ 
                { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true},
            ];
            accounts.forEach(account => {
                if (!account.deleted) {
                    const accountTitle = this.sanitizeAccountTitle(account.title);
                    this.columns.push({ title: account.title, dataProperty: accountTitle, additionalDataProperty1: accountTitle + '_currency',  pipe: 'amountwithempty', alignment: 'right', noWrap:true, component: 'amount', filterOptions: { currencyDataProperty: accountTitle + '_currency'} as AmountFilterOptions});
                }
            });
        });
    }

    sanitizeAccountTitle(accountTitle: string) : string {
        let result = accountTitle;
        result = result.toLowerCase();
        result = this.replaceAll(result, ' ', '_');
        result = this.replaceAll(result, 'ą', 'a');
        result = this.replaceAll(result, 'ć', 'c');
        result = this.replaceAll(result, 'ę', 'e');
        result = this.replaceAll(result, 'ł', 'l');
        result = this.replaceAll(result, 'ń', 'n');
        result = this.replaceAll(result, 'ó', 'o');
        result = this.replaceAll(result, 'ś', 's');
        result = this.replaceAll(result, 'ź', 'z');
        result = this.replaceAll(result, 'ż', 'z');
        return result;
    }

    replaceAll(str, find, replace) : string {
        return str.replace(new RegExp(find, 'g'), replace);
    }
}
