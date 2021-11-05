import { Component, OnInit } from '@angular/core';
import { TransactionCategory } from 'app/api/generated';
import { BehaviorSubject } from 'rxjs';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'transaction-categories',
    moduleId: module.id,
    templateUrl: 'transaction-categories.component.html',
    styleUrls: ['./transaction-categories.component.scss']
})
export class TransactionCategoriesComponent implements OnInit{
    data: TransactionCategory[] = [ {}];
    sortColumn: string = 'title';
    sortOrder: number = 1;
    dataSubject = new BehaviorSubject(null);
    
    constructor (private transactionsService: TransactionsService, private router: Router, private route: ActivatedRoute) {}

    ngOnInit(){
        this.transactionsService.transactionsCategoriesGet().subscribe((data: TransactionCategory[]) =>{
            this.data = data;
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

    addNew() {
        this.router.navigate(["new"], { relativeTo: this.route});
    }
}
