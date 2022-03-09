import { Injectable} from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate,  Router, RouterStateSnapshot} from '@angular/router';
import { DatasetServiceFacade } from 'app/api/DatasetServiceFacade';
import { DatasetInfo, DatasetState } from 'app/api/generated';

@Injectable({
    providedIn: 'root'
  })
export class DatasetStateGuard implements CanActivate {

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

        if (this.datasetState === DatasetState.Closed)
            this.router.navigate(['/dataset']);
        // if (this.datasetState === DatasetState.Opened)
        //     this.router.navigate(['/dashboard']);
        if (this.datasetState === DatasetState.Opening || this.datasetState === DatasetState.Closing)
            this.router.navigate(['/dataset']);
        return false;
    }
}