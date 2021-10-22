import { Component, OnInit } from '@angular/core';
import { DataService } from '../../services/data.service';

declare interface TableData {
    headerRow: string[];
    dataRows: string[][];
}

@Component({
    selector: 'table-cmp',
    moduleId: module.id,
    templateUrl: 'table.component.html'
})

export class TableComponent implements OnInit{
    data: any[];

    constructor (private dataService: DataService) {}

    ngOnInit(){
        this.dataService.sendGetRequest().subscribe((data: any[])=>{
            this.data = data;
          }) 

    }
}
