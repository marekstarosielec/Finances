import { Component, OnInit } from '@angular/core';
import { DataService } from '../../services/data.service';

declare interface TableData {
    headerRow: string[];
    dataRows: string[][];
}

@Component({
    selector: 'table-cmp',
    moduleId: module.id,
    templateUrl: 'transactions.component.html'
})
export class TransactionsComponent implements OnInit{
    data: any[];
    loading: boolean;

    constructor (private dataService: DataService) {}

    ngOnInit(){
        this.loading = true;
        this.dataService.sendGetRequest().subscribe((data: any[])=>{
            this.data = data;
            this.loading = false;
        }) 

    }
}
