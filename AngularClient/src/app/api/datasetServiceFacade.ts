import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { DatasetInfo, DatasetService, DatasetState } from "./generated";
import { take } from "rxjs/operators";

@Injectable({
    providedIn: 'root'
})
export class DatasetServiceFacade {

    private datasetInfo$: BehaviorSubject<DatasetInfo> = new BehaviorSubject({ state: DatasetState.Opening});

    constructor(private datasetService: DatasetService) {
        this.refreshDataset();
    }

    refreshDataset()  {
        this.datasetService.datasetGet().pipe(take(1)).subscribe((result:DatasetInfo) => {
            this.datasetInfo$.next(result);
        });
    }

    getDatasetInfo(): Observable<DatasetInfo> {
        return this.datasetInfo$.asObservable();
    }

    openDataset() {
        this.datasetService.datasetOpenPost().subscribe((result:DatasetInfo) => {
            console.log('Dataset is opening. Current state is ' + result.state);
            this.datasetInfo$.next(result);
        });
    }

    closeDataset() {
        this.datasetService.datasetClosePost().subscribe((result:DatasetInfo) => {
            this.datasetInfo$.next(result);
        });
    }
}