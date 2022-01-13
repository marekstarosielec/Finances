import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { QueryParamsHandler } from './queryParamsHandler';
import * as fs from 'fast-sort';
import { BehaviorSubject } from 'rxjs';

export interface GridColumn {
    title: string;
    dataProperty: string;
}

@Component({
    selector: 'grid',
    templateUrl: 'grid.component.html',
    styleUrls: ['./grid.component.scss']
})

export class GridComponent implements OnInit, OnDestroy{
    @Input() public name: string;
    @Input() public columns: GridColumn[];
    @Output() public rowClicked = new EventEmitter<any>();
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
    
    params: QueryParamsHandler;
    dataSubject = new BehaviorSubject([]);

    constructor(
        private router: Router, 
        private route: ActivatedRoute) {
    }

    ngOnInit() {
        this.route.queryParams.subscribe((qp: Params) => {
            this.params = new QueryParamsHandler(this.name, qp, this.initialSortColumn, this.initialSortOrder);
            // this.maximumVisibleNumberOfRecords = qp.limit ?? 100;
            // if (this.maximumVisibleNumberOfRecords < 0) this.maximumVisibleNumberOfRecords = 100;
            // this.accountFilter = qp.account ? decodeURIComponent(qp.account) : "";
            // this.categoryFilter = qp.category ? decodeURIComponent(qp.category) : "";
            // this.descriptionFilter = qp.description ? decodeURIComponent(qp.description) : "";
            //this.sortColumn = qp[] ?? 'date';
            // this.sortOrder = qp.sortOrder ?? -1;
            // if (qp.from)
            //     this.dateFromFilter = new Date(qp.from);
            // else
            //     this.dateFromFilter = undefined;
            
            // if (qp.to)
            //     this.dateToFilter = new Date(qp.to);
            // else
            //     this.dateToFilter = undefined;
            this.prepareView();
        });
    }

    ngOnDestroy()
    {
  
    }

    sort(column: GridColumn)
    {
        this.params.sort(column.dataProperty);
        this.router.navigate([], { relativeTo: this.route,  
            queryParams: this.params.getQueryParams(), 
            queryParamsHandling: "merge" });
    }

    prepareView() {
        if (!this.params)
            this.params = new QueryParamsHandler(this.name, {}, this.initialSortColumn, this.initialSortOrder);
        
        // if (!this.data)
        // {
        //     this.currentNumberOfRecords = 0;
        //     this.filteredNumberOfRecords = 0;
        //     return;
        // }

        let data = this.data;
        // if (this.accountFilter !== '') {
        //     data = data.filter(d => d.account === this.accountFilter);
        // }
        // if (this.categoryFilter !== '') {
        //     data = data.filter(d => d.category === this.categoryFilter || (this.categoryFilter === 'missing' && !!!d.category));
        // }
        // if (this.descriptionFilter !== '') {
        //     data = data.filter(d => d.bankInfo?.toUpperCase().indexOf(this.descriptionFilter.toUpperCase()) > -1
        //     || d.comment?.toUpperCase().indexOf(this.descriptionFilter.toUpperCase()) > -1);
        // }
        // if (this.dateFromFilter != undefined){
        //     data = data.filter(d => new Date(d.date) >= this.dateFromFilter);    
        //     console.log('from', this.dateFromFilter);        
        // }
        // if (this.dateToFilter != undefined){
        //     data = data.filter(d => new Date(d.date) <= this.dateToFilter);            
        //     console.log('to', this.dateToFilter);
        // }

        // this.filteredNumberOfRecords = data.length;
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
        
        // this.totalAmounts =  l(data)
        //     .groupBy('currency')
        //     .map((objs, key) => ({
        //         'currency': key,
        //         'amount': l.sumBy(objs, 'amount'),
        //         'incoming': l.sumBy(objs.filter(o => o.amount > 0), 'amount'),
        //         'outgoing': l.sumBy(objs.filter(o => o.amount < 0), 'amount') }))
        //     .value();
        // if (this.maximumVisibleNumberOfRecords && this.maximumVisibleNumberOfRecords != 0) {
        //     data = data.slice(0, this.maximumVisibleNumberOfRecords);
        // }
        // this.currentNumberOfRecords = data.length;
       this.dataSubject.next(data);
    }
}
