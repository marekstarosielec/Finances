import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';

export interface GridColumn {
    title: string;
    dataProperty: string;
}

@Component({
    selector: 'grid',
    templateUrl: 'grid.component.html',
    styleUrls: ['./grid.component.scss']
})

export class GridComponent implements OnInit, OnDestroy{
    @Input() public columns: GridColumn[];
    @Input() public data: any[];
    @Output() public rowClicked = new EventEmitter<any>();

    constructor() {
    }

    ngOnInit() {

    }

    ngOnDestroy()
    {
  
    }
}
