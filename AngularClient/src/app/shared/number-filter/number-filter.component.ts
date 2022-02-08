import { AfterViewInit, Component, EventEmitter, Input, OnDestroy, OnInit, Output} from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';

export interface NumberFilterOptions {
}

export interface NumberFilterValue {
    from: number;
    to: number;
}

@Component({
    selector: 'number-filter',
    templateUrl: 'number-filter.component.html',
    styleUrls: ['./number-filter.component.scss']
})

export class NumberFilterComponent implements OnInit, OnDestroy, AfterViewInit {
    @Input() name: string;
    @Input() filterValue: NumberFilterValue;
    @Input() data: any[];
    @Input() options: NumberFilterOptions;

    from$ = new Subject<Number>();
    to$ = new Subject<Number>();
    private ngUnsubscribe = new Subject();

    @Output() filterChanged = new EventEmitter<NumberFilterValue>()

    currencyList: any[];
    
    constructor() {
    }

    ngOnInit() {

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

    private emitEvent() {
        this.filterChanged.emit(this.filterValue);
    }
}
