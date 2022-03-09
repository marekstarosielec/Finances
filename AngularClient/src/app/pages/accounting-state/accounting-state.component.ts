import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AccountingDatasetServiceFacade } from 'app/api/accountingDatasetServiceFacade';
import { AccountingDatasetInfo, AccountingDatasetService, DatasetState } from 'app/api/generated';
import { SettingsService } from 'app/api/settingsService';
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
  private executeOnOpen: boolean;
  @ViewChild('password', { static: true }) password: ElementRef;
    
  constructor(private accountingDatasetServiceFacade: AccountingDatasetServiceFacade, 
    private accountingDatasetService: AccountingDatasetService, 
    private settingService: SettingsService,
    private modalService: NgbModal) { }

  ngOnInit(): void {
    this.subscriptions.add(this.accountingDatasetServiceFacade.getDatasetInfo().subscribe((info: AccountingDatasetInfo) => {
      this.info = info;
      if (info?.state == DatasetState.Opening || info?.state == DatasetState.Closing){
        setTimeout(() =>this.accountingDatasetServiceFacade.refreshDataset(), 1000);
      }
      if (info?.state === DatasetState.Opened && this.executeOnOpen) {
        this.executeOnOpen = false;
        this.accountingDatasetService.accountingDatasetExecutePost().pipe(take(1)).subscribe(r => {
        });
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

  isOpened(): boolean {
    return this.info?.state == DatasetState.Opened;
  }

  isClosing(): boolean {
    return this.info?.state == DatasetState.Closing;
  }

  open(content) {
    this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
       if (result === 'delete')
            this.closeNoSaveConfirmed();
    }, (reason) => { });
  }

  closeNoSaveConfirmed() {
    this.accountingDatasetServiceFacade.closeDataset('', false);
    this.accountingDatasetServiceFacade.refreshDataset();
  }

  execute() {
    if (!this.settingService.CurrentPassword && this.info.state !== DatasetState.Opened) {
        this.modalService.open(this.password, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
            this.settingService.CurrentPassword = result;
            this.executeWithPassword();
        }, (reason) => { });
    } else {
        this.executeWithPassword();
    }
  }

  executeWithPassword() {
    if (!this.settingService.CurrentPassword && this.info.state !== DatasetState.Opened){
      return;
    }
    if (this.info.state === DatasetState.Closed) {
      this.executeOnOpen = true;
      this.accountingDatasetServiceFacade.openDataset(this.settingService.CurrentPassword);
    } else {
      this.accountingDatasetService.accountingDatasetExecutePost().pipe(take(1)).subscribe(r => {
      });
    }
  }
}
