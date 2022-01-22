import { Injectable } from '@angular/core';
import { AmountFilterOptions, AmountFilterValue } from 'app/shared/amount-filter/amount-filter.component';
import { DateFilterValue } from 'app/shared/date-filter/date-filter.component';
import { GridColumn } from 'app/shared/grid/grid.component';
import { QueryParamsHandler } from 'app/shared/grid/queryParamsHandler';
import { ListFilterValue } from 'app/shared/list-filter/list-filter.component';

@Injectable({
  providedIn: 'root'
})
export class DataFilteringService {
  public AmountFilterIdentifier : string = 'amount';
  public DateFilterIdentifier : string = 'date';
  public ListFilterIdentifier : string = 'list';
  
  public getFilter(column: GridColumn, params: QueryParamsHandler) {
    if (column.filterComponent === this.AmountFilterIdentifier) {
      return this.getAmountFilter(column, params);
    }
    if (column.filterComponent === this.DateFilterIdentifier) {
      return this.getDateFilter(column, params);
    }    
    if (column.filterComponent === this.ListFilterIdentifier) {
      return this.getListFilter(column, params);
    }
  }

  public addFilter(column: GridColumn, params: QueryParamsHandler, value: any) {
    if (column.filterComponent === this.AmountFilterIdentifier) {
      return this.addAmountFilter(column, params, value);
    }    
    if (column.filterComponent === this.DateFilterIdentifier) {
      return this.addDateFilter(column, params, value);
    }
    if (column.filterComponent === this.ListFilterIdentifier) {
      return this.addListFilter(column, params, value);
    }
  }

  public applyFilters(data: any[], columns: GridColumn[], params: QueryParamsHandler) : any[] {
    columns.forEach(column => {
      if (column.filterComponent === this.AmountFilterIdentifier) {
        data = this.applyAmountFilter(data, column, params);
      }
      if (column.filterComponent === this.DateFilterIdentifier) {
        data = this.applyDateFilter(data, column, params);
      }
      if (column.filterComponent === this.ListFilterIdentifier) {
        data = this.applyListFilter(data, column, params);
      }
    });
    return data;
  }

  private getAmountFilter(column: GridColumn, params: QueryParamsHandler) : AmountFilterValue {
    const direction = params.filters.find(f => f.column === column.dataProperty && f.appendix === 'direction')?.filterValue?.split('|');
    const currency = params.filters.find(f => f.column === column.dataProperty && f.appendix === 'currency')?.filterValue?.split('|');
    return { direction: direction, currency: currency } as AmountFilterValue;
  }

  private addAmountFilter(column: GridColumn, params: QueryParamsHandler, value: AmountFilterValue) { 
    params.setFilter(column.dataProperty, value.direction.join('|'), 'direction');
    params.setFilter(column.dataProperty, value.currency.join('|'), 'currency');
  }

  private applyAmountFilter(data: any[], column: GridColumn, params: QueryParamsHandler) : any[] {
    const value = this.getAmountFilter(column, params) as AmountFilterValue;
    const options = column.filterOptions as AmountFilterOptions;
    if (!value || !column.dataProperty) {
      return data;
    }
    if (value.direction && value.direction.indexOf('in') === -1){
      data = data.filter(d => d[column.dataProperty] <= 0);
    }
    if (value.direction && value.direction.indexOf('out') === -1){
      data = data.filter(d => d[column.dataProperty] >= 0);
    }
    if (value.currency && options.currencyDataProperty) {
      data = data.filter(c => value.currency.indexOf(c[options.currencyDataProperty]) > -1);
    }
    return data;
  }

  private getDateFilter(column: GridColumn, params: QueryParamsHandler) : DateFilterValue {
    const from = params.filters.find(f => f.column === column.dataProperty && f.appendix === 'from')?.filterValue;
    const to = params.filters.find(f => f.column === column.dataProperty && f.appendix === 'to')?.filterValue;
    return { dateFrom: from ? new Date(from): undefined, dateTo: to ? new Date(to) : undefined } as DateFilterValue;
  }

  private addDateFilter(column: GridColumn, params: QueryParamsHandler, value: DateFilterValue) { 
    params.setFilter(column.dataProperty, value.dateFrom ? value.dateFrom.getFullYear() + '-' + (value.dateFrom.getMonth()+1) + '-' + value.dateFrom.getDate() : undefined, 'from');
    params.setFilter(column.dataProperty, value.dateTo ? value.dateTo.getFullYear() + '-' + (value.dateTo.getMonth()+1) + '-' + value.dateTo.getDate() : undefined, 'to');
  }

  private applyDateFilter(data: any[], column: GridColumn, params: QueryParamsHandler) : any[] {
    const value = this.getDateFilter(column, params) as DateFilterValue;    
    if (!value || !column.dataProperty) {
      return data;
    }
    if (value.dateFrom) {
      data = data.filter(d => new Date(d[column.dataProperty]) >= value.dateFrom);
    }
    if (value.dateTo) {
      data = data.filter(d => new Date(d[column.dataProperty]) <= value.dateTo);
    }
    return data;
  }

  private getListFilter(column: GridColumn, params: QueryParamsHandler) : ListFilterValue {
    const selectedValue = params.filters.find(f => f.column === column.dataProperty)?.filterValue;
    return { selectedValue: selectedValue } as ListFilterValue;
  }

  private addListFilter(column: GridColumn, params: QueryParamsHandler, value: ListFilterValue) { 
    params.setFilter(column.dataProperty, value.selectedValue);
  }

  private applyListFilter(data: any[], column: GridColumn, params: QueryParamsHandler) : any[] {
    const value = this.getListFilter(column, params) as ListFilterValue;
    if (!value || !column.dataProperty) {
      return data;
    }
    if (value.selectedValue && value.selectedValue !=='<all>'){
      data = data.filter(d => 
        (value.selectedValue ==='<missing>' && !d[column.dataProperty])
        || (value.selectedValue === d[column.dataProperty]));
    }
    return data;
  }
}
