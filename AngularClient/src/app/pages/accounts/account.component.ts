import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { TransactionAccount, TransactionsService } from 'app/api/generated';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import { Location } from '@angular/common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'account',
    moduleId: module.id,
    templateUrl: 'account.component.html',
    styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit, OnDestroy{
    private routeSubscription: Subscription;
    data: TransactionAccount;
    form = new FormGroup({
        title: new FormControl('', [Validators.required, Validators.minLength(3)])
    });
    constructor (private transactionsService: TransactionsService, private route: ActivatedRoute, private location: Location,
        private modalService: NgbModal) {}

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe(
            (params: Params) => {
                this.transactionsService.transactionsAccountsGet().subscribe((accounts: TransactionAccount[]) =>{
                    this.data = accounts.find(ta => ta.id === params['id']);
                });
            }
        );
    }

    ngOnDestroy() {
        this.routeSubscription.unsubscribe();
    }
    
    isSavable(): boolean {
        return this.form.valid && this.form.value.title!=this.data.title;
    }

    submit() {
        if(!this.form.valid){
            return;
        }
        this.data.title = this.form.value.title;
        this.transactionsService.transactionsAccountPost(this.data).pipe(take(1)).subscribe(() =>
        {
            this.location.back();
        });
    }

    delete() {
        this.transactionsService.transactionsAccountIdDelete(this.data.id).pipe(take(1)).subscribe(() =>
        {
            this.location.back();
        });
    }

    open(content) {
        this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
            if (result === 'delete')
                this.delete();
        }, (reason) => { });
      }
    
}
