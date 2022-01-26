import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { QueryParamsHandler } from './queryParamsHandler';
import * as fs from 'fast-sort';
import * as l from 'lodash';
import { Subject } from 'rxjs';
import { DataFilteringService } from 'app/services/data-filtering.service';

export interface GridColumn {
    title: string;
    dataProperty: string;
    additionalDataProperty1?: string;
    subDataProperty1?: string;
    filterComponent?: string;
    filterOptions?: any;
    pipe?: string;
    alignment?: string;
    conditionalFormatting?: string;
    noWrap?: boolean;
}

export interface ViewChangedData {
    totalNumberOfRecords: number;
    maximumVisibleNumberOfRecords: number;
    filteredData: any[];
    displayedData: any[];
}

@Component({
    selector: 'grid',
    templateUrl: 'grid.component.html',
    styleUrls: ['./grid.component.scss']
})

export class GridComponent implements OnInit, OnDestroy{
    @Input() public name: string;
    @Input() public columns: GridColumn[];
    @Input() public initialSortColumn: string;
    @Input() public initialSortOrder: number;
    
    private _data: any[];
    get data(): any[] {
        return this._data;
    }
    @Input()
    set data(value: any[]) {
        this._data = value;
        this.prepareView();
    }

    @Input()
    set maximumNumberOfRecordsToShow(value: number) {
        if (this.params.maximumVisibleNumberOfRecords === value)
            return;
        this.params.setMaximumVisibleNumberOfRecords(value);
        this.navigate();
    }

    @Output() public viewChanged = new EventEmitter<ViewChangedData>();
    totalNumberOfRecords: number = 0;

    params: QueryParamsHandler;
    dataSubject = new Subject();

    constructor(
        private router: Router, 
        private route: ActivatedRoute,
        private dataFilteringService: DataFilteringService) {
    }

    ngOnInit() {
        this.route.queryParams.subscribe((qp: Params) => {
            this.params = new QueryParamsHandler(this.name, qp, this.initialSortColumn, this.initialSortOrder);
            this.prepareView();
        });
    }

    ngOnDestroy()
    {
  
    }

    sort(column: GridColumn)
    {
        this.params.setSort(column.dataProperty);
        this.navigate();
    }

    prepareView() {
        if (!this.params) {
            this.params = new QueryParamsHandler(this.name, {}, this.initialSortColumn, this.initialSortOrder);
        }

        if (!this.data) {
            return;
        }

        let data = this.data;

        this.totalNumberOfRecords = data.length;

        data = this.dataFilteringService.applyFilters(data, this.columns, this.params);
        const filteredData = l.clone(data);
        data = this.applySorting(data);
        data = this.applyMaximumVisibleNumberOfRecords(data);
        const displayedData = l.clone(data);
        this.dataSubject.next(data);
        this.viewChanged.emit({ totalNumberOfRecords: this.totalNumberOfRecords, maximumVisibleNumberOfRecords: this.params.maximumVisibleNumberOfRecords, filteredData: filteredData, displayedData: displayedData });
    }

    navigate(){
        this.router.navigate([], { relativeTo: this.route,  
            queryParams: this.params.getQueryParams(), 
            queryParamsHandling: "merge" });
    }

    applySorting(data: any[]) : any[] {
        if (this.params.sortOrder == -1)
            data = fs.sort(data).by([
                { desc: t => t[this.params.sortColumn]},
                { asc: t => t.id}
            ]);
        else
            data = fs.sort(data).by([
                { asc: t => t[this.params.sortColumn]},
                { asc: t => t.id}
            ]);
        
        return data;
    }

    applyMaximumVisibleNumberOfRecords(data: any[]) : any[] {
        if (this.params.maximumVisibleNumberOfRecords && this.params.maximumVisibleNumberOfRecords != 0) {
            data = data.slice(0, this.params.maximumVisibleNumberOfRecords);
        }
        return data;
    }

    selectRecord(id: string) { 
        this.router.navigate([id], { relativeTo: this.route});
    }
}
