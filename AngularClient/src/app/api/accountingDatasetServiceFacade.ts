import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { AccountingDatasetInfo, AccountingDatasetService, DatasetInfo, DatasetState } from "./generated";
import { take } from "rxjs/operators";
import { DatasetCloseInstruction } from "./generated/model/datasetCloseInstruction";
import { DatasetOpenInstruction } from "./generated/model/datasetOpenInstruction";
import { DocumentDatasetInfo } from "./generated/model/documentDatasetInfo";

@Injectable({
    providedIn: 'root'
})
export class AccountingDatasetServiceFacade {

    private datasetInfo$: BehaviorSubject<AccountingDatasetInfo> = new BehaviorSubject({ state: DatasetState.Opening});

    constructor(private accountingDatasetService: AccountingDatasetService) {
        this.refreshDataset();
    }

    refreshDataset()  {
        this.accountingDatasetService.accountingDatasetGet().pipe(take(1)).subscribe((result:AccountingDatasetInfo) => {
            this.datasetInfo$.next(result);
        });
    }

    getDatasetInfo(): Observable<DatasetInfo> {
        return this.datasetInfo$.asObservable();
    }

    openDataset(password: string) {
        let instruction : DatasetOpenInstruction = { password: password};
        this.accountingDatasetService.accountingDatasetOpenPost(instruction).subscribe((result:DocumentDatasetInfo) => {
            this.datasetInfo$.next(result);
        });
    }

    closeDataset(password: string, makeBackups: boolean) {
        let instruction : DatasetCloseInstruction = { password: password, makeBackups: makeBackups};
        this.accountingDatasetService.accountingDatasetClosePost(instruction).subscribe((result:DocumentDatasetInfo) => {
            this.datasetInfo$.next(result);
        });
    }
}