import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { BalancesService, Settlement, SettlementService, TransactionsService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField } from "app/shared/details-view/details-view.component";
import { ToolbarElement, ToolbarElementAction, ToolbarElementWithData } from "app/shared/models/toolbar";
import { forkJoin, Subscription } from "rxjs";
import { take } from "rxjs/operators";
import { Location } from '@angular/common';
import { FormGroup, FormsModule } from "@angular/forms";

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
    `
})

export class SettlementDetailsComponent implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: any;
    toolbarElements: ToolbarElement[] = [];
    private routeSubscription: Subscription;
    
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
                    if (!this.data) {
                        let year = settlementList[0].year;
                        let month = settlementList[0].month + 1;
                        if (month > 12) {
                            month = 1;
                            year++;
                        }
                        this.data = { year: year, month: month };
                        this.data['title'] = year + '-' + month.toString().padStart(2,'0');
                    }
                    
                    const matchingTransactions = transactionList.filter(m => m.settlement === this.data['title']);
                    const balancesPLN = balancesList.filter(m => m.account === 'KontoFirmowe').sort((a,b) => a['date'] > b['date'] ? -1 : 1);
                    if (balancesPLN.length > 0) {
                        this.data['balanceAccountPln'] = balancesPLN[0].amount;
                    }
                    const balancesEUR = balancesList.filter(m => m.account === 'KontoFirmoweEUR').sort((a,b) => a['date'] > b['date'] ? -1 : 1);
                    if (balancesEUR.length > 0) {
                        this.data['balanceAccountEur'] = balancesEUR[0].amount;
                    }
                    if (params['id']=='new') {
                        this.data['zus'] = 1684.12;
                        this.data['reserve'] = 100;
                    }
                    this.viewDefinition = {
                        fields: [
                            { title: 'Rok', dataProperty: 'year', component: 'number', required: true} as DetailsViewField,
                            { title: 'Miesiąc', dataProperty: 'month', component: 'number', required: true} as DetailsViewField,
                            { title: 'Przychód w PLN', dataProperty: 'incomeGrossPln', component: 'amount'} as DetailsViewField,
                            { title: 'Stan konta PLN', dataProperty: 'balanceAccountPln', component: 'amount', readonly: true} as DetailsViewField,
                            { title: 'Pit', dataProperty: 'pit', component: 'amount'} as DetailsViewField,
                            { title: 'Vat', dataProperty: 'vat', component: 'amount'} as DetailsViewField,
                            { title: 'Zus', dataProperty: 'zus', component: 'amount'} as DetailsViewField,
                            { title: 'Rezerwa', dataProperty: 'reserve', component: 'amount'} as DetailsViewField,
                            { title: 'Niedopłata/nadpłata na koncie PLN', dataProperty: 'total', component: 'amount', readonly: true} as DetailsViewField,
                            { title: 'Zysk', dataProperty: 'revenue', component: 'amount', readonly: true} as DetailsViewField,
                            { title: 'Komentarz', dataProperty: 'comment', component: 'text'} as DetailsViewField
                        ]
                    };
                    this.toolbarElements.push(
                        { name: 'save', title: 'Zapisz', defaultAction: ToolbarElementAction.SaveChanges},
                        { name: 'delete', title: 'Usuń', defaultAction: ToolbarElementAction.Delete},
                    );
                });
        });
    }

    ngOnDestroy(){
        this.routeSubscription.unsubscribe();
    }

    toolbarElementClick(toolbarElementWithData: ToolbarElementWithData) {
        if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.SaveChanges) {
            if (this.data) {
                this.settlementService.settlementSettlementPut(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            } else {
                this.settlementService.settlementSettlementPost(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            }
        } else if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.Delete) {
            this.settlementService.settlementSettlementIdDelete(toolbarElementWithData.data.id).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
    }

    valueChange(data: Settlement) {
        data.revenue = data.incomeGrossPln - data.pit - data.vat - data.zus; //pit i vat jest za kwartał, trzeba to uwzględnić
        data.total = data.balanceAccountPln - data.pit - data.vat - data.zus - data.reserve;
    }
}
