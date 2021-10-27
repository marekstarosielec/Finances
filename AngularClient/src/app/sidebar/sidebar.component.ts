import { Component, OnDestroy, OnInit, Type } from '@angular/core';
import { DatasetServiceFacade } from 'app/api/DatasetServiceFacade';
import { DatasetInfo, DatasetState } from 'app/api/generated';
import { DatasetService } from 'app/api/generated/api/dataset.service';
import { CloseDatasetComponent } from 'app/pages/close-dataset/close-dataset.component';
import { DashboardComponent } from 'app/pages/dashboard/dashboard.component';
import { IconsComponent } from 'app/pages/icons/icons.component';
import { MapsComponent } from 'app/pages/maps/maps.component';
import { NotificationsComponent } from 'app/pages/notifications/notifications.component';
import { OpenDatasetComponent } from 'app/pages/open-dataset/open-dataset.component';
import { TransactionsComponent } from 'app/pages/transactions/transactions.component';
import { TypographyComponent } from 'app/pages/typography/typography.component';
import { UserComponent } from 'app/pages/user/user.component';
import { Subscription } from 'rxjs';


export interface RouteInfo {
    path: string;
    title: string;
    icon: string;
    class: string;
    availableInStates: DatasetState[];
    component: Type<any>;
}

export const ROUTES: RouteInfo[] = [
    { path: 'dashboard',     title: 'Dashboard',         icon:'nc-bank',       class: '', availableInStates: [DatasetState.Open], component: DashboardComponent },
    { path: 'transactions',  title: 'Tranzakcje',        icon:'nc-tile-56',    class: '', availableInStates: [DatasetState.Open], component: TransactionsComponent },
    { path: 'icons',         title: 'Icons',             icon:'nc-diamond',    class: '', availableInStates: [DatasetState.Open], component: IconsComponent },
    { path: 'maps',          title: 'Maps',              icon:'nc-pin-3',      class: '', availableInStates: [DatasetState.Open], component: MapsComponent },
    { path: 'notifications', title: 'Notifications',     icon:'nc-bell-55',    class: '', availableInStates: [DatasetState.Open], component: NotificationsComponent },
    { path: 'user',          title: 'User Profile',      icon:'nc-single-02',  class: '', availableInStates: [DatasetState.Open], component: UserComponent },
    { path: 'typography',    title: 'Typography',        icon:'nc-caps-small', class: '', availableInStates: [DatasetState.Open], component: TypographyComponent },
    { path: 'opendataset',   title: 'Otwórz zbiór',      icon:'nc-spaceship',  class: '', availableInStates: [DatasetState.Closed], component: OpenDatasetComponent },
    { path: 'closedataset',  title: 'Zamknij zbiór',     icon:'nc-spaceship',  class: '', availableInStates: [DatasetState.Open], component: CloseDatasetComponent },
];

export function GetDefaultRoute(state: DatasetState): RouteInfo {
    if (state == DatasetState.Open)
        return ROUTES.find(r => r.path === 'dashboard')
    return ROUTES.find(r => { return r.path === 'opendataset'});
}

@Component({
    moduleId: module.id,
    selector: 'sidebar-cmp',
    templateUrl: 'sidebar.component.html',
})

export class SidebarComponent implements OnInit, OnDestroy {
    public menuItems: any[];
    private dataServiceSubscription: Subscription;
    constructor(private datasetServiceFacade: DatasetServiceFacade) {
    }

    ngOnInit() {

        this.dataServiceSubscription = this.datasetServiceFacade.getDatasetInfo().subscribe((datasetInfo: DatasetInfo) => {
           this.menuItems = ROUTES.filter(listTitle => listTitle.availableInStates.includes(datasetInfo.state));
           
            
        });
        
    }

    ngOnDestroy() {
        this.dataServiceSubscription.unsubscribe();
    }
}
