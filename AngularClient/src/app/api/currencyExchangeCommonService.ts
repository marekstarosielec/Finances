import { Injectable } from "@angular/core";
import { NbpService } from "app/services/nbp.service";
import { take } from "rxjs/operators";
import { CurrencyExchange, CurrencyExchangeService } from "./generated";

@Injectable({
    providedIn: 'root'
})
export class CurrencyExchangeCommonService {
    constructor(private currencyExchangeService: CurrencyExchangeService, private nbpService: NbpService) {
        
    }

    public Refresh() {
        this.currencyExchangeService.currencyExchangeGet().pipe(take(1)).subscribe((currencyExchange: CurrencyExchange[]) =>{
            const sorted = currencyExchange.sort((d1,d2) => d1.date > d2.date ? -1 : 1);
            const lastDate = new Date(sorted[0].date);
            let currentDate = new Date();
            currentDate.setUTCFullYear(lastDate.getFullYear());
            currentDate.setUTCMonth(lastDate.getMonth());
            currentDate.setUTCDate(lastDate.getDate());
            currentDate.setUTCHours(0,0,0,0);
            const today = new Date();
            while (currentDate <= today) {
                const currentDataIndex = currencyExchange.findIndex(c => c.date === currentDate.toISOString().replace(/.\d+Z$/g, "Z"));
                const currentDay = currentDate.getDay();
                if (currentDataIndex === -1 && currentDay > 0 && currentDay < 6){
                    this.nbpService.ratesGet(currentDate).pipe(take(1)).subscribe((rates) => {
                        let date = new Date(rates.rates[0].effectiveDate);
                        date.setUTCHours(0,0,0,0);
            
                        const data = {  
                            date: date.toISOString(),
                            code: 'EUR',
                            rate: rates.rates[0].mid
                        } as CurrencyExchange;
                        this.currencyExchangeService.currencyExchangeCurrencyExchangePost(data).pipe(take(1)).subscribe(() =>
                        {
                        });
                    })
                }
                currentDate.setDate(currentDate.getDate() + 1);
            }
        });
    }
}