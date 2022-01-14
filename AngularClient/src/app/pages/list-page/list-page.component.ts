import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GridColumn } from 'app/shared/grid/grid.component';

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
                <div class="card-body">
                    <ng-container *ngFor="let toolbarElement of toolbarElements">
                        <button class="btn btn-primary" (click)="toolbarElementClicked(toolbarElement)">{{toolbarElement.title}}</button>
                    </ng-container>
                    <grid [name]="name" [columns]="columns" [data]="data"
                    [initialSortColumn]="initialSortColumn" [initialSortOrder]="initialSortOrder"></grid>
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
    @Output() toolbarElementClick = new EventEmitter<ToolbarElement>();

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
}
