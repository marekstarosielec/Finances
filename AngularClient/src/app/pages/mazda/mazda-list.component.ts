import { Component, OnInit } from '@angular/core';
import { DocumentsService, TransactionsService } from 'app/api/generated';
import { MazdaService } from '../../api/generated/api/mazda.service'
import { GridColumn, RowClickedData } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';
import { forkJoin } from 'rxjs';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';
import { ListFilterOptions } from 'app/shared/list-filter/list-filter.component';
import { ActivatedRoute, Router } from '@angular/router';
import { Summary, SummaryAmountCategoryOptions } from '../list-page/list-page.component';

@Component({
    moduleId: module.id,
    template: `
        <list-page 
        name="mazda" 
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
export class MazdaListComponent implements OnInit{
    data: any[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];
    summaries: Summary[] = [];
    
    constructor (private mazdaService: MazdaService, 
        private transactionsService: TransactionsService,
        private documentsService: DocumentsService,
        private router: Router, 
        private route: ActivatedRoute) {}

    ngOnInit(){
        forkJoin([this.mazdaService.mazdaGet(), this.transactionsService.transactionsGet(), this.documentsService.documentsGet()])
        .pipe(take(1)).subscribe(([mazda, transactions, documents]) =>{
            const mazdaMeter = mazda.map(e => ({ ...e, category: 'Licznik' }));
            const mazdaTransactions = transactions.filter(f => f.category?.toUpperCase().indexOf("MAZDA") > -1);
            const mazdaDocuments = documents.filter(f => f.car?.toUpperCase().indexOf("GA839ES") > -1).map(d => ({...d,  category: "Dokument", comment: d.description, showImage: 1}));
            const allTransactions = [...mazdaMeter, ...mazdaTransactions, ...mazdaDocuments];
            this.data = allTransactions;

            this.columns = [ 
                { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true, customEvent: true},
                { title: 'Kategoria', dataProperty: 'category', component: 'list', filterOptions: { idProperty: 'category'  } as ListFilterOptions, customEvent: true},
                { title: 'Licznik', dataProperty: 'meter', pipe: 'number', component: 'text', alignment: 'right', customEvent: true},
                { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amountwithempty', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions, customEvent: true},
                { title: 'Komentarz', dataProperty: 'comment', component: 'text', customEvent: true},
                { title: '', dataProperty: 'showImage', component: 'icon', customEvent: true, image: 'nc-image', conditionalFormatting: 'bool'}
            ];
            this.toolbarElements.push({ name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew});
            this.summaries.push( { name: 'amount-category', options: { amountProperty: 'amount', categoryProperty: 'category', showDirectioned: false } as SummaryAmountCategoryOptions})            

            // const meterData = mazda.filter(s => s.meter && s.meter > 0).sort((s1, s2) => s1.date < s2.date ? -1 : 1);
            // const usageData = [];
            // const usageStats = [];
            // let previousDate;
            // let previousMeter;
            // meterData.forEach(m => {
            //     if (previousDate && previousMeter) {
            //         const currentDate = new Date(m.date);
            //         const meterDiff = m.meter - previousMeter;
            //         const dateDiff = (currentDate.getTime() - previousDate.getTime())  / (1000 * 60 * 60 * 24);
            //         console.log("There is " + dateDiff + " days between " + currentDate + " and " + previousDate);
            //         const meterPerDay = meterDiff / dateDiff;
            //         // console.log('meter', meterDiff);
            //         // console.log('date', dateDiff);
            //         for (let loopDiff = 0; loopDiff < dateDiff; loopDiff++) {
            //             let loopDate = new Date(currentDate);
            //             loopDate.setDate(currentDate.getDate() + loopDiff);
            //             usageData.push({
            //                 year: loopDate.getFullYear(),
            //                 month: loopDate.getMonth(),
            //                 day: loopDate.getDate(),
            //                 meter: meterPerDay
            //             });
            //         }
            //     }
            //     previousDate = new Date(m.date);
            //     previousMeter = m.meter;
            // });
            // usageData.forEach(u => {
            //     let usageStat = usageStats.findIndex(s => s.year == u.year && s.month == u.month);
            //     if (usageStat < 0) {
            //         usageStats.push({ year: u.year, month: u.month});
            //         usageStat = usageStats.findIndex(s => s.year == u.year && s.month == u.month);
            //     }
            //     if (!usageStats[usageStat]['meter']){
            //         usageStats[usageStat]['meter'] = 0;
            //     }
            //     usageStats[usageStat]['meter'] = +usageStats[usageStat]['meter'] + u.meter;  
            // });
            // console.log(usageData);
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
