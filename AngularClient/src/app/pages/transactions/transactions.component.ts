import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { TransactionAccount, TransactionCategory } from 'app/api/generated';
import { BehaviorSubject, ObjectUnsubscribedError, Subject, Subscription } from 'rxjs';
import { MBankScrapperService } from '../../api/generated/api/mBankScrapper.service';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { Transaction } from '../../api/generated/model/transaction';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { debounceTime, distinctUntilChanged, filter, map, take } from 'rxjs/operators';
import * as _ from 'fast-sort';
import * as l from 'lodash';
import { DateChange } from 'app/shared/date-filter/date-filter.component';

export interface AmountSums {
    amount: number;
    currency: string;
    incoming: number;
    outgoing: number;
}

@Component({
    selector: 'transactions',
    moduleId: module.id,
    templateUrl: 'transactions.component.html',
    styleUrls: ['./transactions.component.scss']
})
export class TransactionsComponent implements OnInit, OnDestroy, AfterViewInit{
    data: Transaction[];
    primaryAccountList: TransactionAccount[];
    primaryCategoryList: TransactionCategory[];
    secondaryCategoryList: TransactionCategory[];
    public accountFilter: string = '';
    public categoryFilter: string = '';
    descriptionFilter: string = '';
    dateFromFilter: Date;
    dateToFilter: Date;
    sortColumn: string = 'date';
    sortOrder: number = -1;
    filteredNumberOfRecords: number = 0;
    currentNumberOfRecords: number = 0;
    totalNumberOfRecords: number = 0;
    maximumVisibleNumberOfRecords: number = 100;
    dataSubject = new BehaviorSubject(null);
    loading: boolean;
    subscription: Subscription;
    searchTerm$ = new Subject<string>();
    totalAmounts: AmountSums[];
    constructor (
        private transactionsService: TransactionsService, 
        private mbankScrappingService: MBankScrapperService,
        private router: Router, 
        private route: ActivatedRoute) {}
    
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
                    var secondaryCategories = categories.filter(c => c.usageIndex === 0);
                    secondaryCategories = _.sort(secondaryCategories).by([
                        { asc: c => c.deleted},
                        { asc: c => c.title}
                    ]);
                    this.secondaryCategoryList = secondaryCategories;
                    
                    this.loading = false;
                    this.prepareView();
                })
            })
        });
        this.route.queryParams.subscribe((qp: Params) => {
            this.maximumVisibleNumberOfRecords = qp.limit ?? 100;
            if (this.maximumVisibleNumberOfRecords < 0) this.maximumVisibleNumberOfRecords = 100;
            this.accountFilter = qp.account ? decodeURIComponent(qp.account) : "";
            this.categoryFilter = qp.category ? decodeURIComponent(qp.category) : "";
            this.descriptionFilter = qp.description ? decodeURIComponent(qp.description) : "";
            this.sortColumn = qp.sortColumn ?? 'date';
            this.sortOrder = qp.sortOrder ?? -1;
            if (qp.from)
                this.dateFromFilter = new Date(qp.from);
            else
                this.dateFromFilter = undefined;
            
            if (qp.to)
                this.dateToFilter = new Date(qp.to);
            else
                this.dateToFilter = undefined;
            this.prepareView();
        });
    }

    ngAfterViewInit(): void {
        this.subscription = this.searchTerm$.pipe(
            debounceTime(500), 
            distinctUntilChanged()
        ).subscribe((text: string) => {
            this.router.navigate(['/transactions'], { queryParams: {  description: encodeURIComponent(text) }, queryParamsHandling: "merge" });
        });
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
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
        this.router.navigate(['/transactions'], { queryParams: { sortColumn: this.sortColumn, sortOrder: this.sortOrder }, queryParamsHandling: "merge" });
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
        if (this.descriptionFilter !== '') {
            data = data.filter(d => d.bankInfo?.toUpperCase().indexOf(this.descriptionFilter.toUpperCase()) > -1
            || d.comment?.toUpperCase().indexOf(this.descriptionFilter.toUpperCase()) > -1);
        }
        if (this.dateFromFilter != undefined){
            data = data.filter(d => new Date(d.date) >= this.dateFromFilter);    
            console.log('from', this.dateFromFilter);        
        }
        if (this.dateToFilter != undefined){
            data = data.filter(d => new Date(d.date) <= this.dateToFilter);            
            console.log('to', this.dateToFilter);
        }

        this.filteredNumberOfRecords = data.length;
        
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
        
        this.totalAmounts =  l(data)
            .groupBy('currency')
            .map((objs, key) => ({
                'currency': key,
                'amount': l.sumBy(objs, 'amount'),
                'incoming': l.sumBy(objs.filter(o => o.amount > 0), 'amount'),
                'outgoing': l.sumBy(objs.filter(o => o.amount < 0), 'amount') }))
            .value();
        if (this.maximumVisibleNumberOfRecords && this.maximumVisibleNumberOfRecords != 0) {
            data = data.slice(0, this.maximumVisibleNumberOfRecords);
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
        const search = decodeURIComponent(this.categoryFilter);
        if (this.secondaryCategoryList.filter(c => c.title === search).length > 0)
            return search;
        return undefined;
    }

    selectTransaction(id: string) {
        this.router.navigate([id], { relativeTo: this.route});
    }

    addNew() {
        this.router.navigate(["new"], { relativeTo: this.route});
    }

    filterByDate(event: DateChange) : void {
        this.dateFromFilter = event.dateFrom;
        this.dateToFilter = event.dateTo;
    }

    filterByDateApply() : void {
        let from: string;
        let to: string;
        if (this.dateFromFilter != undefined)
            from = this.dateFromFilter.getFullYear() + '-' + (this.dateFromFilter.getMonth()+1) + '-' + this.dateFromFilter.getDate();
        if (this.dateToFilter != undefined)
            to = this.dateToFilter.getFullYear() + '-' + (this.dateToFilter.getMonth()+1) + '-' + this.dateToFilter.getDate();
        
        this.router.navigate(['/transactions'], { queryParams: {  
            from: from, 
            to: to, 
            }, queryParamsHandling: "merge" });
    }
}
