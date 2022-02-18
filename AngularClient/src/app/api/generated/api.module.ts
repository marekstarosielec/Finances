import { NgModule, ModuleWithProviders, SkipSelf, Optional } from '@angular/core';
import { Configuration } from './configuration';
import { HttpClient } from '@angular/common/http';

import { AccountingDatasetService } from './api/accountingDataset.service';
import { BalancesService } from './api/balances.service';
import { CurrenciesService } from './api/currencies.service';
import { CurrencyExchangeService } from './api/currencyExchange.service';
import { DatasetService } from './api/dataset.service';
import { DocumentDatasetService } from './api/documentDataset.service';
import { DocumentsService } from './api/documents.service';
import { ElectricityService } from './api/electricity.service';
import { GasService } from './api/gas.service';
import { MBankScrapperService } from './api/mBankScrapper.service';
import { MazdaService } from './api/mazda.service';
import { SantanderScrapperService } from './api/santanderScrapper.service';
import { SkodaService } from './api/skoda.service';
import { StatisticsService } from './api/statistics.service';
import { TransactionsService } from './api/transactions.service';

@NgModule({
  imports:      [],
  declarations: [],
  exports:      [],
  providers: []
})
export class ApiModule {
    public static forRoot(configurationFactory: () => Configuration): ModuleWithProviders<ApiModule> {
        return {
            ngModule: ApiModule,
            providers: [ { provide: Configuration, useFactory: configurationFactory } ]
        };
    }

    constructor( @Optional() @SkipSelf() parentModule: ApiModule,
                 @Optional() http: HttpClient) {
        if (parentModule) {
            throw new Error('ApiModule is already loaded. Import in your base AppModule only.');
        }
        if (!http) {
            throw new Error('You need to import the HttpClientModule in your AppModule! \n' +
            'See also https://github.com/angular/angular/issues/20575');
        }
    }
}
