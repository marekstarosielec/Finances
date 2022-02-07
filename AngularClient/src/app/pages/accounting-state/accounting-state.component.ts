import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AccountingDatasetServiceFacade } from 'app/api/accountingDatasetServiceFacade';
import { AccountingDatasetInfo, AccountingDatasetService, DatasetState } from 'app/api/generated';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';


@Component({
  selector: 'app-accounting-state',
  templateUrl: './accounting-state.component.html',
  styleUrls: ['./accounting-state.component.scss']
})
export class AccountingStateComponent implements OnInit, OnDestroy {

  public info: AccountingDatasetInfo;
  private subscriptions: Subscription = new Subscription();
  public todayDate : Date = new Date();
  formOpen = new FormGroup({
      password: new FormControl('', [Validators.required, Validators.minLength(8)])
  });
  formClose = new FormGroup({
    password: new FormControl('', [Validators.required, Validators.minLength(8)]),
    password2: new FormControl('', [Validators.required, Validators.minLength(8)])
  });
  
  constructor(private accountingDatasetServiceFacade: AccountingDatasetServiceFacade, private accountingDatasetService: AccountingDatasetService, private modalService: NgbModal) { }

  ngOnInit(): void {
    this.subscriptions.add(this.accountingDatasetServiceFacade.getDatasetInfo().subscribe((info: AccountingDatasetInfo) => {
      this.info = info;
      this.formOpen.controls['password'].setValue('');
      this.formClose.controls['password'].setValue('');
      this.formClose.controls['password2'].setValue('');
      if (info?.state == DatasetState.Opening || info?.state == DatasetState.Closing){
        setTimeout(() =>this.accountingDatasetServiceFacade.refreshDataset(), 1000);
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
    this.accountingDatasetServiceFacade.openDataset(this.formOpen.value.password);
    this.accountingDatasetServiceFacade.refreshDataset();
  }

  closeSubmit() {
    if (this.formClose.controls['password'].value != this.formClose.controls['password2'].value) {
      this.formClose.controls['password2'].setErrors({ differentPassword: true })
      return;
    }
    else {
        this.formClose.controls['password2'].setErrors(null);
    }
    if (!this.formClose.valid) return;
    this.accountingDatasetServiceFacade.closeDataset(this.formClose.value.password, true);    
    this.accountingDatasetServiceFacade.refreshDataset();
  }

  closeNoSave() {
    
  }

  open(content) {
    this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
       if (result === 'delete')
            this.closeNoSaveConfirmed();
    }, (reason) => { });
  }

  closeNoSaveConfirmed() {
    this.accountingDatasetServiceFacade.closeDataset(this.formClose.value.password, false);
    this.accountingDatasetServiceFacade.refreshDataset();
  }

  execute() {
    this.accountingDatasetService.accountingDatasetExecutePost().pipe(take(1)).subscribe(r => {
    });
  }
}
