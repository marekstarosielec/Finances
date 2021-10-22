import { Component, OnInit } from '@angular/core';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { Transaction } from '../../api/generated/model/transaction';

@Component({
    selector: 'transactions',
    moduleId: module.id,
    templateUrl: 'transactions.component.html',
    styleUrls: ['./transactions.component.scss']
})
export class TransactionsComponent implements OnInit{
    data: Transaction[];
    loading: boolean;

    constructor (private transactionsService: TransactionsService) {}

    ngOnInit(){
        this.loading = true;
        this.transactionsService.transactionsGet().subscribe((transactions: Transaction[]) =>{
            this.data = transactions;
            this.loading = false;
        });

    }
}
