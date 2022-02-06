import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { SkodaService } from "app/api/generated";
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

export class SkodaDetailsComponent implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: any;
    private routeSubscription: Subscription;
    
    constructor(private skodaService: SkodaService, 
        private route: ActivatedRoute, 
        private location: Location) {  
    }

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe((params: Params) => {
            forkJoin([
                this.skodaService.skodaGet()])
                .pipe(take(1)).subscribe(([skoda]) => {
                    this.data = skoda.filter(t => t.id == params['id'])[0];
                    this.viewDefinition = {
                        fields: [
                            { title: 'Data', dataProperty: 'date', component: 'date', required: true} as DetailsViewField,
                            { title: 'Licznik', dataProperty: 'meter', component: 'text'} as DetailsViewField,
                            { title: 'Komentarz', dataProperty: 'comment', component: 'text'} as DetailsViewField
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
                this.skodaService.skodaSkodaPut(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            } else {
                this.skodaService.skodaSkodaPost(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            }
        } else if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.Delete) {
            this.skodaService.skodaSkodaIdDelete(toolbarElementWithData.data.id).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
    }
}
