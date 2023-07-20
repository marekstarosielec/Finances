import { Component, Input, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { StatisticsBill, StatisticsBills } from "app/api/generated";

interface date {
    numericDate: string;
    monthName: string;
}

interface categoryValues {
    name: string;
    amounts: StatisticsBill[];
}

@Component({
    selector: 'bills',
    moduleId: module.id,
    templateUrl: 'bills.component.html',
    styleUrls: ['bills.component.scss']
})
export class BillsComponent implements OnInit {
    dates: date[] = [];    
    categories: categoryValues[] = [];
    sums: StatisticsBill[] = [];
    @Input() title: string = "";
    @Input() categoryTitles: string[] = [];
    @Input() skipSums: boolean = false;
    constructor(private router: Router) 
    {
    }

    ngOnInit(): void {

    }

    @Input()
    set bills(value: StatisticsBills){
        this.recalculate(value);
    }

    recalculate(value: StatisticsBills): void {
        if (!value)
            return;
        value.periods.forEach(element => {
            let numericMonth = '0' + (element.month).toString();
            numericMonth = numericMonth.substring(numericMonth.length -2);

            let monthName = '';
            switch (element.month){
                case 1: monthName = 'styczeń'; break;
                case 2: monthName = 'luty'; break;
                case 3: monthName = 'marzec'; break;
                case 4: monthName = 'kwiecień'; break;
                case 5: monthName = 'maj'; break;
                case 6: monthName = 'czerwiec'; break;
                case 7: monthName = 'lipiec'; break;
                case 8: monthName = 'sierpień'; break;
                case 9: monthName = 'wrzesień'; break;
                case 10: monthName = 'październik'; break;
                case 11: monthName = 'listopad'; break;
                case 12: monthName = 'grudzień'; break;
            }

            let numericDate = element.year + '-' + numericMonth;
            this.dates.push({
                numericDate: numericDate,
                monthName: monthName
            });
        });

        this.categoryTitles.forEach(element => {
            const amounts = value.amounts.filter(a => a.category === element);
            this.categories.push({ name: element, amounts: amounts})
        });
        value.periods.forEach(period => {
            var amount = value.amounts.filter(a => a.period.year === period.year && a.period.month === period.month && this.categoryTitles.indexOf(a.category) > -1).reduce((sum, current) => sum + current.amount, 0);
            this.sums.push({ period: period, amount: amount });
        });
    }

    showTransactions(category: string){
        this.router.navigate(["transactions"], { queryParams: {  transactions_filter_category_noAppendix: category }});
    }
}