import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { NbpRates } from "./nbp-rates.model";

@Injectable({
    providedIn: 'root'
})
export class NbpService {


    constructor(protected httpClient: HttpClient) {
       
    }

    public ratesGet(date: Date): Observable<NbpRates> {
        let month = (date.getMonth()+1).toString();
        if (month.length < 2) {
            month = '0' + month;
        }
        let day = (date.getDate()).toString();
        if (day.length < 2) {
            day = '0' + day;
        }
        const parsedDate = date.getFullYear().toString()+'-'+month+'-'+day;

        return this.httpClient.get<NbpRates>(`http://api.nbp.pl/api/exchangerates/rates/a/eur/${parsedDate}/${parsedDate}/?format=json`,
            {
                responseType: <any>'json',
                observe: 'body'
            }
        );
    }
}