import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { DatasetInfo, DatasetService } from "./generated";
import { take } from "rxjs/operators";

@Injectable({
    providedIn: 'root'
})
export class DatasetServiceFacade {

    private datasetInfo$: BehaviorSubject<DatasetInfo> = new BehaviorSubject(null);

    constructor(private datasetService: DatasetService) {
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
        });
    }

    closeDataset() {
        this.datasetService.datasetClosePost().subscribe((result:DatasetInfo) => {
            this.datasetInfo$.next(result);
        });
    }
}