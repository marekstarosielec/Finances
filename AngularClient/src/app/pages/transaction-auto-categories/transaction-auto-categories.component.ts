import { Component, OnInit } from '@angular/core';
import { TransactionAutoCategory } from 'app/api/generated';
import { TransactionsService } from '../../api/generated/api/transactions.service'
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';

@Component({
    selector: 'transaction-auto-categories',
    moduleId: module.id,
    template: `
        <list-page name="transaction-auto-categories" [columns]="columns" [data]="data" initialSortColumn="bankInfo" initialSortOrder=1 [toolbarElements]="toolbarElements" (toolbarElementClick)="toolbarElementClick($event)"></list-page>
    `
})
export class TransactionAutoCategoriesComponent implements OnInit{
    data: TransactionAutoCategory[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];

    constructor (private transactionsService: TransactionsService) {}

    ngOnInit(){
        this.transactionsService.transactionsAutocategoriesGet().pipe(take(1)).subscribe((transactionAutoCategories: TransactionAutoCategory[]) =>{
            this.data = transactionAutoCategories;
        });
        this.columns = [ 
            { title: 'Opis w banku', dataProperty: 'bankInfo', component: 'free-text'},
            { title: 'Kategoria', dataProperty: 'category', component: 'free-text'},
        ];
        this.toolbarElements.push({ name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew});
        this.toolbarElements.push({ name: 'apply', title: 'Zastosuj' });  
    }

    toolbarElementClick(toolbarElement: ToolbarElement) {
        this.transactionsService.transactionsAutocategorizePost().pipe(take(1)).subscribe(() =>{});
    }
}
