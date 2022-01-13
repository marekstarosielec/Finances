import { Params } from "@angular/router";

export class QueryParamsHandler {
    name: string;
    params: Params;

    sortColumn: string = '';
    sortOrder: number = -1;

    constructor(name: string, params: Params, initialSortColumn: string, initialSortOrder: number) {
        this.name = name;
        this.params = params;
        if (params[this.name+'_sortColumn'])
            this.sortColumn = params[this.name+'_sortColumn'];
        else
            this.sortColumn = initialSortColumn;
        if (params[this.name+'_sortOrder'])
            this.sortOrder = params[this.name+'_sortOrder'];
        else
            this.sortOrder = initialSortOrder
    }

    sort(column: string) {
        if (column === this.sortColumn){
            this.sortOrder = this.sortOrder * (-1);
        } else {
            this.sortColumn = column;
            this.sortOrder = -1;
        }
    }

    getQueryParams() {
        let queryParams = {};
        queryParams[this.name + '_sortColumn'] = this.sortColumn;
        queryParams[this.name + '_sortOrder'] = this.sortOrder; 
        return queryParams;
    }
}