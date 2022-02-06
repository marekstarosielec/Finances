export * from './balances.service';
import { BalancesService } from './balances.service';
export * from './currencies.service';
import { CurrenciesService } from './currencies.service';
export * from './dataset.service';
import { DatasetService } from './dataset.service';
export * from './documentDataset.service';
import { DocumentDatasetService } from './documentDataset.service';
export * from './documents.service';
import { DocumentsService } from './documents.service';
export * from './mBankScrapper.service';
import { MBankScrapperService } from './mBankScrapper.service';
export * from './skoda.service';
import { SkodaService } from './skoda.service';
export * from './statistics.service';
import { StatisticsService } from './statistics.service';
export * from './transactions.service';
import { TransactionsService } from './transactions.service';
export const APIS = [BalancesService, CurrenciesService, DatasetService, DocumentDatasetService, DocumentsService, MBankScrapperService, SkodaService, StatisticsService, TransactionsService];
