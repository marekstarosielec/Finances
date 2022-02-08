import { Injectable } from '@angular/core';
import { AmountFilterOptions, AmountFilterValue } from 'app/shared/amount-filter/amount-filter.component';
import { DateFilterValue } from 'app/shared/date-filter/date-filter.component';
import { GridColumn } from 'app/shared/grid/grid.component';
import { QueryParamsHandler } from 'app/shared/grid/queryParamsHandler';
import { ListFilterValue } from 'app/shared/list-filter/list-filter.component';
import { NumberFilterValue } from 'app/shared/number-filter/number-filter.component';
import { TextFilterOptions, TextFilterValue } from 'app/shared/text-filter/text-filter.component';

@Injectable({
  providedIn: 'root'
})
export class DataFilteringService {
  public AmountFilterIdentifier : string = 'amount';
  public DateFilterIdentifier : string = 'date';
  public ListFilterIdentifier : string = 'list';
  public TextFilterIdentifier : string = 'text';
  public NumberFilterIdentifier : string = 'number';
  
  public getFilter(column: GridColumn, params: QueryParamsHandler) {
    if (column.component === this.AmountFilterIdentifier) {
      return this.getAmountFilter(column, params);
    }
    if (column.component === this.DateFilterIdentifier) {
      return this.getDateFilter(column, params);
    }    
    if (column.component === this.ListFilterIdentifier) {
      return this.getListFilter(column, params);
    }
    if (column.component === this.TextFilterIdentifier) {
      return this.getTextFilter(column, params);
    }
    if (column.component === this.NumberFilterIdentifier) {
      return this.getNumberFilter(column, params);
    }
  }

  public addFilter(column: GridColumn, params: QueryParamsHandler, value: any) {
     if (column.component === this.AmountFilterIdentifier) {
      return this.addAmountFilter(column, params, value);
    }    
    if (column.component === this.DateFilterIdentifier) {
      return this.addDateFilter(column, params, value);
    }
    if (column.component === this.ListFilterIdentifier) {
      return this.addListFilter(column, params, value);
    }
    if (column.component === this.TextFilterIdentifier) {
      return this.addTextFilter(column, params, value);
    }
    if (column.component === this.NumberFilterIdentifier) {
      return this.addNumberFilter(column, params, value);
    }
  }

  public applyFilters(data: any[], columns: GridColumn[], params: QueryParamsHandler) : any[] {
    columns.forEach(column => {
      if (column.component === this.AmountFilterIdentifier) {
        data = this.applyAmountFilter(data, column, params);
      }
      if (column.component === this.DateFilterIdentifier) {
        data = this.applyDateFilter(data, column, params);
      }
      if (column.component === this.ListFilterIdentifier) {
        data = this.applyListFilter(data, column, params);
      }
      if (column.component === this.TextFilterIdentifier) {
        data = this.applyTextFilter(data, column, params);
      }
      if (column.component === this.NumberFilterIdentifier) {
        data = this.applyNumberFilter(data, column, params);
      }
    });
    return data;
  }

  private getAmountFilter(column: GridColumn, params: QueryParamsHandler) : AmountFilterValue {
    const direction = params.filters.find(f => f.column === column.dataProperty && f.appendix === 'direction')?.filterValue?.split('|');
    const currency = params.filters.find(f => f.column === column.dataProperty && f.appendix === 'currency')?.filterValue?.split('|');
    const from = params.filters.find(f => f.column === column.dataProperty && f.appendix === 'from')?.filterValue;
    const to = params.filters.find(f => f.column === column.dataProperty && f.appendix === 'to')?.filterValue;
    return { direction: direction, currency: currency, from: from ? Number(from) : undefined, to: to ? Number(to) : undefined } as AmountFilterValue;
  }

