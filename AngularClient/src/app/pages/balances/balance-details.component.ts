import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { BalancesService, CurrenciesService, GasService, TransactionsService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField, DetailsViewFieldAmountOptions, DetailsViewFieldListOptions } from "app/shared/details-view/details-view.component";
import { ToolbarElementAction, ToolbarElementWithData } from "app/shared/models/toolbar";
import { forkJoin, Subscription } from "rxjs";
import { take } from "rxjs/operators";
import { Location } from '@angular/common';

@Component({
    moduleId: module.id,
    template: `
        <details-view [viewDefinition]="viewDefinition" [data]="data"  (toolbarElementClick)="toolbarElementClick($event)"></details-view>
    `
})

export class BalanceDetailsComponent implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: any;
    private routeSubscription: Subscription;
    
    constructor(private balanceService: BalancesService, 
        private route: ActivatedRoute, 
        private transactionsService: TransactionsService,
        private currenciesService: CurrenciesService, 
        private location: Location) {  
    }

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe((params: Params) => {
            forkJoin([
                this.balanceService.balancesGet(),
                this.transactionsService.transactionsAccountsGet(),
                this.currenciesService.currenciesGet()
            ])
                .pipe(take(1)).subscribe(([balances, accounts, currencies]) => {
                    this.data = balances.filter(t => t.id == params['id'])[0];
                    this.viewDefinition = {
                        fields: [
                            { title: 'Data', dataProperty: 'date', component: 'date', required: true} as DetailsViewField,
                            { title: 'Konto', dataProperty: 'account', component: 'list', required: true, options: { referenceList: accounts, referenceListIdField: 'title'} as DetailsViewFieldListOptions} as DetailsViewField,
                            { title: 'Kwota', dataProperty: 'amount', component: 'amount', required: true, options: { currencyList: currencies, currencyListIdField: 'code', currencyDataProperty: 'currency'} as DetailsViewFieldAmountOptions} as DetailsViewField,
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
                this.balanceService.balancesBalancePut(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            } else {
                this.balanceService.balancesBalancePost(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            }
        } else if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.Delete) {
            this.balanceService.balancesBalanceIdDelete(toolbarElementWithData.data.id).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
    }
}
