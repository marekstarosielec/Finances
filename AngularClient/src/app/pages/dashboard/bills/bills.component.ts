import { Component, Input } from "@angular/core";
import { Transaction } from "app/api/generated";
import * as _ from 'fast-sort';

interface date {
    numericDate: string;
    monthName: string;
}

interface dateValue {
    numericDate: string;
    mount: number;
}

interface categoryValues {
    name: string;
    amounts: dateValue[];
}

@Component({
    selector: 'bills',
    moduleId: module.id,
    templateUrl: 'bills.component.html',
    styleUrls: ['bills.component.scss']
})
export class BillsComponent {
    dates: date[] = [];    
    values: categoryValues[] = [ {
        name: 'Gaz',
        amounts: []
    }];

    @Input()
    set transactions(value: Transaction[]){
        this.recalculate(value);
    }

    recalculate(value: Transaction[]): void {
        if (!!!value)
            return;

        
        let currentDate=new Date();
        for(let x=1; x<=48; x++){
            let numericMonth = '0' + (currentDate.getMonth()+1).toString();
            numericMonth = numericMonth.substring(numericMonth.length -2);
            let monthName = '';
            switch (currentDate.getMonth()){
                case 0: monthName = 'styczeń'; break;
                case 1: monthName = 'luty'; break;
                case 2: monthName = 'marzec'; break;
                case 3: monthName = 'kwiecień'; break;
                case 4: monthName = 'maj'; break;
                case 5: monthName = 'czerwiec'; break;
                case 6: monthName = 'lipiec'; break;
                case 7: monthName = 'sierpień'; break;
                case 8: monthName = 'wrzesień'; break;
                case 9: monthName = 'październik'; break;
                case 10: monthName = 'listopad'; break;
                case 11: monthName = 'grudzień'; break;
            }
            let numericDate = currentDate.getFullYear() + '-' + numericMonth;
            this.dates.push({
                numericDate: numericDate,
                monthName: monthName
            });
            currentDate.setMonth(currentDate.getMonth() -1);


            // this.values.forEach(category => {
            //     let values = value.filter(t => {
            //         console.log(t.date.substring(7));
            //         return t.date.substring(7) == numericDate
            //     });
            //     //console.log(values);
            // });
        }   
    }
}