import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { DatasetInfo, DatasetService, DatasetState } from "./generated";
import { take } from "rxjs/operators";
import { Router } from "@angular/router";
import { GetDefaultRoute } from "app/sidebar/sidebar.component";

@Injectable({
    providedIn: 'root'
})
export class DatasetServiceFacade {

    private datasetInfo$: BehaviorSubject<DatasetInfo> = new BehaviorSubject({ state: DatasetState.Closed });

    constructor(private router: Router, private datasetService: DatasetService) {
        this.datasetService.datasetGet().pipe(take(1)).subscribe((result:DatasetInfo) => {
            this.datasetInfo$.next(result);
        });
    }

    getDatasetInfo(): Observable<DatasetInfo> {
        return this.datasetInfo$.asObservable();
    }

    openDataset() {
        this.datasetService.datasetOpenPost().subscribe((result:DatasetInfo) => {
            this.datasetInfo$.next(result);
            this.router.navigate([GetDefaultRoute(result.state).path]);
        });
    }

    closeDataset() {
        this.datasetService.datasetClosePost().subscribe((result:DatasetInfo) => {
            this.datasetInfo$.next(result);
            this.router.navigate([GetDefaultRoute(result.state).path]);
        });
    }
}