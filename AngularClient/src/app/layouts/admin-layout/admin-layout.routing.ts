import { Routes } from '@angular/router';

import { DashboardComponent } from '../../pages/dashboard/dashboard.component';
import { UserComponent } from '../../pages/user/user.component';
import { TransactionsComponent } from '../../pages/transactions/transactions.component';
import { TypographyComponent } from '../../pages/typography/typography.component';
import { IconsComponent } from '../../pages/icons/icons.component';
import { NotificationsComponent } from '../../pages/notifications/notifications.component';
import { OpenDatasetComponent } from '../../pages/open-dataset/open-dataset.component';
import { CloseDatasetComponent } from 'app/pages/close-dataset/close-dataset.component';
import { DatasetStateGuard } from 'app/guards/datasetStateGuard';
import { DatasetState } from 'app/api/generated';
import { LoadingIndicatorComponent } from 'app/shared/loading-indicator/loading-indicator.component';
import { AccountsComponent } from 'app/pages/accounts/accounts.component';
import { AccountComponent } from 'app/pages/accounts/account.component';
import { TransactionCategoriesComponent } from 'app/pages/transaction-categories/transaction-categories.component';
import { TransactionAutoCategoriesComponent } from 'app/pages/transaction-auto-categories/transaction-auto-categories.component';
import { TransactionComponent } from 'app/pages/transactions/transaction.component';
import { TransactionCategoryComponent } from 'app/pages/transaction-categories/transaction-category.component';
import { TransactionAutoCategoryComponent } from 'app/pages/transaction-auto-categories/transaction-auto-category.component';
import { DocumentsComponent } from 'app/pages/documents/documents.component';
import { DocumentComponent } from 'app/pages/documents/document.component';
import { SkodaListComponent } from 'app/pages/skoda/skoda-list.component';
import { SkodaDetailsComponent } from 'app/pages/skoda/skoda-details.component';
import { MazdaListComponent } from 'app/pages/mazda/mazda-list.component';
import { MazdaDetailsComponent } from 'app/pages/mazda/mazda-details.component';
import { AccountingStateComponent } from 'app/pages/accounting-state/accounting-state.component';
import { CurrencyExchangeListComponent } from 'app/pages/currency-exchange/currency-exchange-list.component';
import { CurrencyExchangeDetailsComponent } from 'app/pages/currency-exchange/currency-exchange-details.component';
import { ElectricityListComponent } from 'app/pages/electricity/electricity-list.component';
import { ElectricityDetailsComponent } from 'app/pages/electricity/electricity-details.component';
import { GasListComponent } from 'app/pages/gas/gas-list.component';
import { GasDetailsComponent } from 'app/pages/gas/gas-details.component';
import { BalanceListComponent } from 'app/pages/balances/balance-list.component';
import { BalanceDetailsComponent } from 'app/pages/balances/balance-details.component';
import { BalanceSummaryListComponent } from 'app/pages/balances/balance-summary-list.component';
import { BalanceSummaryDetailsComponent } from 'app/pages/balances/balance-summary-details.component';

export const AdminLayoutRoutes: Routes = [
    { path: 'dashboard',      component: DashboardComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    // { path: 'user',           component: UserComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    { path: 'transactions',   component: TransactionsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'transactions/:id',   component: TransactionComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'accounts',   component: AccountsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'accounts/:id',   component: AccountComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'transaction-categories',   component: TransactionCategoriesComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'transaction-categories/:id',   component: TransactionCategoryComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'transaction-auto-categories',   component: TransactionAutoCategoriesComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'transaction-auto-categories/:id',   component: TransactionAutoCategoryComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'balances',   component: BalanceListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'balances/:id',   component: BalanceDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'balancesummary',   component: BalanceSummaryListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'balancesummary/:id',   component: BalanceSummaryDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'skoda',   component: SkodaListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'skoda/:id',   component: SkodaDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'mazda',   component: MazdaListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'mazda/:id',   component: MazdaDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'documents',   component: DocumentsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'documents/:id',   component: DocumentComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'documents/:id/:type',   component: DocumentComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'accounting',   component: AccountingStateComponent },
    { path: 'currency-exchange',   component: CurrencyExchangeListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'currency-exchange/:id',   component: CurrencyExchangeDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'electricity',   component: ElectricityListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'electricity/:id',   component: ElectricityDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'gas',   component: GasListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'gas/:id',   component: GasDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
     // { path: 'typography',     component: TypographyComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
     { path: 'icons',          component: IconsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    // { path: 'notifications',  component: NotificationsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    { path: 'opendataset',    component: OpenDatasetComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Closed, DatasetState.OpeningError]} },
    { path: 'closedataset',   component: CloseDatasetComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'loading',        component: LoadingIndicatorComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opening, DatasetState.Closing, DatasetState.OpeningError, DatasetState.ClosingError]} }
];
