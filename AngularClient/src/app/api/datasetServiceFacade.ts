import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { DatasetInfo, DatasetService, DatasetState } from "./generated";
import { take } from "rxjs/operators";
import { DatasetOpen } from "./generated/model/datasetOpen";
import { DatasetCloseInstruction } from "./generated/model/datasetCloseInstruction";
import { DatasetOpenInstruction } from "./generated/model/datasetOpenInstruction";

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

    openDataset(password: string) {
        let instruction : DatasetOpenInstruction = { password: password};
        this.datasetService.datasetOpenPost(instruction).subscribe((result:DatasetInfo) => {
            this.datasetInfo$.next(result);
        });
    }

    closeDataset(password: string) {
        let instruction : DatasetCloseInstruction = { password: password};
        this.datasetService.datasetClosePost(instruction).subscribe((result:DatasetInfo) => {
            this.datasetInfo$.next(result);
        });
    }
}