import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';

@Component({
    selector: 'list-filter',
    templateUrl: 'list-filter.component.html',
    styleUrls: ['./list-filter.component.scss']
})

export class ListFilterComponent implements OnInit, OnDestroy {
    @Input() name: string;
    @Input() filterValue: string;
    @Input() list: any;

    @Output() filterChanged = new EventEmitter<string>()

    constructor() {
    }

    ngOnInit() {
        
    }

    ngOnDestroy()
    {

    }

    private setFilter(value?: string) : void {
        this.filterValue = value;
        this.filterChanged.emit(this.filterValue);
    }
}
