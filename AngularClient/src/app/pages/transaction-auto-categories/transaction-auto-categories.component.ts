import { Component, OnInit } from '@angular/core';
import { TransactionAutoCategory, TransactionCategory } from 'app/api/generated';
import { BehaviorSubject } from 'rxjs';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs/operators';

@Component({
    selector: 'transaction-auto-categories',
    moduleId: module.id,
    templateUrl: 'transaction-auto-categories.component.html',
    styleUrls: ['./transaction-auto-categories.component.scss']
})
export class TransactionAutoCategoriesComponent implements OnInit{
    data: TransactionAutoCategory[] = [ {}];
    sortColumn: string = 'category';
    sortOrder: number = 1;
    dataSubject = new BehaviorSubject(null);
    
    constructor (private transactionsService: TransactionsService, private router: Router, private route: ActivatedRoute) {}

    ngOnInit(){
        this.transactionsService.transactionsAutocategoriesGet().subscribe((result: TransactionAutoCategory[]) =>{
            this.data = result;
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

    apply() {
        this.transactionsService.transactionsAutocategorizePost().pipe(take(1)).subscribe(() =>{
           
        });
    }
}
