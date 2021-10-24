export * from './dataset.service';
import { DatasetService } from './dataset.service';
export * from './mBankScrapper.service';
import { MBankScrapperService } from './mBankScrapper.service';
export * from './transactions.service';
import { TransactionsService } from './transactions.service';
export const APIS = [DatasetService, MBankScrapperService, TransactionsService];
