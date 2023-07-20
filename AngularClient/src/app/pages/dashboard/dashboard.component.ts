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
    { title: "Rachunki", categories: ["Gaz", "Prąd", "Internet", "Telefon", "Woda", "Śmieci"]},
    { title: "Jedzenie", categories: ["Jedzenie i chemia", "Rozrywka i restauracje"]},
    { title: "Subskrybcje", categories: ["Amazon Prime", "Apple iCloud", "Disney Plus", "Empik Premium", "F1", "Hbo", "Netflix", "Spotify"]},
    { title: "Przychody", categories: ["Marek pensja", "Mirela pensja", "Mirela premia", "500+", "300+"], skipSums: true},
    { title: "Kieszonkowe", categories: ["Mikołaj kieszonkowe", "Marta kieszonkowe"]},
    { title: "Firma", categories: ["Marek ZUS", "Marek podatek dochodowy", "Marek VAT"]},
    { title: "Samochody", categories: ["Mazda paliwo", "Mazda serwis", "Mazda ubezpieczenie", "Skoda paliwo", "Skoda serwis"]},
    
    
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
