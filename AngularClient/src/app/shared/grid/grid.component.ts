import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { QueryParamsHandler } from './queryParamsHandler';
import * as fs from 'fast-sort';
import * as l from 'lodash';
import { Subject } from 'rxjs';
import { DateChange } from '../date-filter/date-filter.component';
import { AmountFilterValue } from '../amount-filter/amount-filter.component';

export interface GridColumn {
    title: string;
    dataProperty: string;
    dataProperty2?: string;
    filterComponent?: string;
    filterComponentData?: any[];
    filterComponentData2?: any;
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
        private route: ActivatedRoute) {
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
        
        data = this.applyFiltering(data);
        const filteredData = l.clone(data);
        data = this.applySorting(data);
        data = this.applyMaximumVisibleNumberOfRecords(data);
        const displayedData = l.clone(data);
        this.dataSubject.next(data);
        this.viewChanged.emit({ totalNumberOfRecords: this.totalNumberOfRecords, maximumVisibleNumberOfRecords: this.params.maximumVisibleNumberOfRecords, filteredData: filteredData, displayedData: displayedData });
    }

    freeTextFilterApply(column: GridColumn, filterValue: string) {
        this.params.setFilter(column.dataProperty, filterValue);
        this.navigate();
    }
    
    dateFilterApply(column: GridColumn, dateChange: DateChange) : void {
        this.params.setDateFilter(column.dataProperty, dateChange.dateFrom, dateChange.dateTo);
        this.navigate();
    }

    listFilterApply(column: GridColumn, element: string) : void {
        this.params.setFilter(column.dataProperty, element);
        this.navigate();
    }

    amountFilterApply(column: GridColumn, value: AmountFilterValue) : void {
        this.params.setFilter(column.dataProperty, value.incomingOutgoing, undefined, 'incomingOutgoing');
        this.params.setFilter(column.dataProperty, value.currency.join('|'), undefined, 'currency');
        this.navigate();
    }

    navigate(){
        this.router.navigate([], { relativeTo: this.route,  
            queryParams: this.params.getQueryParams(), 
            queryParamsHandling: "merge" });
    }

    applyFiltering(data: any[]) : any[] {
        if (!this.params.filters)
            return data;
        this.params.filters.forEach(fd => {
            const gridColumn = this.getColumnFromDataProperty(fd.column);
            if (gridColumn)
            {
                if (gridColumn.filterComponent=="free-text" && fd.filterValue) {
                    data = data.filter(d => d[gridColumn.dataProperty] 
                        && d[gridColumn.dataProperty].toUpperCase().indexOf(fd.filterValue.toUpperCase()) > -1);
                }
                else if (gridColumn.filterComponent=="date" && (fd.filterValue || fd.filterValue2)) {
                    if (fd.filterValue) {
                        const from = new Date(fd.filterValue);
                        data = data.filter(d => new Date(d[gridColumn.dataProperty]) >= from);
                    }
                    if (fd.filterValue2) {
                        const to = new Date(fd.filterValue2);
                        data = data.filter(d => new Date(d[gridColumn.dataProperty]) <= to);
                    }
                }
                else if (gridColumn.filterComponent=="list" && fd.filterValue) {
                    data = data.filter(d =>    
                        (fd.filterValue==='<missing>' && !d[gridColumn.dataProperty])
                        || (!fd.filterValue)
                        || (fd.filterValue == d[gridColumn.dataProperty])    
                    );
                }
                else if (gridColumn.filterComponent=="amount") {
                    data = data.filter(d =>    
                        (fd.filterValue==='incoming' && d[gridColumn.dataProperty] >= 0)
                        || (fd.filterValue==='outgoing' && d[gridColumn.dataProperty] <= 0)
                    );
                }
            }
        });
        return data;
    }

    getFilterValueFromParam(column: GridColumn) : string {
        let result: string = '';
        this.params.filters.forEach(fd => {
            if (fd.column == column.dataProperty) {
                result = fd.filterValue;
                return;
            }
        });
        return result;
    }

    getAmountFilterValueFromParam(column: GridColumn) : AmountFilterValue {
        const incomingOutgoingFilter = this.params.filters.find(f => f.column === column.dataProperty && f.appendix === 'incomingOutgoing')
        return { incomingOutgoing: incomingOutgoingFilter?.filterValue, currency: [] } as AmountFilterValue;
    }

    getDateFilterValueFromParam(column: GridColumn, from: boolean) : Date {
        let result: Date;
        this.params.filters.forEach(fd => {
            if (fd.column == column.dataProperty) {
                const valueToConvert = from ? fd.filterValue : fd.filterValue2;
                if (!valueToConvert)
                    return;
                result = new Date(valueToConvert);
            }
        });
        return result;
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

    getColumnFromDataProperty(columnDataProperty: string) : GridColumn {
        return this.columns.find(cd => cd.dataProperty == columnDataProperty);
    }

    selectRecord(id: string) { 
        this.router.navigate([id], { relativeTo: this.route});
    }
}
