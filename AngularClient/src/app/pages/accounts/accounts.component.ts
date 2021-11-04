import { Component, OnInit } from '@angular/core';
import { TransactionAccount } from 'app/api/generated';
import { BehaviorSubject } from 'rxjs';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'accounts',
    moduleId: module.id,
    templateUrl: 'accounts.component.html',
    styleUrls: ['./accounts.component.scss']
})
export class AccountsComponent implements OnInit{
    data: TransactionAccount[] = [ {}];
    sortColumn: string = 'title';
    sortOrder: number = 1;
    dataSubject = new BehaviorSubject(null);
    
    constructor (private transactionsService: TransactionsService, private router: Router, private route: ActivatedRoute) {}

    ngOnInit(){
        this.transactionsService.transactionsAccountsGet().subscribe((accounts: TransactionAccount[]) =>{
            this.data = accounts;
            this.prepareView();
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

    prepareView() {
        let data = this.data;
        data = data.sort((a,b) => (a[this.sortColumn] > b[this.sortColumn]) ? this.sortOrder : ((b[this.sortColumn] > a[this.sortColumn]) ? this.sortOrder * (-1) : 0));
        this.dataSubject.next(data);
    }

    selectRecord(id: string) {
        this.router.navigate([id], { relativeTo: this.route});
    }
}
