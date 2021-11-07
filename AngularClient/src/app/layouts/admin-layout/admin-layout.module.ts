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
    BalancesComponent,
    BalanceComponent,
    OpenDatasetComponent,
    CloseDatasetComponent,
    TypographyComponent,
    IconsComponent,
    NotificationsComponent,
    LoadingIndicatorComponent
  ]
})

export class AdminLayoutModule {}
