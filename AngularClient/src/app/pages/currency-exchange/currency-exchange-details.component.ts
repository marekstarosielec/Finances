import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { CurrencyExchangeService, TransactionsService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField } from "app/shared/details-view/details-view.component";
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

export class CurrencyExchangeDetailsComponent implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: any;
    private routeSubscription: Subscription;
    
    constructor(private currencyExchangeService: CurrencyExchangeService,
        private route: ActivatedRoute, 
        private location: Location) {  
    }

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe((params: Params) => {
            forkJoin([
                this.currencyExchangeService.currencyExchangeGet()])
                .pipe(take(1)).subscribe(([categories]) => {
                    this.data = categories.filter(t => t.id == params['id'])[0];
                    this.viewDefinition = {
                        fields: [
                            { title: 'Data', dataProperty: 'date', component: 'date'} as DetailsViewField,
                            { title: 'Waluta', dataProperty: 'code', component: 'text'} as DetailsViewField,
                            { title: 'Kurs', dataProperty: 'rate', component: 'number'} as DetailsViewField
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
                this.currencyExchangeService.currencyExchangeCurrencyExchangePut(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            } else {
                this.currencyExchangeService.currencyExchangeCurrencyExchangePost(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            }
        } else if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.Delete) {
            this.currencyExchangeService.currencyExchangeCurrencyExchangeIdDelete(toolbarElementWithData.data.id).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
    }
}
