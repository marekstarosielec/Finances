import { Component, OnInit } from '@angular/core';
import { DatasetService } from 'app/api/generated';
import { DatasetInfo } from 'app/api/generated/model/datasetInfo';
import { DatasetState } from 'app/api/generated/model/datasetState';
import { BehaviorSubject, Subject } from 'rxjs';
import { MBankScrapperService } from '../../api/generated/api/mBankScrapper.service';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { Transaction } from '../../api/generated/model/transaction';
import * as _ from 'lodash';
import { ActivatedRoute, Params, Router } from '@angular/router';

@Component({
    selector: 'transactions',
    moduleId: module.id,
    templateUrl: 'transactions.component.html',
    styleUrls: ['./transactions.component.scss']
})
export class TransactionsComponent implements OnInit{
    data: Transaction[];
    accountList: string[];
    public accountFilter: string = '';
    numberOfRecords: number = 100;
    sortColumn: string = 'date';
    sortOrder: number = -1;
    totalNumberOfRecords: number = 0;
    dataSubject = new BehaviorSubject(null);
    loading: boolean;

    constructor (private transactionsService: TransactionsService, private mbankScrappingService: MBankScrapperService,
        private datasetService: DatasetService, private router: Router, private route: ActivatedRoute) {}

    ngOnInit(){
        this.loading = true;
        this.transactionsService.transactionsGet().subscribe((transactions: Transaction[]) =>{
            this.data = transactions;
            this.totalNumberOfRecords = transactions.length;
            this.accountList = _(transactions).groupBy('account')
                .map(function(elements, account) {
                    return account;
                }).value().sort((a,b) => (a > b) ? 1 : ((b > a) ? -1 : 0));
            this.loading = false;
            this.prepareView();
        });
        this.route.queryParams.subscribe((qp: Params) => {
            this.numberOfRecords = qp.limit ?? 100;
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
        if (!this.data)
            return;

        let data = this.data;
        if (this.accountFilter !== '') {
            data = data.filter(d => d.account === this.accountFilter);
        }

        data = data.sort((a,b) => (a[this.sortColumn] > b[this.sortColumn]) ? this.sortOrder : ((b[this.sortColumn] > a[this.sortColumn]) ? this.sortOrder * (-1) : 0))
        if (this.numberOfRecords && this.numberOfRecords != 0) {
            data = data.slice(0, this.numberOfRecords);
        }
        this.dataSubject.next(data);
    }

    showAll() {
        this.router.navigate(['/transactions'], { queryParams: { limit: 0 } });
    }

    showSome(){
        this.router.navigate(['/transactions'], { queryParams: {  limit: 100 } });
    }

    filterByAccount(account: string) {
        this.accountFilter = account;
        this.prepareView();
    }

    selectTransaction(id: string) {
        this.router.navigate([id], { relativeTo: this.route});
    }

    addNew() {
        this.router.navigate(["new"], { relativeTo: this.route});
    }
}
