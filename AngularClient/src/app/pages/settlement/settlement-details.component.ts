import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { BalancesService, Settlement, SettlementService, Transaction, TransactionsService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField } from "app/shared/details-view/details-view.component";
import { ToolbarElement, ToolbarElementAction, ToolbarElementWithData } from "app/shared/models/toolbar";
import { forkJoin, Subscription } from "rxjs";
import { take } from "rxjs/operators";
import { Location } from '@angular/common';
import { FormGroup, FormsModule } from "@angular/forms";
import { GridColumn, RowClickedData } from "app/shared/grid/grid.component";
import { ListFilterOptions } from "app/shared/list-filter/list-filter.component";
import { TextFilterOptions } from "app/shared/text-filter/text-filter.component";
import { AmountFilterOptions } from "app/shared/amount-filter/amount-filter.component";

@Component({
    moduleId: module.id,
    template: `
        <details-view 
        [viewDefinition]="viewDefinition" 
        [data]="data" 
        [toolbarElements]="toolbarElements" 
        (toolbarElementClick)="toolbarElementClick($event)"
        (valueChange)="valueChange($event)"
        ></details-view>
        
        <grid [name]="name" [columns]="columns" [data]="subList"
                    initialSortColumn="date" initialSortOrder="1"
                    (rowClicked)="rowClickedEvent($event)"></grid>
    `
})

export class SettlementDetailsComponent implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: Settlement;
    toolbarElements: ToolbarElement[] = [];
    private routeSubscription: Subscription;
    
    public columns: GridColumn[];
    public subList: Transaction[];

    constructor(private settlementService: SettlementService, 
        private balancesService: BalancesService,
        private transactionsService: TransactionsService,
        private route: ActivatedRoute, 
        private location: Location) {  
    }

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe((params: Params) => {
            forkJoin([
                this.settlementService.settlementGet(), this.balancesService.balancesGet(), this.transactionsService.transactionsGet()])
                .pipe(take(1)).subscribe(([settlementList, balancesList, transactionList]) => {
                    this.data = settlementList.filter(m => m.id == params['id'])[0];
                   
                    this.subList = transactionList.filter(m => m.settlement === this.data['title']);
                    this.columns = [ 
                        { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true, customEvent: true},
                        { title: 'Konto', dataProperty: 'account', component: 'list', filterOptions: { idProperty: 'account' } as ListFilterOptions, customEvent: true},
                        { title: 'Kategoria', dataProperty: 'category', component: 'list', filterOptions: { idProperty: 'category', usageIndexPeriodDays: 40, usageIndexThreshold: 5, usageIndexPeriodDateProperty: 'date' } as ListFilterOptions, customEvent: true},
                        { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amount', alignment: 'right', noWrap:true, conditionalFormatting: 'amount', component: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions, customEvent: true},
                        { title: 'Opis', dataProperty: 'bankInfo', subDataProperty1: 'comment', component: 'text', filterOptions: { additionalPropertyToSearch1: 'comment' } as TextFilterOptions, customEvent: true}
                    ];

                    this.viewDefinition = {
                        fields: [
                            { title: 'Okres', dataProperty: 'title', component: 'text', required: true, readonly: true} as DetailsViewField,
                            { title: 'Przychód w EUR', dataProperty: 'incomeGrossEur', component: 'amount', readonly: this.data.closed} as DetailsViewField,
                            { title: 'Przychód w PLN', dataProperty: 'incomeGrossPln', component: 'amount', readonly: this.data.closed} as DetailsViewField,
                            { title: 'Kurs wymiany', dataProperty: 'exchangeRatio', component: 'number', readonly: true} as DetailsViewField,
                            { title: 'Stan konta PLN', dataProperty: 'balanceAccountPln', component: 'amount', readonly: true} as DetailsViewField,
                            { title: 'Pit', dataProperty: 'pit', component: 'amount', readonly: this.data.closed} as DetailsViewField,
                            { title: 'Vat', dataProperty: 'vat', component: 'amount', readonly: this.data.closed} as DetailsViewField,
                            { title: 'Zus', dataProperty: 'zus', component: 'amount', readonly: this.data.closed} as DetailsViewField,
                            { title: 'Rezerwa', dataProperty: 'reserve', component: 'amount', readonly: this.data.closed} as DetailsViewField,
                            { title: 'Niedopłata/nadpłata na koncie PLN', dataProperty: 'total', component: 'amount', readonly: true} as DetailsViewField,
                            { title: 'Zysk', dataProperty: 'revenue', component: 'amount', readonly: true} as DetailsViewField,
                            { title: 'Komentarz', dataProperty: 'comment', component: 'text'} as DetailsViewField
                        ]
                    };

                    if (this.data.closed) {
                        var i = this.viewDefinition.fields.length;
                        while (i--) {
                            if (this.viewDefinition.fields[i].dataProperty === 'balanceAccountPln'
                                || this.viewDefinition.fields[i].dataProperty === 'balanceAccountEur'
                                || this.viewDefinition.fields[i].dataProperty === 'total') {
                                this.viewDefinition.fields.splice(i, 1);
                            }
                        }
                    } else {
                        const balancesPLN = balancesList.filter(m => m.account === 'KontoFirmowe').sort((a,b) => a['date'] > b['date'] ? -1 : 1);
                        if (balancesPLN.length > 0) {
                            this.data['balanceAccountPln'] = balancesPLN[0].amount;
                        }
                        
                        this.toolbarElements.push(
                            { name: 'save', title: 'Zapisz', defaultAction: ToolbarElementAction.SaveChanges},
                            { name: 'delete', title: 'Usuń', defaultAction: ToolbarElementAction.Delete},
                            { name: 'close', title: 'Zamknij', align: 'right'}
                        );
                    }
                });
        });
    }

    ngOnDestroy(){
        this.routeSubscription.unsubscribe();
    }

    toolbarElementClick(toolbarElementWithData: ToolbarElementWithData) {
        if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.SaveChanges) {
            this.settlementService.settlementSettlementPut(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        } else if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.Delete) {
            this.settlementService.settlementSettlementIdDelete(toolbarElementWithData.data.id).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        } else if (toolbarElementWithData.toolbarElement.name === 'close') {
            toolbarElementWithData.data.closed = true;
            this.settlementService.settlementSettlementPut(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
    }

    valueChange(data: Settlement) {
        data.revenue = data.incomeGrossPln - data.pit - data.vat - data.zus + data.balanceAccountPln; 
        data.total = data.balanceAccountPln - data.pit - data.vat - data.zus - data.reserve;
        data.exchangeRatio = data.incomeGrossEur == 0 ? 0 : data.incomeGrossPln / data.incomeGrossEur;
    }

    rowClickedEvent(rowClickedData: RowClickedData) {}
}
