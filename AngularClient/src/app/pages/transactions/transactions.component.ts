import { Component, OnInit } from '@angular/core';
import { DatasetService } from 'app/api/generated';
import { DatasetInfo } from 'app/api/generated/model/datasetInfo';
import { DatasetState } from 'app/api/generated/model/datasetState';
import { BehaviorSubject, Subject } from 'rxjs';
import { MBankScrapperService } from '../../api/generated/api/mBankScrapper.service';
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
    dataSubject = new BehaviorSubject(null);
    loading: boolean;

    constructor (private transactionsService: TransactionsService, private mbankScrappingService: MBankScrapperService,
        private datasetService: DatasetService) {}

    ngOnInit(){
        this.loading = true;
        this.transactionsService.transactionsGet().subscribe((transactions: Transaction[]) =>{
            this.data = transactions;
            this.dataSubject.next(transactions);
            this.loading = false;
        });
    }

    scrapButtonClick(){
        this.mbankScrappingService.mBankScrapperPost().subscribe(t => {
            console.log(t);
        })
    }

    datasetButtonClick(){
        this.datasetService.datasetGet().subscribe((t: DatasetInfo) => {
           if (t.state == DatasetState.Error)
            console.error(t);
        else
            console.log(t);
        })
    }

    sort(column: string)
    {
        let sorted = this.dataSubject.value.sort((a,b) => (a[column] > b[column]) ? 1 : ((b[column] > a[column]) ? -1 : 0))
        this.dataSubject.next(sorted);
    }
}
