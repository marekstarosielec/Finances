import { Component, OnDestroy, OnInit, Type } from '@angular/core';
import { Route } from '@angular/router';
import { DatasetServiceFacade } from 'app/api/DatasetServiceFacade';
import { DatasetInfo, DatasetState } from 'app/api/generated';
import { RouteInfo, ROUTES } from 'app/layouts/admin-layout/admin-layout.routing';
import { Subscription } from 'rxjs';



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
