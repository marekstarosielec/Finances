import { Component, OnInit } from '@angular/core';
import { Balance, BalancesService, StatisticsAll, StatisticsService, Transaction } from 'app/api/generated';
import { take } from 'rxjs/operators';
import * as _ from 'fast-sort';

interface DashboardElement {
  title: string;
  categories: string[];
  skipSums?: boolean;
}

@Component({
    selector: 'dashboard-cmp',
    moduleId: module.id,
    templateUrl: 'dashboard.component.html',
    styleUrls: ['dashboard.component.scss']
})

export class DashboardComponent implements OnInit{
  public creditCardBalance: Balance;
  public scrappingDate: string;
  public balances: Balance[];
  public transactions: Transaction[];
  public statistics: StatisticsAll;
  public title: string = "";
  public categoryTitles: string[] = [];
  public dashboardElements: DashboardElement[] = [
    { title: "Rachunki", categories: ["Gaz", "Prąd", "Internet", "Telefon", "Woda", "Śmieci", "Kredyt hipoteczny", "Luxmed dzieci", "Marek ubezpieczenie na życie", "Podatek od nieruchomości"]},
    { title: "Jedzenie", categories: ["Jedzenie i chemia", "Rozrywka i restauracje"]},
    { title: "Subskrybcje", categories: ["Amazon Prime", "Apple iCloud", "Disney Plus", "Empik Premium", "F1", "Hbo", "Netflix", "Spotify"]},
    { title: "Przychody", categories: ["Marek pensja", "Mirela pensja", "Mirela premia", "500+", "300+"], skipSums: true},
    { title: "Dzieci", categories: ["Mikołaj kieszonkowe", "Mikołaj Indeks", "Mikołaj inne", "Marta kieszonkowe", "Mikołaj japoński", "Marta inne", "Marta szkoła", "Marta wydatki inne", "Konie"]},
    { title: "Firma", categories: ["Marek ZUS", "Marek podatek dochodowy", "Marek VAT"]},
    { title: "Samochody", categories: ["Mazda paliwo", "Mazda serwis", "Mazda ubezpieczenie", "Mazda eksploatacja", "Skoda paliwo", "Skoda serwis", "Skoda inne"]},
    { title: "Inne", categories: ["Charytatywne", "Domowe rzeczy", "Elektronika", "Gotówka", "Koty", "Książki, komiksy, gry", "Parking", "Wakacje", "Inne"]},
   
    
  ];
  constructor(private balancesService: BalancesService,
    private statisticsService: StatisticsService) {
  }

  ngOnInit(){
      this.balancesService.balancesGet().pipe(take(1)).subscribe((balances: Balance[]) => {
        this.balances = balances;
      });

      this.statisticsService.statisticsGet().pipe(take(1)).subscribe((statistics: StatisticsAll) => {
        this.statistics = statistics;
      });
    
    }
}
