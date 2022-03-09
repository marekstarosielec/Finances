import { Component, OnDestroy, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { DatasetServiceFacade } from "app/api/DatasetServiceFacade";
import { DatasetInfo, DatasetState } from "app/api/generated";
import { Subscription } from "rxjs";

@Component({
    templateUrl: './dataset.component.html',
    styleUrls: ['./dataset.component.scss']
})
export class DatasetComponent implements OnInit, OnDestroy {
    private subscriptions: Subscription = new Subscription();
    public info: DatasetInfo;
    
    formClose = new FormGroup({
        password: new FormControl('', [Validators.required, Validators.minLength(8)]),
        password2: new FormControl('', [Validators.required, Validators.minLength(8)])
    });

    formOpen = new FormGroup({
        password: new FormControl('', [Validators.required, Validators.minLength(8)])
    });
    
    constructor(private datasetServiceFacade: DatasetServiceFacade) {

    }

    ngOnInit(): void {
        this.subscriptions.add(this.datasetServiceFacade.getDatasetInfo().subscribe((info: DatasetInfo) => {
            this.info = info;
            this.formOpen.controls['password'].setValue('');
            this.formClose.controls['password'].setValue('');
            this.formClose.controls['password2'].setValue('');
            if (info?.state == DatasetState.Opening || info?.state == DatasetState.Closing){
              setTimeout(() => this.datasetServiceFacade.refreshDataset(), 1000);
            }
          }));
    }

    ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    canOpen(): boolean {
        return this.info?.state == DatasetState.Closed || this.info?.state == DatasetState.OpeningError;
    }

    canClose(): boolean {
        return this.info?.state == DatasetState.Opened || this.info?.state == DatasetState.ClosingError;
    }

    isOpening(): boolean {
        return this.info?.state == DatasetState.Opening;
    }

    isClosing(): boolean {
        return this.info?.state == DatasetState.Closing;
    }

    openSubmit() {
        if (!this.formOpen.valid) return;
        this.datasetServiceFacade.openDataset(this.formOpen.value.password);
        this.datasetServiceFacade.refreshDataset();
    }

    closeSubmit() {
        if (this.formClose.controls['password'].value != this.formClose.controls['password2'].value) {
            this.formClose.controls['password2'].setErrors({ differentPassword: true })
            return;
        }
        else {
            this.formClose.controls['password2'].setErrors(null);
        }
        if (this.formClose.controls['password'].value.length != 70) {
            this.formClose.controls['password2'].setErrors({ invalidLength: true })
            return;
        }
        if (!this.formClose.valid) return;
        this.datasetServiceFacade.closeDataset(this.formClose.value.password);    
        this.datasetServiceFacade.refreshDataset();
    }
}