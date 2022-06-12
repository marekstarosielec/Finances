import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import * as fs from 'fast-sort';
import * as l from 'lodash';

export interface ListFilterValue {
    selectedValue: string;
}

export interface ListFilterOptions {
    idProperty: string;
    usageIndexPeriodDays?: number;
    usageIndexPeriodDateProperty?: string;
    usageIndexThreshold?: number;
}

@Component({
    selector: 'list-filter',
    templateUrl: 'list-filter.component.html',
    styleUrls: ['./list-filter.component.scss']
})

export class ListFilterComponent implements OnInit, OnDestroy {
    @Input() name: string;
    @Input() filterValue: ListFilterValue;
    
    private _data: any[];
    get data(): any[] {
        return this._data;
    }
    @Input()
    set data(value: any[]) {
        this._data = value;
        this.buildReferenceList();
    }

    @Input() options: ListFilterOptions;

    primaryList: any[];
    secondaryList: any[];
    noFilterElement: string;
    missingElement: string;
    otherElement: string;
    @Output() filterChanged = new EventEmitter<ListFilterValue>()

    constructor() {
    }

    ngOnInit() {
        this.noFilterElement = this.name + '_noFilterElement';
        this.missingElement = this.name + '_missingElement';
        this.otherElement = this.name + '_otherElement';
        this.buildReferenceList();
    }

    ngOnDestroy()
    {

    }

    setFilter(value?: string) : void {
        this.filterValue = { selectedValue: value} as ListFilterValue;
        this.filterChanged.emit(this.filterValue);
    }

    private buildReferenceList() {
        if (!this.data || !this.options || !this.options.idProperty) {
            return;
        }

        if (!this.options.usageIndexPeriodDateProperty || !this.options.usageIndexPeriodDays || !this.options.usageIndexThreshold)
            this.buildPrimaryListOnly();
        else
            this.buildPrimaryAndSecondaryList();
    }

    private buildPrimaryListOnly() {
        const t = l.groupBy(this.data, this.options.idProperty);
        let arr = [];  
        Object.keys(t).map(function(key){  
            arr.push({ id: key });
            return arr;  
        });   
        this.primaryList = fs.sort(arr).by([
            { asc: l => l.id}
        ]);
    }

    private buildPrimaryAndSecondaryList() {
        const usageIndexPeriodStart = new Date(new Date().setDate(new Date().getDate() - this.options.usageIndexPeriodDays));
        const usageFilter = this.data.filter(r => new Date(r[this.options.usageIndexPeriodDateProperty])>=usageIndexPeriodStart )
        const primaryCandidates = l.countBy(usageFilter, this.options.idProperty);
        let primaryCandidatesArray = [];  
        Object.keys(primaryCandidates).map(function(key){  
            primaryCandidatesArray.push({ id: key, usageIndex:primaryCandidates[key]})  
            return primaryCandidatesArray;  
        });   

        this.primaryList = primaryCandidatesArray.filter(l => l.usageIndex >= this.options.usageIndexThreshold);
        this.primaryList = fs.sort(this.primaryList).by([
            { asc: l => l.id}
        ]);

        this.secondaryList = [];
        const secondaryCandidates = l.countBy(this.data, this.options.idProperty);
        let secondaryCandidatesArray = [];  
        Object.keys(secondaryCandidates).map(function(key){  
            secondaryCandidatesArray.push({ id: key, usageIndex:secondaryCandidates[key]})  
            return secondaryCandidatesArray;  
        });  
        this.secondaryList = []
        secondaryCandidatesArray.forEach(element => {
            if (this.primaryList.findIndex(p => p.id === element.id) === -1) {
                this.secondaryList.push({ id: element.id});
            }
        });
        this.secondaryList = fs.sort(this.secondaryList).by([
            { asc: l => l.id}
        ]);
    }
}
