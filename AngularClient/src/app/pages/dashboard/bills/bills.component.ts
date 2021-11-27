import { Component, Input, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { StatisticsBill, StatisticsBills, TransactionCategory, TransactionsService } from "app/api/generated";
import { take } from "rxjs/operators";

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
    allCategories: TransactionCategory[];
    constructor(private router: Router, private modalService: NgbModal, private transactionsService: TransactionsService) 
    {
    }

    ngOnInit(): void {
        this.transactionsService.transactionsCategoriesGet().pipe(take(1)).subscribe((categories:TransactionCategory[]) => {
            this.allCategories = categories;
        });
    }

    @Input()
    set bills(value: StatisticsBills){
        this.recalculate(value);
    }

    recalculate(value: StatisticsBills): void {
        if (!!!value)
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

        value.categories.forEach(element => {
            const amounts = value.amounts.filter(a => a.category === element);
            this.categories.push({ name: element, amounts: amounts})
        });
        this.sums = value.amounts.filter(a => !!!a.category);
    }

    showTransactions(category: string){
        this.router.navigate(["transactions"], { queryParams: {  category: encodeURIComponent(category) }});
    }

    open(content) {
        this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
            if (result === 'save')   
            {
                
            }
        }, (reason) => { });
    }
}