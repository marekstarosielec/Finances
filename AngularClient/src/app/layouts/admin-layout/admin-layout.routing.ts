import { Routes } from '@angular/router';

import { DashboardComponent } from '../../pages/dashboard/dashboard.component';
import { UserComponent } from '../../pages/user/user.component';
import { TransactionsComponent } from '../../pages/transactions/transactions.component';
import { TypographyComponent } from '../../pages/typography/typography.component';
import { IconsComponent } from '../../pages/icons/icons.component';
import { MapsComponent } from '../../pages/maps/maps.component';
import { NotificationsComponent } from '../../pages/notifications/notifications.component';
import { OpenDatasetComponent } from '../../pages/open-dataset/open-dataset.component';
import { CloseDatasetComponent } from 'app/pages/close-dataset/close-dataset.component';
import { DatasetStateGuard } from 'app/guards/datasetStateGuard';
import { DatasetState } from 'app/api/generated';
import { LoadingIndicatorComponent } from 'app/shared/loading-indicator/loading-indicator.component';

export const AdminLayoutRoutes: Routes = [
    { path: 'dashboard',      component: DashboardComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    { path: 'user',           component: UserComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    { path: 'transactions',   component: TransactionsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    { path: 'typography',     component: TypographyComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    { path: 'icons',          component: IconsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    { path: 'maps',           component: MapsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    { path: 'notifications',  component: NotificationsComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    { path: 'opendataset',    component: OpenDatasetComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Closed]} },
    { path: 'closedataset',   component: CloseDatasetComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opened]} },
    { path: 'loading',         component: LoadingIndicatorComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Opening, DatasetState.Closing]} }
];
