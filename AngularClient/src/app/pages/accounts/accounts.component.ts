import { Component, OnInit } from '@angular/core';
import { TransactionAccount } from 'app/api/generated';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { GridColumn, ViewChangedData } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement, ToolbarElementAction } from '../list-page/list-page.component';

@Component({
    selector: 'accounts',
    moduleId: module.id,
    template: `
        <list-page name="accounts" [columns]="columns" [data]="data" initialSortColumn="title" initialSortOrder=1></list-page>
    `
})
export class AccountsComponent implements OnInit{
    data: TransactionAccount[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];

    constructor (private transactionsService: TransactionsService) {}

    ngOnInit(){
        this.transactionsService.transactionsAccountsGet().pipe(take(1)).subscribe((accounts: TransactionAccount[]) =>{
            this.data = accounts;
        });
        this.columns = [ { title: 'Nazwa', dataProperty: 'title', filterComponent: 'free-text'}];
    }
}
