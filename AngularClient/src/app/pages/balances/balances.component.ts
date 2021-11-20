import { Component, OnInit } from '@angular/core';
import { Balance, BalancesService, DatasetService, TransactionAccount, TransactionsService } from 'app/api/generated';
import { BehaviorSubject } from 'rxjs';
import * as _ from 'lodash';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs/operators';

@Component({
    selector: 'balances',
    moduleId: module.id,
    templateUrl: 'balances.component.html',
    styleUrls: ['./balances.component.scss']
})
export class BalancesComponent implements OnInit{
    data: Balance[];
    numberOfRecords: number = 100;
    sortColumn: string = 'date';
    sortOrder: number = -1;
    accountList: TransactionAccount[];
    public accountFilter: string = '';
    dataSubject = new BehaviorSubject(null);
    showAllRecords: boolean = false;
    
    constructor (private balancesService: BalancesService, private transactionsService: TransactionsService, private router: Router, private route: ActivatedRoute) {}

    ngOnInit(){
        this.balancesService.balancesGet().subscribe((balances: Balance[]) =>{
            this.transactionsService.transactionsAccountsGet().pipe(take(1)).subscribe((accounts: TransactionAccount[]) => {
                this.data = balances;
                this.accountList = accounts;
                this.prepareView();
            });
        });
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

    showAll() {
        this.showAllRecords = true;
        this.prepareView();
    }

    showSome(){
        this.showAllRecords = false;
        this.prepareView();
    }

    prepareView() {
        let data = this.data;
        if (this.accountFilter !== '') {
            data = data.filter(d => d.account === this.accountFilter);
        }
        data = data.sort((a,b) => (a[this.sortColumn] > b[this.sortColumn]) ? this.sortOrder : ((b[this.sortColumn] > a[this.sortColumn]) ? this.sortOrder * (-1) : 0))
        if (!this.showAllRecords) {
            data = data.slice(0, this.numberOfRecords);
        }
        this.dataSubject.next(data);
    }

    filterByAccount(account: string) {
        this.accountFilter = account;
        this.prepareView();
    }

    selectRecord(id) {
        this.router.navigate([id], { relativeTo: this.route});
    }

    addNew() {
        this.router.navigate(["new"], { relativeTo: this.route});
    }
}
