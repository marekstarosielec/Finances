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
import { TransactionComponent } from 'app/pages/transactions/transaction.component';
import { AccountsComponent } from 'app/pages/accounts/accounts.component';
import { AccountComponent } from 'app/pages/accounts/account.component';
import { TransactionCategoriesComponent } from 'app/pages/transaction-categories/transaction-categories.component';
import { TransactionCategoryComponent } from 'app/pages/transaction-categories/transaction-category.component';
import { BalancesComponent } from 'app/pages/balances/balances.component';
import { BalanceComponent } from 'app/pages/balances/balance.component';
import { TransactionAutoCategoryComponent } from 'app/pages/transaction-auto-categories/transaction-auto-category.component';
import { TransactionAutoCategoriesComponent } from 'app/pages/transaction-auto-categories/transaction-auto-categories.component';
import { DocumentsComponent } from 'app/pages/documents/documents.component';
import { DocumentComponent } from 'app/pages/documents/document.component';
import { Transactions2Component } from 'app/pages/transactions/transactions2.component';

export const AdminLayoutRoutes: Routes = [
    { path: 'dashboard',      component: DashboardComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    // { path: 'user',           component: UserComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    { path: 'transactions',   component: TransactionsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'transactions/:id',   component: TransactionComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'transactions2',   component: Transactions2Component, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'accounts',   component: AccountsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'accounts/:id',   component: AccountComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'transaction-categories',   component: TransactionCategoriesComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'transaction-categories/:id',   component: TransactionCategoryComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'transaction-auto-categories',   component: TransactionAutoCategoriesComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'transaction-auto-categories/:id',   component: TransactionAutoCategoryComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'balances',   component: BalancesComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'balances/:id',   component: BalanceComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'documents',   component: DocumentsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'documents/:id',   component: DocumentComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    // { path: 'typography',     component: TypographyComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
     { path: 'icons',          component: IconsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    // { path: 'notifications',  component: NotificationsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    { path: 'opendataset',    component: OpenDatasetComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Closed, DatasetState.OpeningError]} },
    { path: 'closedataset',   component: CloseDatasetComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened, DatasetState.ClosingError]} },
    { path: 'loading',        component: LoadingIndicatorComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opening, DatasetState.Closing, DatasetState.OpeningError, DatasetState.ClosingError]} }
];
