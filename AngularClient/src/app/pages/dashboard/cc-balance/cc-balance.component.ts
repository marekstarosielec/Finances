import { Component, Input } from "@angular/core";
import { Balance } from "app/api/generated";
import * as _ from 'fast-sort';

@Component({
    selector: 'cc-balance',
    moduleId: module.id,
    templateUrl: 'cc-balance.component.html',
    styleUrls: ['cc-balance.component.scss']
})

export class CCBalanceComponent {
    creditCardBalance: Balance;
    public scrappingDate: string;
    
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
        this.creditCardBalance = creditCardBalances[0];
        this.scrappingDate = this.creditCardBalance.date;
    }
}