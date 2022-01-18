import { Component, EventEmitter, Input, OnDestroy, OnInit, Output} from '@angular/core';
import * as fs from 'fast-sort';
import * as l from 'lodash';

export interface AmountFilterOptions {
    currencyDataProperty: string;
}

export interface AmountFilterValue {
    incomingOutgoing: string;
    currency: string[];
}

@Component({
    selector: 'amount-filter',
    templateUrl: 'amount-filter.component.html',
    styleUrls: ['./amount-filter.component.scss']
})

export class AmountFilterComponent implements OnInit, OnDestroy {
    @Input() name: string;
    @Input() filterValue: AmountFilterValue;
    @Input() data: any[];
    @Input() options: AmountFilterOptions;

    @Output() filterChanged = new EventEmitter<AmountFilterValue>()

    currencyList: any[];
    
    constructor() {
    }

    ngOnInit() {
        if (this.filterValue) {
            this.filterValue.currency = [];
        }
        this.buildCurrencyReferenceList();
    }

    ngOnDestroy()
    {

    }

    buildCurrencyReferenceList(){
        if (!this.options || !this.options.currencyDataProperty) {
            return;
        }

        const t = l.groupBy(this.data, this.options.currencyDataProperty);
        let arr = [];  
        Object.keys(t).map(function(key){  
            arr.push({ id: key });
            return arr;  
        });   
        this.currencyList = fs.sort(arr).by([
            { asc: l => l.id}
        ]);
    }

    setIncoming(value: boolean) {
        if (!value && !this.filterValue.incomingOutgoing) {
            this.filterValue.incomingOutgoing = 'outgoing';
        } else {
            this.filterValue.incomingOutgoing = undefined;
        }
        this.emitEvent();
    }

    setOutgoing(value: boolean) {
        if (!value && !this.filterValue.incomingOutgoing) {
            this.filterValue.incomingOutgoing = 'incoming';
        } else {
            this.filterValue.incomingOutgoing = undefined;
        }
        this.emitEvent();
    }

    setCurrency(currency: string, checked: boolean) {
        // const pos = this.filterValue.currency.findIndex(c => c === currency);
        // if (pos > -1 && !checked) {
        //     this.filterValue.currency.splice(pos, 1);
        // } else if (pos === -1 && checked){
        //     this.filterValue.currency.push(currency);
        // }
        // console.log('currencies', this.filterValue.currency);
        // this.emitEvent();
    }

    isCurrencySelected(currency: string): boolean {
        return this.filterValue.currency.length === 0 || this.filterValue.currency.findIndex(c => c === currency)>-1;
    }

    private emitEvent() {
        this.filterChanged.emit(this.filterValue);
    }
}
