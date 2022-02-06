import { Component, OnInit } from '@angular/core';
import { DocumentsService, TransactionsService } from 'app/api/generated';
import { SkodaService } from '../../api/generated/api/skoda.service'
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
        name="skoda" 
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
export class SkodaListComponent implements OnInit{
    data: any[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];
    summaries: Summary[] = [];
    
    constructor (private skodaService: SkodaService, 
        private transactionsService: TransactionsService,
        private documentsService: DocumentsService,
        private router: Router, 
        private route: ActivatedRoute) {}

    ngOnInit(){
        forkJoin([this.skodaService.skodaGet(), this.transactionsService.transactionsGet(), this.documentsService.documentsGet()])
        .pipe(take(1)).subscribe(([skoda, transactions, documents]) =>{
            const skodaMeter = skoda.map(e => ({ ...e, category: 'Licznik' }));
            const skodaTransactions = transactions.filter(f => f.category?.toUpperCase().indexOf("SKODA") > -1);
            const skodaDocuments = documents.filter(f => f.car?.toUpperCase().indexOf("GWE5533K") > -1).map(d => ({...d,  category: "Dokument", comment: d.description, showImage: 1}));
            const allTransactions = [...skodaMeter, ...skodaTransactions, ...skodaDocuments];
            this.data = allTransactions;

            this.columns = [ 
                { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true, customEvent: true},
                { title: 'Kategoria', dataProperty: 'category', component: 'list', filterOptions: { idProperty: 'category'  } as ListFilterOptions, customEvent: true},
                { title: 'Licznik', dataProperty: 'meter', component: 'text', alignment: 'right', customEvent: true},
                { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amount', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions, customEvent: true},
                { title: 'Komentarz', dataProperty: 'comment', component: 'text', customEvent: true},
                { title: '', dataProperty: 'showImage', component: 'icon', customEvent: true, image: 'nc-image', conditionalFormatting: 'bool'}
            ];
            this.toolbarElements.push({ name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew});
            this.summaries.push( { name: 'amount-category', options: { amountProperty: 'amount', categoryProperty: 'category', showDirectioned: false } as SummaryAmountCategoryOptions})            
        });
    }

    rowClickedEvent(rowClickedData: RowClickedData) {
        if (rowClickedData.row['category']==='Licznik') {
            this.router.navigate([rowClickedData.row['id']], { relativeTo: this.route});
        } else if (rowClickedData.row['category']==='Dokument' && rowClickedData.column.dataProperty!=='showImage') {
            this.router.navigate(['documents', rowClickedData.row['id']]);
        } else if (rowClickedData.row['category']==='Dokument' && rowClickedData.column.dataProperty==='showImage') {
            let fileName = rowClickedData.row['number'].toString();
            while (fileName.length < 5)
                fileName = '0' + fileName;
            fileName = 'MX' + fileName + '.' + rowClickedData.row['extension'];
            window.open("http://127.0.0.1:8080/" +fileName, "_blank", "noopener noreferrer");
        } else {
            this.router.navigate(['transactions', rowClickedData.row['id']]);
        } 
    }
        
}
