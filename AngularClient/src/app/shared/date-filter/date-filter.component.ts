import { Component, EventEmitter, Input, OnDestroy, OnInit, Output} from '@angular/core';

export interface DateFilterOptions {
}

export interface DateFilterValue {
    dateFrom?: Date;
    dateTo?: Date;
}

@Component({
    selector: 'date-filter',
    templateUrl: 'date-filter.component.html',
    styleUrls: ['./date-filter.component.scss']
})

export class DateFilterComponent implements OnInit, OnDestroy {
    @Input() name: string;
    @Input() data: any[];
    @Input() options: DateFilterOptions;
    
    _filterValue: DateFilterValue;
    @Input()
    set filterValue(value: DateFilterValue) {
        this._filterValue = value;
        this.swapDatesIfNeccessary();
    }

    get filterValue() {
        return this._filterValue;
    }


    @Output() filterChanged = new EventEmitter<DateFilterValue>()

    today: Date;
    beginningOfTime: Date;
    endOfTime: Date;
    firstDayOfCurrentMonth: Date;
    lastDayOfCurrentMonth: Date;
    firstDayOfPreviousMonth: Date;
    lastDayOfPreviousMonth: Date;
    firstDayOfCurrentYear: Date;
    lastDayOfCurrentYear: Date;
    firstDayOfPreviousYear: Date;
    lastDayOfPreviousYear: Date;
    
    constructor() {
    }

    ngOnInit() {
        this.today = new Date();          
        this.beginningOfTime = new Date('1970-01-01');
        this.endOfTime = new Date('2070-01-01');
        this.firstDayOfCurrentMonth = new Date(this.today.getFullYear(), this.today.getMonth(), 1);
        this.lastDayOfCurrentMonth = new Date(this.today.getFullYear(), this.today.getMonth() + 1, 0);
        this.firstDayOfPreviousMonth = new Date(this.today.getFullYear(), this.today.getMonth() - 1, 1);
        this.lastDayOfPreviousMonth = new Date(this.today.getFullYear(), this.today.getMonth(), 0);
        this.firstDayOfCurrentYear = new Date(this.today.getFullYear(), 0, 1);
        this.lastDayOfCurrentYear = new Date(this.today.getFullYear(), 11, 31);
        this.firstDayOfPreviousYear = new Date(this.today.getFullYear() - 1, 0, 1);
        this.lastDayOfPreviousYear = new Date(this.today.getFullYear() - 1, 11, 31);
    }

    ngOnDestroy()
    {

    }

    isNoFilter() : boolean { 
        return (!this.filterValue.dateFrom || this.filterValue.dateFrom === this.beginningOfTime) 
        && (!this.filterValue.dateTo || this.filterValue.dateTo === this.endOfTime);
    }

    isCurrentMonth() : boolean { 
        return this.datesAreEqual(this.filterValue.dateFrom, this.firstDayOfCurrentMonth) && this.datesAreEqual(this.filterValue.dateTo, this.lastDayOfCurrentMonth);
    }

    isPreviousMonth() : boolean { 
        return this.datesAreEqual(this.filterValue.dateFrom, this.firstDayOfPreviousMonth) && this.datesAreEqual(this.filterValue.dateTo, this.lastDayOfPreviousMonth);
    }

    isCurrentYear() : boolean { 
        return this.datesAreEqual(this.filterValue.dateFrom, this.firstDayOfCurrentYear) && this.datesAreEqual(this.filterValue.dateTo, this.lastDayOfCurrentYear);
    }

    isPreviousYear() : boolean { 
        return this.datesAreEqual(this.filterValue.dateFrom, this.firstDayOfPreviousYear) && this.datesAreEqual(this.filterValue.dateTo, this.lastDayOfPreviousYear);
    }

    isCustom() : boolean {
        return !this.isNoFilter() && !this.isCurrentMonth() && !this.isPreviousMonth() && !this.isCurrentYear() && !this.isPreviousYear();
    }

    customFromChange(event: any) : void {
        if (event.year != undefined) {
            this.filterValue.dateFrom = new Date(event.year, event.month-1, event.day);
        } else if (event.srcElement.value) {
            this.filterValue.dateFrom = new Date(event.srcElement.value);
        }  
        this.swapDatesIfNeccessary();
        this.filterApply();
    }

    customToChange(event: any) : void {
        if (event.year != undefined) {
            this.filterValue.dateTo = new Date(event.year, event.month-1, event.day);
        } else if (event.srcElement.value) {
            this.filterValue.dateTo = new Date(event.srcElement.value);
        }  
        this.swapDatesIfNeccessary();
        this.filterApply();
    }

    setDates(dateFrom?: Date, dateTo?: Date) : void {
        this.filterValue.dateFrom = dateFrom;
        this.filterValue.dateTo = dateTo;
        this.filterApply();
    }

    private datesAreEqual(date1?: Date, date2?: Date) : boolean {
        if (date1 == undefined && date2 != undefined) return false;
        if (date1 != undefined && date2 == undefined) return false;
        if (date1 == undefined && date2 == undefined) return true;
        return date1.getFullYear() == date2.getFullYear() && date1.getMonth() == date2.getMonth() && date1.getDate() == date2.getDate();
    }

    private swapDatesIfNeccessary(){
        if (this.filterValue.dateFrom != undefined && this.filterValue.dateTo != undefined && this.filterValue.dateFrom > this.filterValue.dateTo)
        {
            const dateSwap = this.filterValue.dateFrom;
            this.filterValue.dateFrom = this.filterValue.dateTo;
            this.filterValue.dateTo = dateSwap;
        }
    }

    filterApply() {
        this.filterChanged.emit(this.filterValue);
    }
}
