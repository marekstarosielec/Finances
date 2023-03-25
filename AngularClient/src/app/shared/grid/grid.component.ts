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
    subDataProperty2?: string;
    subDataProperty3?: string;
    subDataProperty4?: string;
    subDataProperty5?: string;
    component?: string;
    filterOptions?: any;
    pipe?: string;
    alignment?: string;
    image?: string;
    imageSet?: string[];
    conditionalFormatting?: string;
    skipConditionalFormattingProperty?: string;
    noWrap?: boolean;
    customEvent?: boolean;
}

export interface ViewChangedData {
    totalNumberOfRecords: number;
    maximumVisibleNumberOfRecords: number;
    filteredData: any[];
    displayedData: any[];
}

export interface RowClickedData {
    row: any[];
    column: GridColumn;
}

export interface RowCheckedData {
    rows: any[];
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
    @Input() public showCheckboxes: boolean;
    public checkedRows: any[] = [];

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
    @Output() public rowClicked = new EventEmitter<RowClickedData>();
    @Output() public rowChecked = new EventEmitter<RowCheckedData>();
    totalNumberOfRecords: number = 0;

    params: QueryParamsHandler;

    dataSubject = new Subject();

    constructor(
        private router: Router, 
        private route: ActivatedRoute,
        private dataFilteringService: DataFilteringService) {
    }

    ngOnInit() {
        window.addEventListener('scroll', this.scrollEvent, true);
        this.route.queryParams.subscribe((qp: Params) => {
            this.params = new QueryParamsHandler(this.name, qp, this.initialSortColumn, this.initialSortOrder);
            this.prepareView();
        });
    }

    ngOnDestroy()
    {
        window.removeEventListener('scroll', this.scrollEvent, true);
    }

    scrollEvent = (event): void => {
        if (!this.data) {
            return;
        }
        let v = event.target.scrollingElement.scrollTop;
        //Some strange offset in order to find the same position.
        if (v > 70)
            v=v-70;
        this.params.setTop(v);
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

        if (this.params.top > 0){
            setTimeout(() => {
                window.scrollBy({top: this.params.top});
            });
        }

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

    selectRecord(row: any, column: GridColumn) { 
        this.navigate();
        setTimeout(() => {
            if (column.customEvent) {
                this.rowClicked.emit({ row: row, column: column} as RowClickedData)
            } else {
                this.router.navigate([row.id], { relativeTo: this.route});
            }
        });
    }

    selectCheckbox(row: any) { 
        const i = this.checkedRows.findIndex(r => r.fullFileName === row.fullFileName);
        if (i > -1)
            this.checkedRows.splice(i, 1);
        else
            this.checkedRows.push(row);
        this.rowChecked.emit({ rows: this.checkedRows });
    }

    getImageSet(column: GridColumn, row: any) {
        const dataPropertyValue = row[column.dataProperty];
        if (!dataPropertyValue)
            return;
        const image = column.imageSet[dataPropertyValue];
        return 'nc-icon ' + image;
    }
}