  private addAmountFilter(column: GridColumn, params: QueryParamsHandler, value: AmountFilterValue) { 
    params.setFilter(column.dataProperty, value.direction.join('|'), 'direction');
    params.setFilter(column.dataProperty, value.currency.join('|'), 'currency');
    params.setFilter(column.dataProperty, value.from ? value.from.toString() : "-99999999", 'from');
    params.setFilter(column.dataProperty, value.to?.toString() ?? "99999999", 'to');
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
      console.log(value.currency);
      data = data.filter(d => value.currency.indexOf(d[options.currencyDataProperty] ?? 'brak') > -1);
    }
    if (value.from && value.from>-99999999) {
      data = data.filter(d => Math.abs(d[column.dataProperty]) > Math.abs(value.from) || (Math.abs(d[column.dataProperty]) * (-1)) < (Math.abs(value.from) * (-1)));
    }
    if (value.to && value.to<99999999) {
      data = data.filter(d => Math.abs(d[column.dataProperty]) < Math.abs(value.to) || (Math.abs(d[column.dataProperty]) * (-1)) > (Math.abs(value.to) * (-1)));
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

  private getTextFilter(column: GridColumn, params: QueryParamsHandler) : TextFilterValue {
    const selectedValue = params.filters.find(f => f.column === column.dataProperty)?.filterValue;
    return { selectedValue: !selectedValue || selectedValue === '<noFilter>' ? '' : selectedValue } as TextFilterValue;
  }

  private addTextFilter(column: GridColumn, params: QueryParamsHandler, value: TextFilterValue) { 
    params.setFilter(column.dataProperty, !value.selectedValue || value.selectedValue==='' ? '<noFilter>' : value.selectedValue);
  }

  private applyTextFilter(data: any[], column: GridColumn, params: QueryParamsHandler) : any[] {
    const value = this.getTextFilter(column, params) as ListFilterValue;
    const options = column.filterOptions as TextFilterOptions;
    if (!value || !value.selectedValue || value.selectedValue === '<noFilter>') {
      return data;
    }
    const search = value.selectedValue.toUpperCase();
    
    data = data.filter(d => 
      d[column.dataProperty]?.toString().toUpperCase().indexOf(search) > -1
      || (options?.additionalPropertyToSearch1 && d[options.additionalPropertyToSearch1]?.toString().toUpperCase().indexOf(search) > -1)
      || (options?.additionalPropertyToSearch2 && d[options.additionalPropertyToSearch2]?.toString().toUpperCase().indexOf(search) > -1)
      || (options?.additionalPropertyToSearch3 && d[options.additionalPropertyToSearch3]?.toString().toUpperCase().indexOf(search) > -1)
      || (options?.additionalPropertyToSearch4 && d[options.additionalPropertyToSearch4]?.toString().toUpperCase().indexOf(search) > -1)
    );
    return data;
  }

  private getNumberFilter(column: GridColumn, params: QueryParamsHandler) : NumberFilterValue {
    const from = params.filters.find(f => f.column === column.dataProperty && f.appendix === 'from')?.filterValue;
    const to = params.filters.find(f => f.column === column.dataProperty && f.appendix === 'to')?.filterValue;
    return { from: from ? from: -99999999, to: to ? to : 99999999 } as NumberFilterValue;
  }

  private addNumberFilter(column: GridColumn, params: QueryParamsHandler, value: NumberFilterValue) { 
    params.setFilter(column.dataProperty, value.from?.toString(), 'from');
    params.setFilter(column.dataProperty, value.to?.toString(), 'to');
  }

  private applyNumberFilter(data: any[], column: GridColumn, params: QueryParamsHandler) : any[] {
    const value = this.getNumberFilter(column, params) as NumberFilterValue;    
    if (!value || !column.dataProperty) {
      return data;
    }
    if (value.from) {
      data = data.filter(d => d[column.dataProperty] >= value.from);
    }
    if (value.to) {
      data = data.filter(d => d[column.dataProperty] <= value.to);
    }
    return data;
  }
}
