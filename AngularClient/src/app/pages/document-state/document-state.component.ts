import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DocumentDatasetServiceFacade } from 'app/api/documentDatasetServiceFacade';
import { DatasetState, DocumentDatasetInfo } from 'app/api/generated';
import { Subscription } from 'rxjs';


@Component({
  selector: 'app-document-state',
  templateUrl: './document-state.component.html',
  styleUrls: ['./document-state.component.scss']
})
export class DocumentStateComponent implements OnInit, OnDestroy {

  public info: DocumentDatasetInfo;
  private subscriptions: Subscription = new Subscription();
  public todayDate : Date = new Date();
  formOpen = new FormGroup({
      password: new FormControl('', [Validators.required, Validators.minLength(8)])
  });
  formClose = new FormGroup({
    password: new FormControl('', [Validators.required, Validators.minLength(8)]),
    password2: new FormControl('', [Validators.required, Validators.minLength(8)])
  });
  
  constructor(private documentDatasetServiceFacade: DocumentDatasetServiceFacade, private modalService: NgbModal) { }

  ngOnInit(): void {
    this.subscriptions.add(this.documentDatasetServiceFacade.getDatasetInfo().subscribe((info: DocumentDatasetInfo) => {
      this.info = info;
      this.formOpen.controls['password'].setValue('');
      this.formClose.controls['password'].setValue('');
      this.formClose.controls['password2'].setValue('');
      if (info?.state == DatasetState.Opening || info?.state == DatasetState.Closing){
        setTimeout(() =>this.documentDatasetServiceFacade.refreshDataset(), 1000);
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
    this.documentDatasetServiceFacade.openDataset(this.formOpen.value.password);
    this.documentDatasetServiceFacade.refreshDataset();
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
    this.documentDatasetServiceFacade.closeDataset(this.formClose.value.password, true);    
    this.documentDatasetServiceFacade.refreshDataset();
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
    this.documentDatasetServiceFacade.closeDataset(this.formClose.value.password, false);
    this.documentDatasetServiceFacade.refreshDataset();
  }
}
