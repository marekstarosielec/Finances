import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { BalancesService, CurrenciesService, GasService, TransactionsService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField, DetailsViewFieldAmountOptions, DetailsViewFieldListOptions } from "app/shared/details-view/details-view.component";
import { ToolbarElement, ToolbarElementAction, ToolbarElementWithData } from "app/shared/models/toolbar";
import { forkJoin, Subscription } from "rxjs";
import { take } from "rxjs/operators";
import { Location } from '@angular/common';
import { BalanceSummaryService } from "app/api/balanceSummaryService";

@Component({
    moduleId: module.id,
    template: `
        <details-view [viewDefinition]="viewDefinition" [data]="data"  (toolbarElementClick)="toolbarElementClick($event)" [toolbarElements]="toolbarElements"></details-view>
    `
})

export class BalanceSummaryDetailsComponent implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: any;
    private routeSubscription: Subscription;
    toolbarElements: ToolbarElement[] = [{ name: 'save', title: 'Zapisz', defaultAction: ToolbarElementAction.SaveChanges}];
    
    constructor(private balanceSummaryService: BalanceSummaryService, 
        private route: ActivatedRoute, 
        private transactionsService: TransactionsService,
        private currenciesService: CurrenciesService, 
        private location: Location) {  
    }

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe((params: Params) => {
            forkJoin([
                this.balanceSummaryService.GetSummary(),
                this.transactionsService.transactionsAccountsGet(),
                this.currenciesService.currenciesGet()
            ])
                .pipe(take(1)).subscribe(([balances, accounts, currencies]) => {
                    this.data = balances.filter(t => t.id == params['id'])[0];
                    this.viewDefinition = {
                        fields: [
                            { title: 'Data', dataProperty: 'date', component: 'date', required: true} as DetailsViewField,
                        ]
                    };
                    accounts.forEach(account => {
                        const accountTitle = this.sanitizeAccountTitle(account.title);
                        if (!this.data.hasOwnProperty(accountTitle)) {
                            this.data[accountTitle] = undefined;
                            this.data[accountTitle + '_currency'] = 'PLN';
                            this.data[accountTitle + '_title'] = account.title;
                        }
                        this.viewDefinition.fields.push(
                            { title: account.title, dataProperty: accountTitle, component: 'amount', required: false, options: { currencyList: currencies, currencyListIdField: 'code', currencyDataProperty: accountTitle + '_currency', allowEmpty: true} as DetailsViewFieldAmountOptions} as DetailsViewField
                        );
                    });
                });
        });
    }

    ngOnDestroy(){
        this.routeSubscription.unsubscribe();
    }

    toolbarElementClick(toolbarElementWithData: ToolbarElementWithData) {
        if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.SaveChanges && this.data) {
            this.balanceSummaryService.SaveChanges(toolbarElementWithData.data)
            .subscribe(() =>
            {
                this.location.back();
            });
        }
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
