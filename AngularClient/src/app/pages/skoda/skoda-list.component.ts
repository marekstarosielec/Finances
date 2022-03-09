import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DecompressFileResult, DocumentsService, FileService, TransactionsService } from 'app/api/generated';
import { SkodaService } from '../../api/generated/api/skoda.service'
import { GridColumn, RowClickedData } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';
import { forkJoin } from 'rxjs';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';
import { ListFilterOptions } from 'app/shared/list-filter/list-filter.component';
import { ActivatedRoute, Router } from '@angular/router';
import { Summary, SummaryAmountCategoryOptions, SummaryAmountCurrencyOptions } from '../list-page/list-page.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SettingsService } from 'app/api/settingsService';

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

        <ng-template #password let-modal>
            <div class="modal-header">
                <h4 class="modal-title" id="modal-basic-title">Podaj hasło</h4>
            </div>
            <div class="modal-body">
                <input 
                    #password
                    id="password" 
                    type="password" 
                    class="form-control">
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-light" (click)="modal.close('')">Anuluj</button>
                <button type="button" class="btn btn-outline-danger" (click)="modal.close(password.value)">Otwórz</button>
            </div>
        </ng-template>
    `
})
export class SkodaListComponent implements OnInit{
    data: any[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];
    summaries: Summary[] = [];
    @ViewChild('password', { static: true }) password: ElementRef;
    
    constructor (private skodaService: SkodaService, 
        private transactionsService: TransactionsService,
        private documentsService: DocumentsService,
        private fileService: FileService,
        private settingService: SettingsService,
        private modalService: NgbModal,
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
                { title: 'Licznik', dataProperty: 'meter', pipe: 'number', component: 'text', alignment: 'right', customEvent: true},
                { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amountwithempty', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions, customEvent: true},
                { title: 'Komentarz', dataProperty: 'comment', component: 'text', customEvent: true},
                { title: '', dataProperty: 'showImage', component: 'icon', customEvent: true, image: 'nc-image', conditionalFormatting: 'bool'}
            ];
            this.toolbarElements.push({ name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew});
            this.summaries.push( { name: 'amount-category', options: { amountProperty: 'amount', categoryProperty: 'category', showDirectioned: false } as SummaryAmountCategoryOptions})            

            // const meterData = skoda.filter(s => s.meter && s.meter > 0).sort((s1, s2) => s1.date < s2.date ? -1 : 1);
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
            if (!this.settingService.CurrentPassword)
            {
                this.modalService.open(this.password, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
                    this.settingService.CurrentPassword = result;
                    this.openFile(rowClickedData);
                 }, (reason) => { });
            } else {
                this.openFile(rowClickedData);
            };
        } else {
            this.router.navigate(['transactions', rowClickedData.row['id']]);
        } 
    }
        
    openFile(rowClickedData: RowClickedData) {
        if (!this.settingService.CurrentPassword){
            return;
        }
        
        let number = rowClickedData.row['number'];
        this.fileService.filePost({ number: number, password: this.settingService.CurrentPassword}).pipe(take(1)).subscribe((result: DecompressFileResult) => {
            window.open("http://127.0.0.1:8080" + result.path, "_blank", "noopener noreferrer");
        });
    }
}
