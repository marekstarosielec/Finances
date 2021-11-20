import { Component, Input } from "@angular/core";
import { Balance, MBankScrapperService } from "app/api/generated";
import * as _ from 'fast-sort';

@Component({
    selector: 'scrapping-info',
    moduleId: module.id,
    templateUrl: 'scrapping-info.component.html',
    styleUrls: ['scrapping-info.component.scss']
})

export class ScrappingInfoComponent {
    public scrappingDate: string;
    
    constructor(private mbankScrappingService: MBankScrapperService) {}

    @Input()
    set balances(value: Balance[]){
        this.recalculate(value);
    }

    recalculate(value: Balance[]): void {
        if (!!!value)
            return;
        let creditCardBalances = value.filter(b => b.account == 'EUR eKarta');
        creditCardBalances = _.sort(creditCardBalances).by([
            { desc: b => b.date}
        ]);
        this.scrappingDate = creditCardBalances[0].date;
    }

    scrap() {
        this.mbankScrappingService.mBankScrapperPost().subscribe(t => {
            console.log(t);
        })
    }
}