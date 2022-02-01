import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { TransactionsService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField, DetailsViewFieldListOptions } from "app/shared/details-view/details-view.component";
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

export class TransactionAutoCategoryComponent implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: any;
    private routeSubscription: Subscription;
    
    constructor(private transactionsService: TransactionsService, 
        private route: ActivatedRoute, 
        private location: Location) {  
    }

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe((params: Params) => {
            forkJoin([
                this.transactionsService.transactionsAutocategoriesGet(),
                this.transactionsService.transactionsGet(),
                this.transactionsService.transactionsCategoriesGet()])
                .pipe(take(1)).subscribe(([autocategories, transactions, categories]) => {
                    this.data = autocategories.filter(t => t.id == params['id'])[0];
                    const bankInfo = this.route.snapshot.queryParams["bankInfo"];
                    this.viewDefinition = {
                        fields: [
                            { title: 'Opis w banku', dataProperty: 'bankInfo', component: 'text', required: true, defaultValue: bankInfo ? decodeURIComponent(bankInfo) : undefined } as DetailsViewField,
                            { title: 'Kategoria', dataProperty: 'category', component: 'list', required: true, options: { referenceList: categories, referenceListIdField: 'title', usageIndexData: transactions, usageIndexPeriodDays: 40, usageIndexPeriodDateProperty: 'date', usageIndexThreshold: 5} as DetailsViewFieldListOptions} as DetailsViewField,          
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
                this.transactionsService.transactionsAutocategoryPut(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            } else {
                this.transactionsService.transactionsAutocategoryPost(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            }
        } else if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.Delete) {
            this.transactionsService.transactionsAutocategoryIdDelete(toolbarElementWithData.data.id).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
    }
}
