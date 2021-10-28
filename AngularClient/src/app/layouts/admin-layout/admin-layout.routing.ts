import { Data, Route, RouteConfigLoadEnd, Routes } from '@angular/router';

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
import { Type } from '@angular/core';

export function GetDefaultRoute(state: DatasetState): RouteInfo {
    if (state == DatasetState.Open)
        return ROUTES.find(r => r.path === 'dashboard')
    return ROUTES.find(r => { return r.path === 'opendataset'});
}
export class RouteInfo implements Route {
    path: string;
    title: string;
    icon: string;
    class: string;
    component: Type<any>;
    canActivate?: any[];
    data?: Data;
    availableInStates: DatasetState[];
}

export const ROUTES: RouteInfo[] = [
    { path: 'dashboard',     title: 'Dashboard',         icon:'nc-bank',       class: '', component: DashboardComponent, canActivate: [DatasetStateGuard], availableInStates: [DatasetState.Open] },
    { path: 'transactions',  title: 'Tranzakcje',        icon:'nc-tile-56',    class: '', component: TransactionsComponent, canActivate: [DatasetStateGuard], availableInStates: [DatasetState.Open] },
    { path: 'icons',         title: 'Icons',             icon:'nc-diamond',    class: '', component: IconsComponent, canActivate: [DatasetStateGuard], availableInStates: [DatasetState.Open] },
    { path: 'maps',          title: 'Maps',              icon:'nc-pin-3',      class: '', component: MapsComponent, canActivate: [DatasetStateGuard], availableInStates: [DatasetState.Open] },
    { path: 'notifications', title: 'Notifications',     icon:'nc-bell-55',    class: '', component: NotificationsComponent, canActivate: [DatasetStateGuard], availableInStates: [DatasetState.Open] },
    { path: 'user',          title: 'User Profile',      icon:'nc-single-02',  class: '', component: UserComponent, canActivate: [DatasetStateGuard], availableInStates: [DatasetState.Open] },
    { path: 'typography',    title: 'Typography',        icon:'nc-caps-small', class: '', component: TypographyComponent, canActivate: [DatasetStateGuard], availableInStates: [DatasetState.Open] },
    { path: 'opendataset',   title: 'Otwórz zbiór',      icon:'nc-spaceship',  class: '', component: OpenDatasetComponent, canActivate: [DatasetStateGuard], availableInStates: [DatasetState.Closed] },
    { path: 'closedataset',  title: 'Zamknij zbiór',     icon:'nc-spaceship',  class: '', component: CloseDatasetComponent, canActivate: [DatasetStateGuard], availableInStates: [DatasetState.Open] },
];


export const AdminLayoutRoutes: Routes = ROUTES;
// export const AdminLayoutRoutes: Routes = ROUTES.map(r => { 
//     const r1: Route = { 
//         path: r.path, 
//         component: r.component,
//         canActivate: [DatasetStateGuard],
//         states: r.availableInStates}
//     }; 
//     return r1;
// });

// export const AdminLayoutRoutes: Routes = [
//      { path: 'dashboard',      component: DashboardComponent, canActivate: [DatasetStateGuard], data: {states: [DatasetState.Open]} },
//      { path: 'user',           component: UserComponent },
//      { path: 'transactions',   component: TransactionsComponent },
//      { path: 'typography',     component: TypographyComponent },
//      { path: 'icons',          component: IconsComponent },
//      { path: 'maps',           component: MapsComponent },
//      { path: 'notifications',  component: NotificationsComponent },
//     { path: 'opendataset',    component: OpenDatasetComponent },
//     { path: 'closedataset',   component: CloseDatasetComponent }
// ];
