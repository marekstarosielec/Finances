import { AfterViewInit, Component, EventEmitter, Input, OnDestroy, OnInit, Output} from '@angular/core';
import * as fs from 'fast-sort';
import * as l from 'lodash';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';

export interface AmountFilterOptions {
    currencyDataProperty: string;
}

export interface AmountFilterValue {
    direction: string[];
    currency: string[];
    from: number;
    to: number;
}

@Component({
    selector: 'amount-filter',
    templateUrl: 'amount-filter.component.html',
    styleUrls: ['./amount-filter.component.scss']
})

export class AmountFilterComponent implements OnInit, OnDestroy, AfterViewInit {
    @Input() name: string;
    @Input() filterValue: AmountFilterValue;
    @Input() data: any[];
    @Input() options: AmountFilterOptions;

    from$ = new Subject<Number>();
    to$ = new Subject<Number>();
    private ngUnsubscribe = new Subject();

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
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }

    ngAfterViewInit(): void {
        this.from$.pipe(
            debounceTime(500), 
            distinctUntilChanged(),
            takeUntil(this.ngUnsubscribe)
        ).subscribe((value: number) => {
            this.filterValue.from = value;
            this.emitEvent();
        });
        this.to$.pipe(
            debounceTime(500), 
            distinctUntilChanged(),
            takeUntil(this.ngUnsubscribe)         
        ).subscribe((value: number) => {
            this.filterValue.to = value;
            this.emitEvent();
        });
    }

    buildCurrencyReferenceList(){
        if (!this.options || !this.options.currencyDataProperty) {
            return;
        }

        const t = l.groupBy(this.data, this.options.currencyDataProperty);
        let arr = [];  
        Object.keys(t).map(function(key){  
            arr.push({ id: key =='undefined' ? 'brak' : key});
            return arr;  
        });  

        this.currencyList = fs.sort(arr).by([
            { asc: l => l.id}
        ]);
    }

    setIn(value: boolean) {
        this.setDefaultDirections();
            
        if (!value) {
            const pos = this.filterValue.direction.findIndex(d => d === 'in');
            if (pos > -1) {
                this.filterValue.direction.splice(pos,1);
            }
        } else {
            const pos = this.filterValue.direction.findIndex(d => d === 'in');
            if (pos === -1) {
                this.filterValue.direction.push('in');
            }
        }
        this.emitEvent();
    }

    setOut(value: boolean) {
        this.setDefaultDirections();

        if (!value) {
            const pos = this.filterValue.direction.findIndex(d => d === 'out');
            if (pos > -1) {
                this.filterValue.direction.splice(pos,1);
            }
        } else {
            const pos = this.filterValue.direction.findIndex(d => d === 'out');
            if (pos === -1) {
                this.filterValue.direction.push('out');
            }
        }
        this.emitEvent();
    }

    
    isDirectionSelected(direction: string) {
        this.setDefaultDirections();
        return this.filterValue.direction.findIndex(d => d === direction) > -1;
    }
    

    private setDefaultDirections() {
        if (!this.filterValue.direction) {
            this.filterValue.direction = ['in', 'out'];
        }
    }

    isCurrencySelected(currency: string): boolean {
        this.setDefaultCurrencies();
        return this.filterValue.currency.findIndex(c => c === currency) > -1;
    }

    setCurrency(currency: string, checked: boolean) {
        this.setDefaultCurrencies();
        if (checked && this.filterValue.currency.indexOf(currency) === -1){
            this.filterValue.currency.push(currency);
        } else {
            const pos = this.filterValue.currency.indexOf(currency);
            this.filterValue.currency.splice(pos,1);
        }
        this.emitEvent();
    }

    private setDefaultCurrencies() {
        if (!this.filterValue.currency) {
            this.filterValue.currency = [];
            this.currencyList.forEach(currency => this.filterValue.currency.push(currency.id));
        }
    }

    private emitEvent() {
        this.filterChanged.emit(this.filterValue);
    }
}
