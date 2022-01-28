import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { CurrenciesService, TransactionsService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField, DetailsViewFieldAmountOptions, DetailsViewFieldListOptions } from "app/shared/details-view/details-view.component";
import { ToolbarElement, ToolbarElementAction, ToolbarElementWithData } from "app/shared/models/toolbar";
import { forkJoin, Subscription } from "rxjs";
import { Location } from '@angular/common';
import { take } from "rxjs/operators";

@Component({
    moduleId: module.id,
    template: `
        <details-view [viewDefinition]="viewDefinition" [data]="data" [toolbarElements]="toolbarElements" (toolbarElementClick)="toolbarElementClick($event)"></details-view>
    `
})

export class Transaction2Component implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: any;
    private routeSubscription: Subscription;
    toolbarElements: ToolbarElement[] = [];
    
    constructor(private transactionsService: TransactionsService, 
        private currenciesService: CurrenciesService, 
        private route: ActivatedRoute, 
        private location: Location) {  
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
                    this.toolbarElements.push(
                        { name: 'save', title: 'Zapisz', defaultAction: ToolbarElementAction.SaveChanges} as ToolbarElement,
                        { name: 'delete', title: 'Usuń', defaultAction: ToolbarElementAction.Delete} as ToolbarElement);
                    this.data = transactions.filter(t => t.id == params['id'])[0];
                    this.viewDefinition = {
                        fields: [
                            { title: 'Data', dataProperty: 'date', component: 'date', required: true} as DetailsViewField,
                            { title: 'Konto', dataProperty: 'account', component: 'list', required: true, options: { referenceList: accounts, referenceListIdField: 'title'} as DetailsViewFieldListOptions} as DetailsViewField,
                            { title: 'Kategoria', dataProperty: 'category', component: 'list', required: true, options: { referenceList: categories, referenceListIdField: 'title', usageIndexData: transactions, usageIndexPeriodDays: 40, usageIndexPeriodDateProperty: 'date', usageIndexThreshold: 5} as DetailsViewFieldListOptions} as DetailsViewField,
                            { title: 'Kwota', dataProperty: 'amount', component: 'amount', required: true, options: { currencyList: currencies, currencyListIdField: 'code', currencyDataProperty: 'currency'} as DetailsViewFieldAmountOptions} as DetailsViewField,
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

    
    toolbarElementClick(toolbarElementWithData: ToolbarElementWithData) {
        if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.SaveChanges) {
            if (this.data) {
                this.transactionsService.transactionsTransactionPut(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            } else {
                this.transactionsService.transactionsTransactionPost(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            }
        } else if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.Delete) {
            this.transactionsService.transactionsTransactionIdDelete(toolbarElementWithData.data.id).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
    }
}
