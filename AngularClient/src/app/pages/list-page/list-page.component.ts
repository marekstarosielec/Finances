import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GridColumn, ViewChangedData } from 'app/shared/grid/grid.component';
import { Subject } from 'rxjs';

export enum ToolbarElementAction {
    AddNew
}

export interface ToolbarElement {
    name: string;
    title: string;
    defaultAction?: ToolbarElementAction;
}

@Component({
    selector: 'list-page',
    styleUrls: ['./list-page.component.scss'],
    template: `
    <div class="row">
        <div class="col-md-12">
            <div class="card">
                <div class="card-header">
                    <ng-container *ngIf="currentView$ | async as currentView">
                        <span *ngIf="currentView.maximumVisibleNumberOfRecords != 0 && currentView.displayedData.length < currentView.filteredData.length && currentView.displayedData.length > 0">
                            Widoczne {{currentView.displayedData.length | formattedNumber}} z {{currentView.filteredData.length | formattedNumber}} rekordów. <span class="link" (click)="maximumNumberOfRecordsToShow=0">Pokaż wszystkie.</span>
                        </span>
                        <span *ngIf="currentView.maximumVisibleNumberOfRecords == 0 && currentView.filteredData.length > 0">
                            Widoczne wszystkie {{currentView.filteredData.length | formattedNumber}} rekordów. <span class="link" (click)="maximumNumberOfRecordsToShow=100">Pokaż pierwsze 100.</span>
                        </span>
                        <span *ngIf="currentView.maximumVisibleNumberOfRecords != 0 && currentView.displayedData.length == currentView.filteredData.length && currentView.displayedData.length > 0">
                            Widoczne wszystkie {{currentView.displayedData?.length| formattedNumber}} rekordów.
                        </span>
                        <span *ngIf="currentView.displayedData.length == 0">
                            Brak rekordów.
                        </span> 
                    </ng-container>
                </div>
                <div class="card-body">
                    <ng-container *ngFor="let toolbarElement of toolbarElements">
                        <button class="btn btn-primary" (click)="toolbarElementClicked(toolbarElement)">{{toolbarElement.title}}</button>
                    </ng-container>
                    <grid [name]="name" [columns]="columns" [data]="data"
                        [initialSortColumn]="initialSortColumn" [initialSortOrder]="initialSortOrder"
                        [maximumNumberOfRecordsToShow] = "maximumNumberOfRecordsToShow"
                        (viewChanged)="viewChanged($event)"></grid>
                </div>
            </div>
        </div>
    </div>
    `
})

export class ListPageComponent implements OnInit, OnDestroy {
    @Input() public name: string;
    @Input() public columns: GridColumn[];
    @Input() public data: any[];
    @Input() public initialSortColumn: string;
    @Input() public initialSortOrder: number;
    @Input() public toolbarElements: ToolbarElement[];
    @Output() public toolbarElementClick = new EventEmitter<ToolbarElement>();

    maximumNumberOfRecordsToShow: Number = 100;
    public currentView$: Subject<ViewChangedData> = new Subject<ViewChangedData>();

    constructor(private router: Router, private route: ActivatedRoute) {
    }

    ngOnInit() {
        if (!this.toolbarElements)
            this.toolbarElements = [{ name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew}];
    }

    ngOnDestroy()
    {

    }

    toolbarElementClicked(toolbarElement: ToolbarElement) {
        if (toolbarElement.defaultAction === ToolbarElementAction.AddNew)
            this.router.navigate(["new"], { relativeTo: this.route});
        else
            this.toolbarElementClick.emit(toolbarElement);
    }
    
    viewChanged(viewChangedData: ViewChangedData) {
        setTimeout(() => {
            this.currentView$.next(viewChangedData);
        }, 0);
    }
}
