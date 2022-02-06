import { Component, OnDestroy, OnInit } from '@angular/core';
import { DatasetServiceFacade } from 'app/api/DatasetServiceFacade';
import { DatasetInfo, DatasetState } from 'app/api/generated';
import { DatasetService } from 'app/api/generated/api/dataset.service';
import { Subscription } from 'rxjs';


export interface RouteInfo {
    path: string;
    title: string;
    icon: string;
    class: string;
    availableInStates: DatasetState[]
}

export const ROUTES: RouteInfo[] = [
    { path: '/dashboard',     title: 'Pulpit',         icon:'nc-bank',       class: '', availableInStates: [DatasetState.Opened, DatasetState.ClosingError]  },
    { path: '/transactions',  title: 'Tranzakcje',        icon:'nc-tile-56',    class: '', availableInStates: [DatasetState.Opened, DatasetState.ClosingError] },
    { path: '/balances',  title: 'Salda',        icon:'nc-money-coins',    class: '', availableInStates: [DatasetState.Opened, DatasetState.ClosingError] },
    { path: '/documents',  title: 'Dokumenty',        icon:'nc-single-copy-04',    class: '', availableInStates: [DatasetState.Opened, DatasetState.ClosingError] },
    { path: '/skoda',  title: 'Skoda',        icon:'nc-bus-front-12',    class: '', availableInStates: [DatasetState.Opened, DatasetState.ClosingError] },
    // { path: '/icons',         title: 'Icons',             icon:'nc-diamond',    class: '', availableInStates: [DatasetState.Opened] },
    /*{ path: '/notifications', title: 'Notifications',     icon:'nc-bell-55',    class: '', availableInStates: [DatasetState.Opened] },
    { path: '/user',          title: 'User Profile',      icon:'nc-single-02',  class: '', availableInStates: [DatasetState.Opened] },
    { path: '/typography',    title: 'Typography',        icon:'nc-caps-small', class: '', availableInStates: [DatasetState.Opened] },*/
    { path: '/opendataset',   title: 'Otwórz zbiór',      icon:'nc-spaceship',  class: '', availableInStates: [DatasetState.Closed, , DatasetState.OpeningError] },
    { path: '/closedataset',  title: 'Zamknij zbiór',     icon:'nc-spaceship',  class: '', availableInStates: [DatasetState.Opened, DatasetState.ClosingError] },
];

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
            this.menuItems = ROUTES.filter(listTitle => listTitle.availableInStates.includes(datasetInfo?.state));     
        });      
    }

    ngOnDestroy() {
        this.dataServiceSubscription.unsubscribe();
    }
}
