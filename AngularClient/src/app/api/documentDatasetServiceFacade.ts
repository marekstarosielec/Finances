import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { DatasetInfo, DatasetState, DocumentDatasetService } from "./generated";
import { take } from "rxjs/operators";
import { DatasetCloseInstruction } from "./generated/model/datasetCloseInstruction";
import { DatasetOpenInstruction } from "./generated/model/datasetOpenInstruction";
import { DocumentDatasetInfo } from "./generated/model/documentDatasetInfo";

@Injectable({
    providedIn: 'root'
})
export class DocumentDatasetServiceFacade {

    private datasetInfo$: BehaviorSubject<DocumentDatasetInfo> = new BehaviorSubject({ state: DatasetState.Opening});

    constructor(private documentDatasetService: DocumentDatasetService) {
        this.refreshDataset();
    }

    refreshDataset()  {
        this.documentDatasetService.documentDatasetGet().pipe(take(1)).subscribe((result:DocumentDatasetInfo) => {
            this.datasetInfo$.next(result);
        });
    }

    getDatasetInfo(): Observable<DatasetInfo> {
        return this.datasetInfo$.asObservable();
    }

    openDataset(password: string) {
        let instruction : DatasetOpenInstruction = { password: password};
        this.documentDatasetService.documentDatasetOpenPost(instruction).subscribe((result:DocumentDatasetInfo) => {
            this.datasetInfo$.next(result);
        });
    }

    closeDataset(password: string, makeBackups: boolean) {
        let instruction : DatasetCloseInstruction = { password: password, makeBackups: makeBackups};
        this.documentDatasetService.documentDatasetClosePost(instruction).subscribe((result:DocumentDatasetInfo) => {
            this.datasetInfo$.next(result);
        });
    }
}