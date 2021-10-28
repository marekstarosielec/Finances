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
    { path: '/dashboard',     title: 'Dashboard',         icon:'nc-bank',       class: '', availableInStates: [DatasetState.Open] },
    { path: '/transactions',  title: 'Tranzakcje',        icon:'nc-tile-56',    class: '', availableInStates: [DatasetState.Open] },
    { path: '/icons',         title: 'Icons',             icon:'nc-diamond',    class: '', availableInStates: [DatasetState.Open] },
    { path: '/maps',          title: 'Maps',              icon:'nc-pin-3',      class: '', availableInStates: [DatasetState.Open] },
    { path: '/notifications', title: 'Notifications',     icon:'nc-bell-55',    class: '', availableInStates: [DatasetState.Open] },
    { path: '/user',          title: 'User Profile',      icon:'nc-single-02',  class: '', availableInStates: [DatasetState.Open] },
    { path: '/typography',    title: 'Typography',        icon:'nc-caps-small', class: '', availableInStates: [DatasetState.Open] },
    { path: '/opendataset',   title: 'Otwórz zbiór',      icon:'nc-spaceship',  class: '', availableInStates: [DatasetState.Closed] },
    { path: '/closedataset',  title: 'Zamknij zbiór',     icon:'nc-spaceship',  class: '', availableInStates: [DatasetState.Open] },
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
            this.menuItems = ROUTES.filter(listTitle => listTitle.availableInStates.includes(datasetInfo.state));     
        });
        
    }

    ngOnDestroy() {
        this.dataServiceSubscription.unsubscribe();
    }
}
