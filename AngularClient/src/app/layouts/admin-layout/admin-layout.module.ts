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
import { TransactionAutoCategoriesComponent } from 'app/pages/transaction-auto-categories/transaction-auto-categories.component';
import { CCBalanceComponent } from 'app/pages/dashboard/cc-balance/cc-balance.component';
import { ScrappingInfoComponent } from 'app/pages/dashboard/scrapping-info/scrapping-info.component';
import { BillsComponent } from 'app/pages/dashboard/bills/bills.component';
import { DocumentStateComponent } from 'app/pages/document-state/document-state.component';
import { DateFilterComponent } from 'app/shared/date-filter/date-filter.component';
import { GridComponent } from 'app/shared/grid/grid.component';
import { ListPageComponent } from 'app/pages/list-page/list-page.component';
import { FormattedNumberPipe } from 'app/pipes/formattedNumber.component';
import { FormattedAmountPipe } from 'app/pipes/formattedAmount.component';
import { FormattedDatePipe } from 'app/pipes/formattedDate.component';
import { DynamicPipe } from 'app/pipes/dynamic.component';
import { ListFilterComponent } from 'app/shared/list-filter/list-filter.component';
import { AmountFilterComponent } from 'app/shared/amount-filter/amount-filter.component';
import { TextFilterComponent } from 'app/shared/text-filter/text-filter.component';
import { DetailsViewComponent } from 'app/shared/details-view/details-view.component';
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
import { NumberFilterComponent } from 'app/shared/number-filter/number-filter.component';
import { ElectricityListComponent } from 'app/pages/electricity/electricity-list.component';
import { ElectricityDetailsComponent } from 'app/pages/electricity/electricity-details.component';
import { GasListComponent } from 'app/pages/gas/gas-list.component';
import { GasDetailsComponent } from 'app/pages/gas/gas-details.component';
import { BalanceListComponent } from 'app/pages/balances/balance-list.component';
import { BalanceDetailsComponent } from 'app/pages/balances/balance-details.component';
import { BalanceSummaryListComponent } from 'app/pages/balances/balance-summary-list.component';
import { FormattedAmountWithEmptyPipe } from 'app/pipes/formattedAmountWithEmpty.component';
import { BalanceSummaryDetailsComponent } from 'app/pages/balances/balance-summary-details.component';
import { TutoringListListComponent } from 'app/pages/tutoring-list/tutoring-list-list.component';
import { TutoringListDetailsComponent } from 'app/pages/tutoring-list/tutoring-list-details.component';
import { TutoringListComponent } from 'app/pages/tutoring/tutoring-list.component';
import { TutoringDetailsComponent } from 'app/pages/tutoring/tutoring-details.component';
import { CaseListListComponent } from 'app/pages/case-list/case-list-list.component';
import { CaseListDetailsComponent } from 'app/pages/case-list/case-list-details.component';
import { CaseListComponent } from 'app/pages/case/case-list.component';

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
    TextFilterComponent,
    GridComponent,
    ListPageComponent,
    FormattedNumberPipe,
    FormattedAmountPipe,
    FormattedAmountWithEmptyPipe,
    FormattedDatePipe,
    DynamicPipe,
    ListFilterComponent,
    AmountFilterComponent,
    DetailsViewComponent,
    SkodaListComponent,
    SkodaDetailsComponent,
    MazdaListComponent,
    MazdaDetailsComponent,
    AccountingStateComponent,
    CurrencyExchangeListComponent,
    CurrencyExchangeDetailsComponent,
    NumberFilterComponent,
    ElectricityListComponent,
    ElectricityDetailsComponent,
    GasListComponent,
    GasDetailsComponent,
    BalanceListComponent,
    BalanceDetailsComponent,
    BalanceSummaryListComponent,
    BalanceSummaryDetailsComponent,
    TutoringListListComponent,
    TutoringListDetailsComponent,
    TutoringListComponent,
    TutoringDetailsComponent,
    CaseListListComponent,
    CaseListDetailsComponent,
    CaseListComponent
  ]
})

export class AdminLayoutModule {}
