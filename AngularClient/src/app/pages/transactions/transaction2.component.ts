import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { CurrenciesService, Transaction, TransactionsService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField, DetailsViewFieldAmountOptions, DetailsViewFieldListOptions } from "app/shared/details-view/details-view.component";
import { forkJoin, Subscription } from "rxjs";
import { take } from "rxjs/operators";

@Component({
    moduleId: module.id,
    template: `
        <details-view [viewDefinition]="viewDefinition" [data]="data"></details-view>
    `
})

export class Transaction2Component implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: any;
    private routeSubscription: Subscription;
    
    constructor(private transactionsService: TransactionsService, private currenciesService: CurrenciesService, private route: ActivatedRoute) {

        
    }
    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe((params: Params) => {
            forkJoin([
                this.transactionsService.transactionsGet(),
                this.transactionsService.transactionsAccountsGet(),
                this.transactionsService.transactionsCategoriesGet(),
                this.currenciesService.currenciesGet(), 
                ])
                .pipe(take(1)).subscribe(([transactions, accounts, categories, currencies]) => {
                    this.data = transactions.filter(t => t.id == params['id'])[0];
                    this.viewDefinition = {
                        fields: [
                            { title: 'Data', dataProperty: 'date', component: 'date'} as DetailsViewField,
                            { title: 'Konto', dataProperty: 'account', component: 'list', options: { referenceList: accounts, referenceListIdField: 'title'} as DetailsViewFieldListOptions} as DetailsViewField,
                            { title: 'Kategoria', dataProperty: 'category', component: 'list', options: { referenceList: categories, referenceListIdField: 'title', usageIndexData: transactions, usageIndexPeriodDays: 40, usageIndexPeriodDateProperty: 'date', usageIndexThreshold: 5} as DetailsViewFieldListOptions} as DetailsViewField,
                            { title: 'Kwota', dataProperty: 'amount', component: 'amount', options: { currencyList: currencies, currencyListIdField: 'code', currencyDataProperty: 'currency'} as DetailsViewFieldAmountOptions} as DetailsViewField,
                            { title: 'Opis w banku', dataProperty: 'bankInfo', component: 'multiline-text', readonly: true} as DetailsViewField,
                            { title: 'Komentarz', dataProperty: 'comment', component: 'text'} as DetailsViewField,
                            { title: 'Szczegóły', dataProperty: 'details', component: 'text'} as DetailsViewField,
                            { title: 'Osoba', dataProperty: 'person', component: 'text'} as DetailsViewField
                        ]
                    };
                });
        });
    }

    ngOnDestroy(){
        this.routeSubscription.unsubscribe();
    }
}
