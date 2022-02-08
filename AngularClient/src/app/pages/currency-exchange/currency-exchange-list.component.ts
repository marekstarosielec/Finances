import { Component, OnInit } from '@angular/core';
import { CurrencyExchange, CurrencyExchangeService} from 'app/api/generated';
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';
import { NbpService } from 'app/services/nbp.service';

@Component({
    moduleId: module.id,
    template: `
        <list-page name="currency-exchange" 
        [columns]="columns" 
        [data]="data" 
        initialSortColumn="date" 
        initialSortOrder=1
        [toolbarElements]="toolbarElements" 
        (toolbarElementClick)="toolbarElementClick($event)">
        </list-page>
    `
})
export class CurrencyExchangeListComponent implements OnInit{
    data: CurrencyExchange[]; 
    columns: GridColumn[];
    toolbarElements: ToolbarElement[] = [];
   
    constructor (private currencyExchangeService: CurrencyExchangeService, private nbpService: NbpService) {}

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
        let currentDate = new Date(2022,1,1);
        const today = new Date();
        while (currentDate <= today) {
            console.log(currentDate.toISOString());
            const currentDataIndex = this.data.findIndex(c => c.date === currentDate.toISOString());
            const currentDay = currentDate.getDay();
            if (currentDataIndex === -1 && currentDay > 0 && currentDay < 6){
                this.nbpService.ratesGet(currentDate).pipe(take(1)).subscribe((rates) => {
                    console.log(rates);
                    const data = {  
                        date: new Date(rates.rates[0].effectiveDate).toISOString(),
                        code: 'EUR',
                        rate: rates.rates[0].mid
                    } as CurrencyExchange;
                    console.log(data);
                    this.currencyExchangeService.currencyExchangeCurrencyExchangePost(data).pipe(take(1)).subscribe(() =>
                    {
                    });
                })
            }
            currentDate.setDate(currentDate.getDate() + 1);
        }
    }
}
function NbpRates(rate: any, NbpRates: any) {
    throw new Error('Function not implemented.');
}

