import { Component, OnInit } from '@angular/core';
import { DocumentsService, TransactionsService } from 'app/api/generated';
import { ElectricityService } from '../../api/generated/api/electricity.service'
import { GridColumn, RowClickedData } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';
import { forkJoin } from 'rxjs';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';
import { ListFilterOptions } from 'app/shared/list-filter/list-filter.component';
import { ActivatedRoute, Router } from '@angular/router';
import { Summary, SummaryAmountCategoryOptions, SummaryAmountCurrencyOptions } from '../list-page/list-page.component';

@Component({
    moduleId: module.id,
    template: `
        <list-page 
        name="electricity" 
        [columns]="columns" 
        [data]="data" 
        initialSortColumn="date" 
        initialSortOrder=-1 
        [toolbarElements]="toolbarElements"
        (rowClicked)="rowClickedEvent($event)"
        [summaries]="summaries">
        </list-page>
    `
})
export class ElectricityListComponent implements OnInit{
    data: any[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];
    summaries: Summary[] = [];
    
    constructor (private electricityService: ElectricityService, 
        private transactionsService: TransactionsService,
        private documentsService: DocumentsService,
        private router: Router, 
        private route: ActivatedRoute) {}

    ngOnInit(){
        forkJoin([this.electricityService.electricityGet(), this.transactionsService.transactionsGet(), this.documentsService.documentsGet()])
        .pipe(take(1)).subscribe(([electricity, transactions, documents]) =>{
            const electricityMeter = electricity.map(e => ({ ...e, category: 'Licznik' }));
            const electricityTransactions = transactions.filter(f => f.category?.toUpperCase().indexOf("PRĄD") > -1);
            const allTransactions = [...electricityMeter, ...electricityTransactions];
            this.data = allTransactions;

            this.columns = [ 
                { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true, customEvent: true},
                { title: 'Kategoria', dataProperty: 'category', component: 'list', filterOptions: { idProperty: 'category'  } as ListFilterOptions, customEvent: true},
                { title: 'Licznik', dataProperty: 'meter', pipe: 'number', component: 'text', alignment: 'right', customEvent: true},
                { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amount', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions, customEvent: true},
                { title: 'Komentarz', dataProperty: 'comment', component: 'text', customEvent: true}
            ];
            this.toolbarElements.push({ name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew});
            this.summaries.push( { name: 'amount-category', options: { amountProperty: 'amount', categoryProperty: 'category', showDirectioned: false } as SummaryAmountCategoryOptions})            
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
