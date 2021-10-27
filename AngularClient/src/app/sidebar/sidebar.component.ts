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
    openDataset: boolean;
    closedDataset: boolean;
}

export const ROUTES: RouteInfo[] = [
    { path: '/dashboard',     title: 'Dashboard',         icon:'nc-bank',       class: '',              openDataset: true,              closedDataset: false },
    { path: '/transactions',  title: 'Tranzakcje',        icon:'nc-tile-56',    class: '',              openDataset: true,              closedDataset: false },
    { path: '/icons',         title: 'Icons',             icon:'nc-diamond',    class: '',              openDataset: true,              closedDataset: false },
    { path: '/maps',          title: 'Maps',              icon:'nc-pin-3',      class: '',              openDataset: true,              closedDataset: false },
    { path: '/notifications', title: 'Notifications',     icon:'nc-bell-55',    class: '',              openDataset: true,              closedDataset: false },
    { path: '/user',          title: 'User Profile',      icon:'nc-single-02',  class: '',              openDataset: true,              closedDataset: false },
    { path: '/typography',    title: 'Typography',        icon:'nc-caps-small', class: '',              openDataset: true,              closedDataset: false },
    { path: '/opendataset',   title: 'Otwórz zbiór',      icon:'nc-spaceship',  class: '',              openDataset: false,             closedDataset: true },
    { path: '/closedataset',  title: 'Zamknij zbiór',     icon:'nc-spaceship',  class: '',              openDataset: true,              closedDataset: false },
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
            if (datasetInfo && datasetInfo.state == DatasetState.Open)
                this.menuItems = ROUTES.filter(listTitle => listTitle.openDataset);
            else
                this.menuItems = ROUTES.filter(listTitle => listTitle.closedDataset);
            
        });
        
    }

    ngOnDestroy() {
        this.dataServiceSubscription.unsubscribe();
    }
}
