import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GridColumn } from 'app/shared/grid/grid.component';

@Component({
    selector: 'list-page',
    //templateUrl: 'list-page.component.html',
    styleUrls: ['./list-page.component.scss'],
    template: `
    <div class="row">
        <div class="col-md-12">
            <div class="card">
                <div class="card-body">
                    <button (click)="addNew()" class="btn btn-primary">Dodaj</button>
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
    
    constructor(private router: Router, private route: ActivatedRoute) {
    }

    ngOnInit() {

    }

    ngOnDestroy()
    {

    }

    addNew() {
        this.router.navigate(["new"], { relativeTo: this.route});
    }
}
