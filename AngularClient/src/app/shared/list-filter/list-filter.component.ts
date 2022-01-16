import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import * as fs from 'fast-sort';

@Component({
    selector: 'list-filter',
    templateUrl: 'list-filter.component.html',
    styleUrls: ['./list-filter.component.scss']
})

export class ListFilterComponent implements OnInit, OnDestroy {
    @Input() name: string;
    @Input() listTitleProperty: string;
    @Input() filterValue: string;
    @Input() list: any[];
    primaryList: any[];
    secondaryList: any[];
    noFilterElement: string;
    missingElement: string;
    otherElement: string;
    @Output() filterChanged = new EventEmitter<string>()

    constructor() {
    }

    ngOnInit() {
        this.noFilterElement = this.name + '_noFilterElement';
        this.missingElement = this.name + '_missingElement';
        this.otherElement = this.name + '_otherElement';

        this.primaryList = this.list.filter(l => !l.hasOwnProperty('usageIndex') || l.usageIndex > 5);
        this.primaryList = fs.sort(this.primaryList).by([
            { asc: l => l[this.listTitleProperty]}
        ]);
        this.secondaryList = this.list.filter(l => l.hasOwnProperty('usageIndex') && l.usageIndex <= 0);
    }

    ngOnDestroy()
    {

    }

    private setFilter(value?: string) : void {
        this.filterValue = value;
        this.filterChanged.emit(this.filterValue);
    }
}
