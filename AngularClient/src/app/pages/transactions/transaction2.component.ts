import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { Transaction, TransactionsService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField } from "app/shared/details-view/details-view.component";
import { Subscription } from "rxjs";
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
    
    constructor(private transactionsService: TransactionsService, private route: ActivatedRoute) {

        
    }
    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe((params: Params) => {
            this.transactionsService.transactionsIdGet(params['id']).pipe(take(1)).subscribe((result: Transaction) => {
                this.data = result;
                this.viewDefinition = {
                    fields: [
                        { title: 'Data', dataProperty: 'date', component: 'date'} as DetailsViewField,
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
