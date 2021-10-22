import { Component, OnInit } from '@angular/core';
import { TransactionsService } from '../../services/transactions.service';

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

    constructor (private transactionsService: TransactionsService) {}

    ngOnInit(){
        this.loading = true;
        this.transactionsService.sendGetRequest().subscribe((data: any[])=>{
            this.data = data;
            this.loading = false;
        }) 

    }
}
