import { AfterViewInit, Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
    selector: 'free-text-filter',
    templateUrl: 'free-text-filter.component.html',
    styleUrls: ['./free-text-filter.component.scss']
})

export class FreeTextFilterComponent implements OnInit, OnDestroy, AfterViewInit {
    searchTerm$ = new Subject<string>();
    subscription: Subscription;
    
    @Input() initialValue: string;
    @Output() filterChanged = new EventEmitter<string>()

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
            this.filterChanged.emit(text);
        });
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }
}
