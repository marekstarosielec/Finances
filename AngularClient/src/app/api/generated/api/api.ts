export * from './mBankScrapper.service';
import { MBankScrapperService } from './mBankScrapper.service';
export * from './transactions.service';
import { TransactionsService } from './transactions.service';
export const APIS = [MBankScrapperService, TransactionsService];
