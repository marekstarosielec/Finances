import { Component, OnInit } from '@angular/core';
import { TransactionsService } from 'app/api/generated';
import { SkodaService } from '../../api/generated/api/skoda.service'
import { GridColumn, RowClickedData } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';
import { forkJoin } from 'rxjs';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';
import { ListFilterOptions } from 'app/shared/list-filter/list-filter.component';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    moduleId: module.id,
    template: `
        <list-page 
        name="skoda" 
        [columns]="columns" 
        [data]="data" 
        initialSortColumn="date" 
        initialSortOrder=-1 
        [toolbarElements]="toolbarElements"
        (rowClicked)="rowClickedEvent($event)">
        </list-page>
    `
})
export class SkodaListComponent implements OnInit{
    data: any[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];

    constructor (private skodaService: SkodaService, 
        private transactionsService: TransactionsService,
        private router: Router, 
        private route: ActivatedRoute) {}

    ngOnInit(){
        forkJoin([this.skodaService.skodaGet(), this.transactionsService.transactionsGet()])
        .pipe(take(1)).subscribe(([skoda, transactions]) =>{
            const skodaMeter = skoda.map(e => ({ ...e, category: 'Licznik' }));
            const skodaTransactions = transactions.filter(f => f.category?.toUpperCase().indexOf("SKODA") > -1);
            
            const allTransactions = [...skodaMeter, ...skodaTransactions];
            this.data = allTransactions;

            this.columns = [ 
                { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true, customEvent: true},
                { title: 'Kategoria', dataProperty: 'category', component: 'list', filterOptions: { idProperty: 'category'  } as ListFilterOptions, customEvent: true},
                { title: 'Licznik', dataProperty: 'meter', component: 'text', alignment: 'right', customEvent: true},
                { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amount', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions, customEvent: true},
                { title: 'Komentarz', dataProperty: 'comment', component: 'text', customEvent: true},
            ];
            this.toolbarElements.push({ name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew});
        });
    }

    rowClickedEvent(rowClickedData: RowClickedData) {
        if (rowClickedData.row['category']==='Licznik') {
            this.router.navigate([rowClickedData.row['id']], { relativeTo: this.route});
        } else {
            this.router.navigate(['transactions', rowClickedData.row['id']]);
        } 
    }
}
