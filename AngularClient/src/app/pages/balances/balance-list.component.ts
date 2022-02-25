import { Component, OnInit } from '@angular/core';
import { Balance, BalancesService, TransactionAccount } from 'app/api/generated';
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement } from 'app/shared/models/toolbar';
import { ListFilterOptions } from 'app/shared/list-filter/list-filter.component';
import { AmountFilterOptions } from 'app/shared/amount-filter/amount-filter.component';
import { SubviewDefinition, SubViewService } from 'app/services/subview.service';

@Component({
    moduleId: module.id,
    template: `
        <list-page name="balances" [columns]="columns" [data]="data" initialSortColumn="date" initialSortOrder=-1></list-page>
    `
})
export class BalanceListComponent implements OnInit{
    data: Balance[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];

    constructor (private balanceService: BalancesService, private subviewService: SubViewService) {}

    ngOnInit(){
        this.subviewService.subviews.next([
            { title: '', icon: 'nc-tile-56', link: 'balancesummary'} as SubviewDefinition,
            { title: '', icon: 'nc-single-copy-04', link: 'balances' } as SubviewDefinition
        ]);
        
        
        this.balanceService.balancesGet().pipe(take(1)).subscribe((balances: Balance[]) =>{
            this.data = balances;
        });
        this.columns = [ 
            { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true},
            { title: 'Konto', dataProperty: 'account', component: 'list', filterOptions: { idProperty: 'account' } as ListFilterOptions},
            { title: 'Kwota', dataProperty: 'amount', additionalDataProperty1: 'currency',  pipe: 'amount', alignment: 'right', noWrap:true, component: 'amount', filterOptions: { currencyDataProperty: 'currency'} as AmountFilterOptions},  
        ];
    }
}
