import { Component, OnInit } from '@angular/core';
import { MBankScrapperService, Transaction } from 'app/api/generated';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { Summary, SummaryAmountCurrencyOptions, ToolbarElement, ToolbarElementAction } from '../list-page/list-page.component';
import { ListFilterOptions } from 'app/shared/list-filter/list-filter.component';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';
import { TextFilterOptions } from 'app/shared/text-filter/text-filter.component';

@Component({
    selector: 'transactions',
    moduleId: module.id,
    template: `
        <list-page name="transactions" [columns]="columns" [data]="data" initialSortColumn="date" initialSortOrder=-1 [summaries]="summaries" [toolbarElements]="toolbarElements" (toolbarElementClick)="toolbarElementClick($event)"></list-page>
    `
})
export class TransactionsComponent implements OnInit{
    data: Transaction[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];
    summaries: Summary[] = [];
    constructor (private transactionsService: TransactionsService, private mbankScrappingService: MBankScrapperService) {}

    ngOnInit(){
        this.transactionsService.transactionsGet()
        .pipe(take(1))
        .subscribe(result => {
            this.data = result;
            this.summaries.push( { name: 'amount-currency', options: { amountProperty: 'amount', currencyProperty: 'currency' } as SummaryAmountCurrencyOptions})
            this.toolbarElements.push({ name: 'mBank', title: 'mBank' });  
            this.toolbarElements.push({ name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew});
            this.columns = [ 
                { title: 'Data', dataProperty: 'date', pipe: 'date', filterComponent: 'date', noWrap: true},
                { title: 'Konto', dataProperty: 'account', filterComponent: 'list', filterOptions: { idProperty: 'account' } as ListFilterOptions},
                { title: 'Kategoria', dataProperty: 'category', filterComponent: 'list', filterOptions: { idProperty: 'category', usageIndexPeriodDays: 40, usageIndexThreshold: 5, usageIndexPeriodDateProperty: 'date' } as ListFilterOptions},
                { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amount', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', filterComponent: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions},
                { title: 'Opis', dataProperty: 'bankInfo', subDataProperty1: 'comment', filterComponent: 'text', filterOptions: { additionalPropertyToSearch1: 'comment' } as TextFilterOptions}
            ];
        });
    }

    toolbarElementClick(toolbarElement: ToolbarElement) {
        this.mbankScrappingService.mBankScrapperPost().pipe(take(1)).subscribe(t => {
            console.log(t);
        })
    }
}
