import { Injectable } from "@angular/core";
import { forkJoin, from, Observable, Subject } from "rxjs";
import { concatMap, map, take } from "rxjs/operators";
import { Balance, BalancesService, TransactionsService } from "./generated";
import * as fs from 'fast-sort';
import { updateParenthesizedType } from "typescript";

@Injectable({
    providedIn: 'root'
})
export class BalanceSummaryService {

    constructor(private balanceService: BalancesService) {
    }

    GetSummary() {
        return this.balanceService.balancesGet().pipe(take(1), map((balances: Balance[]) => {
            balances = fs.sort(balances).by([
                { desc: l => l['date']}
            ]);
            let data = [];
            let currentRecord;
            let currentDate = '';
            let sanitizedTitle = '';
            balances.forEach(balance => {
                currentDate = this.formatDate(new Date(balance.date));
                if (!currentRecord || currentDate !== currentRecord['date']){
                    if (currentRecord) {
                        data.push(currentRecord);
                    }
                    currentRecord = {};
                    currentRecord['id'] = this.getId(currentDate);
                    currentRecord['date'] = currentDate;
                }
                sanitizedTitle = this.sanitizeAccountTitle(balance.account);
                currentRecord[sanitizedTitle] = balance.amount;
                currentRecord[sanitizedTitle + '_currency'] = balance.currency;
                currentRecord[sanitizedTitle + '_title'] = balance.account;
            });
            return data;
        }));
    }

    SaveChanges(data: any) {
        let updates = [];
        Object.keys(data).forEach(property => {
            if (!property.endsWith('_currency') && !property.endsWith('_title') && property !== 'id' && property !== 'date') {
                var title = data[property + '_title'];
                var amount = data[property];
                var currency = data[property + '_currency'];
                if (amount != undefined){
                    updates.push({ account: title, amount: amount, currency: currency, date: data['date']} as Balance);
                }
            }
        });
        let finished = new Subject();
        let count = 0;
        from(updates).pipe(
            concatMap(update => <Observable<any>>this.balanceService.balancesBalancePut(update))
            ).subscribe(val => {
              count++;
              if (count == updates.length){
                finished.next();
                finished.complete();
              }
            });
        return finished;
    }

    sanitizeAccountTitle(accountTitle: string) : string {
        let result = accountTitle;
        result = result.toLowerCase();
        result = this.replaceAll(result, ' ', '_');
        result = this.replaceAll(result, 'ą', 'a');
        result = this.replaceAll(result, 'ć', 'c');
        result = this.replaceAll(result, 'ę', 'e');
        result = this.replaceAll(result, 'ł', 'l');
        result = this.replaceAll(result, 'ń', 'n');
        result = this.replaceAll(result, 'ó', 'o');
        result = this.replaceAll(result, 'ś', 's');
        result = this.replaceAll(result, 'ź', 'z');
        result = this.replaceAll(result, 'ż', 'z');
        return result;
    }

    formatDate(date: Date) : string {
        let month = (date.getMonth()+1).toString();
        if (month.length < 2) {
            month = '0' + month;
        }
        let day = (date.getDate()).toString();
        if (day.length < 2) {
            day = '0' + day;
        }
        const parsedDate = date.getFullYear().toString()+'-'+month+'-'+day;

        return parsedDate;
    }

    getId(date:string) : string {
        return this.replaceAll(date, '-', '');
    }

    replaceAll(str, find, replace) : string {
        return str.replace(new RegExp(find, 'g'), replace);
    }
}