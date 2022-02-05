import { AfterViewInit, Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

export interface TextFilterValue {
    selectedValue: string;
}

export interface TextFilterOptions {
    additionalPropertyToSearch1?: string
    additionalPropertyToSearch2?: string
    additionalPropertyToSearch3?: string
    additionalPropertyToSearch4?: string
}

@Component({
    selector: 'text-filter',
    templateUrl: 'text-filter.component.html',
    styleUrls: ['./text-filter.component.scss']
})

export class TextFilterComponent implements OnInit, OnDestroy, AfterViewInit {
    searchTerm$ = new Subject<string>();
    subscription: Subscription;
    
    @Input() name: string;
    @Input() filterValue: TextFilterValue;
    @Input() data: any[];
    @Input() options: TextFilterOptions;
    @Output() filterChanged = new EventEmitter<TextFilterValue>()
    constructor() {
    }

    ngOnInit() {
        
    }

    ngAfterViewInit(): void {
        if (this.subscription)
            this.subscription.unsubscribe();
        this.subscription = this.searchTerm$.pipe(
            debounceTime(500), 
            distinctUntilChanged()
        ).subscribe((text: string) => {
            this.filterValue.selectedValue = text;
            this.filterChanged.emit(this.filterValue);
        });
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }
}
