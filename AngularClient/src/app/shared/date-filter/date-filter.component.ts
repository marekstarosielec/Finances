import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';

export interface DateChange {
    dateFrom?: Date;
    dateTo?: Date;
}

@Component({
    selector: 'date-filter',
    templateUrl: 'date-filter.component.html',
    styleUrls: ['./date-filter.component.scss']
})

export class DateFilterComponent implements OnInit, OnDestroy {

    _dateFrom: Date;
    @Input()
    set dateFrom(value: Date) {
        this._dateFrom = value;
        this.swapDateIfNeccessary();
    }

    get dateFrom() {
        return this._dateFrom;
    }

    _dateTo: Date;
    @Input()
    set dateTo(value: Date) {
        this._dateTo = value;
        this.swapDateIfNeccessary();
    }

    get dateTo() {
        return this._dateTo;
    }

    @Output() dateChanged = new EventEmitter<DateChange>()


    today: Date;
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
        return this.dateFrom == undefined && this.dateTo == undefined;
    }

    isCurrentMonth() : boolean { 
        return this.datesAreEqual(this.dateFrom, this.firstDayOfCurrentMonth) && this.datesAreEqual(this.dateTo, this.lastDayOfCurrentMonth);
    }

    isPreviousMonth() : boolean { 
        return this.datesAreEqual(this.dateFrom, this.firstDayOfPreviousMonth) && this.datesAreEqual(this.dateTo, this.lastDayOfPreviousMonth);
    }

    isCurrentYear() : boolean { 
        return this.datesAreEqual(this.dateFrom, this.firstDayOfCurrentYear) && this.datesAreEqual(this.dateTo, this.lastDayOfCurrentYear);
    }

    isPreviousYear() : boolean { 
        return this.datesAreEqual(this.dateFrom, this.firstDayOfPreviousYear) && this.datesAreEqual(this.dateTo, this.lastDayOfPreviousYear);
    }

    isCustom() : boolean {
        return !this.isNoFilter() && !this.isCurrentMonth() && !this.isPreviousMonth() && !this.isCurrentYear() && !this.isPreviousYear();
    }

    customFromChange(event: any) : void {
        if (event.year != undefined) {
            this.dateFrom = new Date(event.year, event.month-1, event.day);
        } else if (event.srcElement.value) {
            this.dateFrom = new Date(event.srcElement.value);
        }  
        this.swapDateIfNeccessary();
        this.dateChanged.emit({ dateFrom: this.dateFrom, dateTo : this.dateTo });
    }

    customToChange(event: any) : void {
        if (event.year != undefined) {
            this.dateTo = new Date(event.year, event.month-1, event.day);
        } else if (event.srcElement.value) {
            this.dateTo = new Date(event.srcElement.value);
        }  
        this.swapDateIfNeccessary();
        this.dateChanged.emit({ dateFrom: this.dateFrom, dateTo : this.dateTo });
    }

    private setDates(dateFrom?: Date, dateTo?: Date) : void {
        this.dateFrom = dateFrom;
        this.dateTo = dateTo;
        this.dateChanged.emit({ dateFrom: this.dateFrom, dateTo : this.dateTo });
    }

    private datesAreEqual(date1?: Date, date2?: Date) : boolean {
        if (date1 == undefined && date2 != undefined) return false;
        if (date1 != undefined && date2 == undefined) return false;
        if (date1 == undefined && date2 == undefined) return true;
        return date1.getFullYear() == date2.getFullYear() && date1.getMonth() == date2.getMonth() && date1.getDate() == date2.getDate();
    }

    private swapDateIfNeccessary(){
        if (this.dateFrom != undefined && this.dateTo != undefined && this.dateFrom > this.dateTo)
        {
            const dateSwap = this.dateFrom;
            this.dateFrom = this.dateTo;
            this.dateTo = dateSwap;
        }
    }
}
