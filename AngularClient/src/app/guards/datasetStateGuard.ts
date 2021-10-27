import {Injectable} from '@angular/core';
import {ActivatedRoute, ActivatedRouteSnapshot, CanActivate, CanLoad, Route, Router, RouterStateSnapshot} from '@angular/router';
import { DatasetServiceFacade } from 'app/api/DatasetServiceFacade';
import { DatasetInfo, DatasetState } from 'app/api/generated';
import { ROUTES } from 'app/sidebar/sidebar.component';

@Injectable({
    providedIn: 'root'
  })
export class DatasetStateGuard implements CanActivate, CanLoad {

    private datasetState: DatasetState;

    constructor(private router: Router, private datasetServiceFacade: DatasetServiceFacade) {
        this.datasetState = DatasetState.Closing;
        this.datasetServiceFacade.getDatasetInfo().subscribe((datasetInfo: DatasetInfo) => {
            if (datasetInfo)
                this.datasetState = datasetInfo.state;
        });
    }
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        const states = route.data["states"] as Array<DatasetState>;
        if (states.includes(this.datasetState))
            return true;
        this.router.navigate(['/opendataset']);
        return false;
    }
    canLoad(route: Route): boolean {
        return  true;
    }
    
}