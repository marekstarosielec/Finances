import { Route, Routes } from '@angular/router';

// import { DashboardComponent } from '../../pages/dashboard/dashboard.component';
// import { UserComponent } from '../../pages/user/user.component';
// import { TransactionsComponent } from '../../pages/transactions/transactions.component';
// import { TypographyComponent } from '../../pages/typography/typography.component';
// import { IconsComponent } from '../../pages/icons/icons.component';
// import { MapsComponent } from '../../pages/maps/maps.component';
// import { NotificationsComponent } from '../../pages/notifications/notifications.component';
// import { OpenDatasetComponent } from '../../pages/open-dataset/open-dataset.component';
// import { CloseDatasetComponent } from 'app/pages/close-dataset/close-dataset.component';
import { DatasetStateGuard } from 'app/guards/datasetStateGuard';
import { DatasetState } from 'app/api/generated';
import { ROUTES } from 'app/sidebar/sidebar.component';


export const AdminLayoutRoutes: Routes = ROUTES.map(r => { 
    const r1: Route = { 
        path: r.path, 
        component: r.component,
        canActivate: [DatasetStateGuard],
        data: { states: r.availableInStates}
    }; 
    return r1;
});

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
