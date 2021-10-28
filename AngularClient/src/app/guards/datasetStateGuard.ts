import {Injectable} from '@angular/core';
import {ActivatedRoute, ActivatedRouteSnapshot, CanActivate, CanLoad, Route, Router, RouterStateSnapshot} from '@angular/router';
import { DatasetServiceFacade } from 'app/api/DatasetServiceFacade';
import { DatasetInfo, DatasetState } from 'app/api/generated';
import { ROUTES } from 'app/layouts/admin-layout/admin-layout.routing';
import { GetDefaultRoute } from 'app/sidebar/sidebar.component';

@Injectable({
    providedIn: 'root'
  })
export class DatasetStateGuard implements CanActivate {

    private datasetState: DatasetState;

    constructor(private router: Router, private datasetServiceFacade: DatasetServiceFacade) {
        this.datasetState = DatasetState.Closed;
        this.datasetServiceFacade.getDatasetInfo().subscribe((datasetInfo: DatasetInfo) => {
            if (datasetInfo)
                this.datasetState = datasetInfo.state;
        });
    }
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        const path = route.pathFromRoot[route.pathFromRoot.length -1].url[0].path;
        //const states = route.data["availableInStates"] as Array<DatasetState>;
        const routeInfo = ROUTES.find(r => r.path === path)
        if (routeInfo && routeInfo.availableInStates.includes(this.datasetState))
            return true;
        
        this.router.navigate([GetDefaultRoute(this.datasetState).path]);
        return false;
    }
    
}