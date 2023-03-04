import { Routes } from '@angular/router';

import { DashboardComponent } from '../../pages/dashboard/dashboard.component';
import { UserComponent } from '../../pages/user/user.component';
import { TransactionsComponent } from '../../pages/transactions/transactions.component';
import { TypographyComponent } from '../../pages/typography/typography.component';
import { IconsComponent } from '../../pages/icons/icons.component';
import { NotificationsComponent } from '../../pages/notifications/notifications.component';
import { DatasetStateGuard } from 'app/guards/datasetStateGuard';
import { DatasetState } from 'app/api/generated';
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
import { TutoringListListComponent } from 'app/pages/tutoring-list/tutoring-list-list.component';
import { TutoringListDetailsComponent } from 'app/pages/tutoring-list/tutoring-list-details.component';
import { TutoringDetailsComponent } from 'app/pages/tutoring/tutoring-details.component';
import { TutoringListComponent } from 'app/pages/tutoring/tutoring-list.component';
import { CaseListListComponent } from 'app/pages/case-list/case-list-list.component';
import { CaseListDetailsComponent } from 'app/pages/case-list/case-list-details.component';
import { CaseListComponent } from 'app/pages/case/case-list.component';
import { DatasetComponent } from 'app/pages/dataset/dataset.component';
import { SettlementListComponent } from 'app/pages/settlement/settlement-list.component';
import { SettlementDetailsComponent } from 'app/pages/settlement/settlement-details.component';
import { WaterDetailsComponent } from 'app/pages/water/water-details.component';
import { WaterListComponent } from 'app/pages/water/water-list.component';
import { DocumentCategoryListComponent } from 'app/pages/document-category/document-category-list.component';
import { DocumentCategoryDetailsComponent } from 'app/pages/document-category/document-category-details.component';
import { IncomingComponent } from 'app/pages/Incoming/incoming.component';

export const AdminLayoutRoutes: Routes = [
    { path: 'dashboard',      component: DashboardComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'incoming',      component: IncomingComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
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
    { path: 'accounting',   component: AccountingStateComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'currency-exchange',   component: CurrencyExchangeListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'currency-exchange/:id',   component: CurrencyExchangeDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'electricity',   component: ElectricityListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'electricity/:id',   component: ElectricityDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'gas',   component: GasListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'gas/:id',   component: GasDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
     // { path: 'typography',     component: TypographyComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
     { path: 'icons',          component: IconsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    // { path: 'notifications',  component: NotificationsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    { path: 'dataset',   component: DatasetComponent},
    { path: 'tutoring-list',   component: TutoringListListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'tutoring-list/:id',   component: TutoringListDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'tutoring',   component: TutoringListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'tutoring/:id',   component: TutoringDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'case-list',   component: CaseListListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'case-list/:id',   component: CaseListDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'case',   component: CaseListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'settlement',   component: SettlementListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'settlement/:id',   component: SettlementDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'water',   component: WaterListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'water/:id',   component: WaterDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'document-category',   component: DocumentCategoryListComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'document-category/:id',   component: DocumentCategoryDetailsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    
];
