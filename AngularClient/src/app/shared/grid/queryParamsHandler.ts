import { Params } from "@angular/router";

interface FilterDefinition {
    column: string;
    filterValue: string;
    filterValue2?: string;
}

export class QueryParamsHandler {
    name: string;
    params: Params;

    sortColumn: string = '';
    sortOrder: number = -1;
    filters: FilterDefinition[] = [];
    maximumVisibleNumberOfRecords: number = 100;

    constructor(name: string, params: Params, initialSortColumn: string, initialSortOrder: number) {
        this.name = name;
        this.params = params;
        this.readSort(params, initialSortColumn, initialSortOrder);
        this.readFilter(params);
        this.readMaximumVisibleNumberOfRecords(params);
    }

    readSort(params: Params, initialSortColumn: string, initialSortOrder: number) {
        this.sortColumn = params[this.name+'_sortColumn'] ? params[this.name+'_sortColumn'] : initialSortColumn;
        this.sortOrder = params[this.name+'_sortOrder'] ? params[this.name+'_sortOrder'] : initialSortOrder;
    }

    readFilter(params: Params) {
        const allProperties = Object.keys(params);
        allProperties.forEach(property => {
            if (property.startsWith(this.name+'_filter_')){
                this.setFilter(property.replace(this.name+'_filter_', ''), params[property], params[property.replace('_filter_', '_filter2_')]);
            }
        });
    }
    
    readMaximumVisibleNumberOfRecords(params: Params) {
        this.maximumVisibleNumberOfRecords = params[this.name+'_maximumVisibleNumberOfRecords'] ? params[this.name+'_maximumVisibleNumberOfRecords'] : 100;
    }

    setSort(column: string) {
        if (column === this.sortColumn){
            this.sortOrder = this.sortOrder * (-1);
        } else {
            this.sortColumn = column;
            this.sortOrder = -1;
        }
    }

    setFilter(column: string, filterValue: string, filterValue2?: string){
        const filterDefintion: FilterDefinition = { column: column, filterValue: filterValue, filterValue2: filterValue2 };
        let existing = this.filters.findIndex(fd => fd.column === column);
        if (existing > -1)
            this.filters[existing] = filterDefintion;
        else
            this.filters.push(filterDefintion);
    }

    setDateFilter(column: string, dateFrom?: Date, dateTo?: Date){
        let from: string;
        let to: string;
        if (dateFrom != undefined)
            from = dateFrom.getFullYear() + '-' + (dateFrom.getMonth()+1) + '-' + dateFrom.getDate();
        if (dateTo != undefined)
            to = dateTo.getFullYear() + '-' + (dateTo.getMonth()+1) + '-' + dateTo.getDate();

        const filterDefintion: FilterDefinition = { column: column, filterValue: from, filterValue2: to };
        let existing = this.filters.findIndex(fd => fd.column === column);
        if (existing > -1)
            this.filters[existing] = filterDefintion;
        else
            this.filters.push(filterDefintion);
    }

    setMaximumVisibleNumberOfRecords(maximumVisibleNumberOfRecords: number) {
        this.maximumVisibleNumberOfRecords = maximumVisibleNumberOfRecords;
    }

    getQueryParams() {
        let queryParams = {};
        queryParams[this.name + '_sortColumn'] = this.sortColumn;
        queryParams[this.name + '_sortOrder'] = this.sortOrder; 
        if (this.filters) {
            this.filters.forEach(fd => {
                queryParams[this.name + '_filter_' + fd.column] = fd.filterValue;
                if (fd.filterValue2)
                    queryParams[this.name + '_filter2_' + fd.column] = fd.filterValue2;
            });
        }
        queryParams[this.name + '_maximumVisibleNumberOfRecords'] = this.maximumVisibleNumberOfRecords;
        return queryParams;
    }
}