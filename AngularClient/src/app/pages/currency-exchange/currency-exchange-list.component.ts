import { Component, OnInit } from '@angular/core';
import { CurrencyExchange, CurrencyExchangeService} from 'app/api/generated';
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';
import { CurrencyExchangeCommonService } from 'app/api/currencyExchangeCommonService';

@Component({
    moduleId: module.id,
    template: `
        <list-page name="currency-exchange" 
        [columns]="columns" 
        [data]="data" 
        initialSortColumn="date" 
        initialSortOrder=-1
        [toolbarElements]="toolbarElements" 
        (toolbarElementClick)="toolbarElementClick($event)">
        </list-page>
    `
})
export class CurrencyExchangeListComponent implements OnInit{
    data: CurrencyExchange[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];
   
    constructor (private currencyExchangeService: CurrencyExchangeService, private currenccyExchangeCommonService: CurrencyExchangeCommonService) {}

    ngOnInit(){
        this.currencyExchangeService.currencyExchangeGet().pipe(take(1)).subscribe((currencyExchange: CurrencyExchange[]) =>{
            this.data = currencyExchange;
            this.columns = [
                { title: 'Data', dataProperty: 'date', pipe: 'date', component: 'date', noWrap: true},
                { title: 'Waluta', dataProperty: 'code', component: 'text'},
                { title: 'Kurs', dataProperty: 'rate', component: 'number', pipe: 'number'}];
            this.toolbarElements.push({ name: 'nbp', title: 'NBP' });  
            this.toolbarElements.push({ name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew});
            
        });
    }

    toolbarElementClick(toolbarElement: ToolbarElement) {
        this.currenccyExchangeCommonService.Refresh();
    }
}


