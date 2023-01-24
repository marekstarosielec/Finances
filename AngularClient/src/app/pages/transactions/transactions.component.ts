import { Component, OnInit } from '@angular/core';
import { MBankScrapperService, SantanderScrapperService, Transaction } from 'app/api/generated';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { Summary, SummaryAmountCurrencyOptions} from '../list-page/list-page.component';
import { ListFilterOptions } from 'app/shared/list-filter/list-filter.component';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';
import { TextFilterOptions } from 'app/shared/text-filter/text-filter.component';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';
import { ActivatedRoute, Params, Router } from '@angular/router';
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
    params: Params;
   
    constructor (private transactionsService: TransactionsService, private mbankScrappingService: MBankScrapperService, private santanderScrapperService: SantanderScrapperService, private router: Router, private route: ActivatedRoute) {

    }

    ngOnInit(){
        this.route.queryParams.subscribe((qp: Params) => {
            this.params = qp;
            this.transactionsService.transactionsGet()
            .pipe(take(1))
            .subscribe(result => {
                this.data = result.filter(t => this.params?.savings === "1" || t.category!=="Oszczędzanie");     
            });
        });
        this.summaries.push( { name: 'amount-currency', options: { amountProperty: 'amount', currencyProperty: 'currency' } as SummaryAmountCurrencyOptions})
        this.toolbarElements.push({ name: 'mBank', title: 'mBank' });  
        this.toolbarElements.push({ name: 'santander', title: 'Santander' });  
        this.toolbarElements.push({ name: 'savings', title: 'Oszczędzanie' });  
        this.toolbarElements.push({ name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew});
        this.columns = [ 
            { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true},
            { title: 'Konto', dataProperty: 'account', component: 'list', filterOptions: { idProperty: 'account' } as ListFilterOptions},
            { title: 'Kategoria', dataProperty: 'category', component: 'list', filterOptions: { idProperty: 'category', usageIndexPeriodDays: 40, usageIndexThreshold: 5, usageIndexPeriodDateProperty: 'date' } as ListFilterOptions},
            { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amount', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions},
            { title: 'Opis', dataProperty: 'bankInfo', subDataProperty1: 'comment', subDataProperty2:'caseName', subDataProperty3:'settlement', component: 'text', filterOptions: { additionalPropertyToSearch1: 'comment', additionalPropertyToSearch2: 'caseName', additionalPropertyToSearch3: 'settlement' } as TextFilterOptions}
        ];
    }

    toolbarElementClick(toolbarElement: ToolbarElement) {
        if (toolbarElement.name === 'mBank') {
            this.mbankScrappingService.mBankScrapperPost().pipe(take(1)).subscribe(t => {
                console.log(t);
            });
        }
        if (toolbarElement.name === 'savings') {
            let params = { ...this.params, savings : (this.params.savings==="1") ? "0" : "1" };
            this.router.navigate([], { relativeTo: this.route,  
                queryParams: params, 
                queryParamsHandling: "merge" });
        }
        if (toolbarElement.name === 'santander') {
            this.santanderScrapperService.santanderScrapperPost().pipe(take(1)).subscribe(t => {
                console.log(t);
            });
        }
    }
}
