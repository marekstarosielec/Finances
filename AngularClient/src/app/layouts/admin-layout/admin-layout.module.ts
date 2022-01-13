import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AdminLayoutRoutes } from './admin-layout.routing';

import { DashboardComponent }       from '../../pages/dashboard/dashboard.component';
import { UserComponent }            from '../../pages/user/user.component';
import { TransactionsComponent }    from '../../pages/transactions/transactions.component';
import { TypographyComponent }      from '../../pages/typography/typography.component';
import { IconsComponent }           from '../../pages/icons/icons.component';
import { NotificationsComponent }   from '../../pages/notifications/notifications.component';
import { OpenDatasetComponent }         from '../../pages/open-dataset/open-dataset.component';
import { LoadingIndicatorComponent } from '../../shared/loading-indicator/loading-indicator.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CloseDatasetComponent } from 'app/pages/close-dataset/close-dataset.component';
import { TransactionComponent } from 'app/pages/transactions/transaction.component';
import { AccountsComponent } from 'app/pages/accounts/accounts.component';
import { AccountComponent } from 'app/pages/accounts/account.component';
import { TransactionCategoriesComponent } from 'app/pages/transaction-categories/transaction-categories.component';
import { TransactionCategoryComponent } from 'app/pages/transaction-categories/transaction-category.component';
import { BalancesComponent } from 'app/pages/balances/balances.component';
import { BalanceComponent } from 'app/pages/balances/balance.component';
import { TransactionAutoCategoriesComponent } from 'app/pages/transaction-auto-categories/transaction-auto-categories.component';
import { TransactionAutoCategoryComponent } from 'app/pages/transaction-auto-categories/transaction-auto-category.component';
import { CCBalanceComponent } from 'app/pages/dashboard/cc-balance/cc-balance.component';
import { ScrappingInfoComponent } from 'app/pages/dashboard/scrapping-info/scrapping-info.component';
import { BillsComponent } from 'app/pages/dashboard/bills/bills.component';
import { DocumentsComponent } from 'app/pages/documents/documents.component';
import { DocumentComponent } from 'app/pages/documents/document.component';
import { DocumentStateComponent } from 'app/pages/document-state/document-state.component';
import { DateFilterComponent } from 'app/shared/date-filter/date-filter.component';
import { GridComponent } from 'app/shared/grid/grid.component';

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(AdminLayoutRoutes),
    FormsModule,
    NgbModule,
    ReactiveFormsModule
  ],
  declarations: [
    DashboardComponent,
    UserComponent,
    TransactionsComponent,
    TransactionComponent,
    AccountsComponent,
    AccountComponent,
    TransactionCategoriesComponent,
    TransactionCategoryComponent,
    TransactionAutoCategoriesComponent,
    TransactionAutoCategoryComponent,
    BalancesComponent,
    BalanceComponent,
    OpenDatasetComponent,
    CloseDatasetComponent,
    TypographyComponent,
    IconsComponent,
    NotificationsComponent,
    LoadingIndicatorComponent,
    CCBalanceComponent,
    ScrappingInfoComponent,
    BillsComponent,
    DocumentsComponent,
    DocumentComponent,
    DocumentStateComponent,
    DateFilterComponent,
    GridComponent
  ]
})

export class AdminLayoutModule {}
