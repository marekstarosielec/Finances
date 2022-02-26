import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { TransactionsService, TutoringListService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField, DetailsViewFieldListOptions } from "app/shared/details-view/details-view.component";
import { ToolbarElementAction, ToolbarElementWithData } from "app/shared/models/toolbar";
import { forkJoin, Subscription } from "rxjs";
import { take } from "rxjs/operators";
import { Location } from '@angular/common';

@Component({
    moduleId: module.id,
    template: `
        <details-view [viewDefinition]="viewDefinition" [data]="data" (toolbarElementClick)="toolbarElementClick($event)"></details-view>
    `
})

export class TutoringListDetailsComponent implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: any;
    private routeSubscription: Subscription;
    
    constructor(private tutoringListService: TutoringListService, 
        private route: ActivatedRoute, 
        private transactionsService: TransactionsService,
        private location: Location) {  
    }

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe((params: Params) => {
            forkJoin([
                this.tutoringListService.tutoringListGet(),
                this.transactionsService.transactionsCategoriesGet()])
                .pipe(take(1)).subscribe(([tutoringList, categories]) => {
                    this.data = tutoringList.filter(t => t.id == params['id'])[0];
                    this.viewDefinition = {
                        fields: [
                             { title: 'Tytuł', dataProperty: 'title', component: 'text'} as DetailsViewField,
                             { title: 'Opis', dataProperty: 'description', component: 'text'} as DetailsViewField,
                             { title: 'Kategoria tranzakcji', dataProperty: 'transactionCategory', component: 'list', required: true, options: { referenceList: categories, referenceListIdField: 'title' } as DetailsViewFieldListOptions} as DetailsViewField,
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
                this.tutoringListService.tutoringListTutoringListPut(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            } else {
                this.tutoringListService.tutoringListTutoringListPost(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            }
        } else if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.Delete) {
            this.tutoringListService.tutoringListTutoringListIdDelete(toolbarElementWithData.data.id).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
    }
}
