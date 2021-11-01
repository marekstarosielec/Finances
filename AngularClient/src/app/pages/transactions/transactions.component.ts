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
    numberOfRecords: number = 100;
    sortColumn: string = 'date';
    sortOrder: number = -1;
    totalNumberOfRecords: number = 0;
    dataSubject = new BehaviorSubject(null);
    showAllRecords: boolean = false;
    loading: boolean;

    constructor (private transactionsService: TransactionsService, private mbankScrappingService: MBankScrapperService,
        private datasetService: DatasetService) {}

    ngOnInit(){
        this.loading = true;
        this.transactionsService.transactionsGet().subscribe((transactions: Transaction[]) =>{
            this.data = transactions;
            this.totalNumberOfRecords = transactions.length;
            this.loading = false;
            this.prepareView();
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
        if (column === this.sortColumn){
            this.sortOrder = this.sortOrder * (-1);
        } else {
            this.sortColumn = column;
            this.sortOrder = -1;
        }
        this.prepareView();
    }

    prepareView() {
        let data = this.data;
        data = data.sort((a,b) => (a[this.sortColumn] > b[this.sortColumn]) ? this.sortOrder : ((b[this.sortColumn] > a[this.sortColumn]) ? this.sortOrder * (-1) : 0))
        if (!this.showAllRecords) {
            data = data.slice(0, this.numberOfRecords);
        }
        this.dataSubject.next(data);
    }

    showAll() {
        this.showAllRecords = true;
        this.prepareView();
    }

    showSome(){
        this.showAllRecords = false;
        this.prepareView();
    }
}
