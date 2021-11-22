import { Component, OnInit } from '@angular/core';
import { DatasetService, TransactionAccount, TransactionCategory } from 'app/api/generated';
import { BehaviorSubject, Subject } from 'rxjs';
import { MBankScrapperService } from '../../api/generated/api/mBankScrapper.service';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { Transaction } from '../../api/generated/model/transaction';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { take } from 'rxjs/operators';
import * as _ from 'fast-sort';

@Component({
    selector: 'transactions',
    moduleId: module.id,
    templateUrl: 'transactions.component.html',
    styleUrls: ['./transactions.component.scss']
})
export class TransactionsComponent implements OnInit{
    data: Transaction[];
    primaryAccountList: TransactionAccount[];
    primaryCategoryList: TransactionCategory[];
    secondaryCategoryList: TransactionCategory[];
    public accountFilter: string = '';
    public categoryFilter: string = '';
    numberOfRecords: number = 100;
    sortColumn: string = 'date';
    sortOrder: number = -1;
    filteredNumberOfRecords: number = 0;
    currentNumberOfRecords: number = 0;
    totalNumberOfRecords: number = 0;
    dataSubject = new BehaviorSubject(null);
    loading: boolean;

    constructor (private transactionsService: TransactionsService, private mbankScrappingService: MBankScrapperService,
        private datasetService: DatasetService, private router: Router, private route: ActivatedRoute) {}

    ngOnInit(){
        this.loading = true;
        this.transactionsService.transactionsGet().subscribe((transactions: Transaction[]) =>{
            this.transactionsService.transactionsAccountsGet().pipe(take(1)).subscribe((accounts: TransactionAccount[]) => {
                this.transactionsService.transactionsCategoriesGet().pipe(take(1)).subscribe((categories: TransactionCategory[]) => {
                    this.data = transactions;
                    this.totalNumberOfRecords = transactions.length;
                    this.filteredNumberOfRecords = transactions.length;
                    this.primaryAccountList = accounts;
                    this.primaryCategoryList = categories.filter(c => c.usageIndex > 0).sort((c1, c2) => c2.usageIndex - c1.usageIndex);
                    this.secondaryCategoryList = categories.filter(c => c.usageIndex === 0).sort((c1, c2) => c2.title > c1.title ? -1 : 1);
                    this.loading = false;
                    this.prepareView();
                })
            })
        });
        this.route.queryParams.subscribe((qp: Params) => {
            this.numberOfRecords = qp.limit ?? 100;
            if (this.numberOfRecords < 0) this.numberOfRecords = 100;
            this.accountFilter = qp.account ? decodeURIComponent(qp.account) : "";
            this.categoryFilter = qp.category ? decodeURIComponent(qp.category) : "";
            this.prepareView();
        });
    }

    scrapButtonClick(){
        this.mbankScrappingService.mBankScrapperPost().subscribe(t => {
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
        {
            this.currentNumberOfRecords = 0;
            this.filteredNumberOfRecords = 0;
            return;
        }

        let data = this.data;
        if (this.accountFilter !== '') {
            data = data.filter(d => d.account === this.accountFilter);
        }
        if (this.categoryFilter !== '') {
            data = data.filter(d => d.category === this.categoryFilter || (this.categoryFilter === 'missing' && !!!d.category));
        }

        if (this.sortOrder == -1)
            data = _.sort(data).by([
                { desc: t => t[this.sortColumn]},
                { asc: t => t.id}
            ]);
        else
             data = _.sort(data).by([
                { asc: t => t[this.sortColumn]},
                { asc: t => t.id}
            ]);
       
            if (this.numberOfRecords && this.numberOfRecords != 0) {
            data = data.slice(0, this.numberOfRecords);
        }
        this.currentNumberOfRecords = data.length;
        this.dataSubject.next(data);
    }

    showAll() {
        this.router.navigate(['/transactions'], { queryParams: { limit: 0 }, queryParamsHandling: "merge" });
    }

    showSome(){
        this.router.navigate(['/transactions'], { queryParams: {  limit: 100 }, queryParamsHandling: "merge" });
    }

    filterByAccount(account: string) {
        this.router.navigate(['/transactions'], { queryParams: {  account: encodeURIComponent(account) }, queryParamsHandling: "merge" });
    }

    filterByCategory(category: string) {
        this.router.navigate(['/transactions'], { queryParams: {  category: encodeURIComponent(category) }, queryParamsHandling: "merge" });
    }

    isFilteredByCategory(category: string) : boolean {
        if (category==='all' && this.categoryFilter=='') return true;
        if (category==='missing' && this.categoryFilter=='missing') return true;
        if (this.primaryCategoryList.filter(c => c.title === category).length > 0) return true;
        if (category==='other' && this.secondaryCategoryList.filter(c => c.title === this.categoryFilter).length > 0) return true;
        return false;
    }

    getCategorySecondaryFilter() {
        if (this.secondaryCategoryList.filter(c => c.title === this.categoryFilter).length > 0)
            return this.categoryFilter;
        return undefined;
    }

    selectTransaction(id: string) {
        this.router.navigate([id], { relativeTo: this.route});
    }

    addNew() {
        this.router.navigate(["new"], { relativeTo: this.route});
    }
}
