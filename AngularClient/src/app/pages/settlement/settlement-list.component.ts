import { Component, OnInit } from '@angular/core';
import { GridColumn, RowClickedData } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { forkJoin } from 'rxjs';
import { Settlement, SettlementService } from 'app/api/generated';
import { Summary } from '../list-page/list-page.component';
import { ActivatedRoute, Router } from '@angular/router';
import { ToolbarElement } from 'app/shared/models/toolbar';
import { CurrencyExchangeCommonService } from 'app/api/currencyExchangeCommonService';

@Component({
    moduleId: module.id,
    template: `
        <list-page 
        name="settlement" 
        [columns]="columns" 
        [data]="data" 
        initialSortColumn="title" 
        initialSortOrder=-1
        (rowClicked)="rowClickedEvent($event)"
        [summaries]="summaries"
        [toolbarElements]="toolbarElements">
        </list-page>
    `
})
export class SettlementListComponent implements OnInit{
    data: any[]; 
    columns: GridColumn[];
    summaries: Summary[] = [];
    toolbarElements: ToolbarElement[] = [];
    
    constructor (private settlementService: SettlementService, 
        private router: Router, 
        private route: ActivatedRoute,
        private currencyExchangeCommonService: CurrencyExchangeCommonService) {}

    ngOnInit(){
        this.currencyExchangeCommonService.Refresh();
        forkJoin([this.settlementService.settlementGet()])
        .pipe(take(1)).subscribe(([settlementList]) =>{
            

            this.data = settlementList.map(s => ({...s, 
                eur: 'EUR', 
                pln: 'PLN', 
                formattedAmounts: this.formatAmounts(s), 
                taxes: this.formatTaxes(s),                    
                pitTitle: this.formatWithTitle('pit', s.pitMonth),
                vatTitle: this.formatWithTitle('vat', s.vatMonth)}));

            this.columns = [ 
                { title: 'Data', dataProperty: 'title', component: 'text'},
                { title: 'Przychód', dataProperty: 'incomeGrossEur', additionalDataProperty1: 'eur',  pipe: 'amountwithempty', component: 'amount', alignment: 'right'},
                { title: 'Przewalutowania', dataProperty: 'formattedAmounts', component: 'text', alignment: 'left', noWrap: true},
                { title: 'Podatki', dataProperty: 'taxes', subDataProperty1: 'pitTitle', subDataProperty2: 'vatTitle', component: 'text', alignment: 'left', noWrap: true},
                // { title: 'Pit', dataProperty: 'pit', pipe: 'number', component: 'text', alignment: 'right'},
                // { title: 'Vat', dataProperty: 'vat', pipe: 'number', component: 'text', alignment: 'right'},
                
                
                //  { title: 'Zysk', dataProperty: 'revenue', pipe: 'number', component: 'text', alignment: 'right'},
                //{ title: 'Kurs', dataProperty: 'exchangeRatio', pipe: 'number', component: 'text', alignment: 'right'},
                { title: 'Pozostało', dataProperty: 'remainingEur', additionalDataProperty1: 'eur', pipe: 'number', component: 'text', alignment: 'right'},
                { title: 'Zus', dataProperty: 'zus', pipe: 'number', component: 'text', alignment: 'right'},
                { title: 'Komentarz', dataProperty: 'comment', component: 'text'},
                { title: 'Zus', dataProperty: 'zusPaid', component: 'iconSet', alignment: 'Left', imageSet: ['nc-simple-remove','nc-check-2', 'nc-zoom-split', '']},
                { title: 'Pit', dataProperty: 'pitPaid', component: 'iconSet', alignment: 'Left', imageSet: ['nc-simple-remove','nc-check-2', 'nc-zoom-split', '']},
                { title: 'Vat', dataProperty: 'vatPaid', component: 'iconSet', alignment: 'Left', imageSet: ['nc-simple-remove','nc-check-2', 'nc-zoom-split', '']},
                
                // { title: 'Pit', dataProperty: 'pit', pipe: 'number', component: 'text', alignment: 'right'},
                // { title: 'Vat', dataProperty: 'vat', pipe: 'number', component: 'text', alignment: 'right'},
                
            ];
         });
    }

    formatAmounts(settlement: Settlement) : string {
        if (!settlement.eurConvertedToPln)
            return "";
        return settlement.eurConvertedToPln.toFixed(2) + 'EUR * ' + settlement.convertionExchangeRatio.toFixed(4) + ' = '  + settlement.plnReceivedFromConvertion.toFixed(2)+'PLN';
    }

    formatTaxes(settlement: Settlement) : string {
        if (!settlement.pit || !settlement.vat)
            return "";
        return settlement.pitAndVatMonthPln.toFixed(2) + 'PLN (' + settlement.pitAndVatMonthEur.toFixed(2) + 'EUR)';
    }

    formatWithTitle(title: string, amount: number) : string {
        if (!amount)
            return "";
        return title + ': ' + amount.toFixed(2) + 'pln';
    }


    rowClickedEvent(rowClickedData: RowClickedData) {
        if (!rowClickedData.row['transaction']) {
            this.router.navigate([rowClickedData.row['id']], { relativeTo: this.route});
        } else {
            this.router.navigate(['transactions', rowClickedData.row['id']]);
        } 
    }
}
