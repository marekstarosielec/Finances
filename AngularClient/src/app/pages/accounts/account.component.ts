import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { TransactionAccount, TransactionsService } from 'app/api/generated';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import { Location } from '@angular/common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import {v4 as uuidv4} from 'uuid';

@Component({
    selector: 'account',
    moduleId: module.id,
    templateUrl: 'account.component.html',
    styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit, OnDestroy{
    private routeSubscription: Subscription;
    data: TransactionAccount;
    adding: boolean = false;
    form = new FormGroup({
        title: new FormControl('', [Validators.required, Validators.minLength(3)])
    });
    constructor (private transactionsService: TransactionsService, private route: ActivatedRoute, private location: Location,
        private modalService: NgbModal) {}

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe(
            (params: Params) => {
                if (params['id']==='new'){
                    this.adding = true;
                } else { 
                    this.adding = false;
                    this.transactionsService.transactionsAccountsGet().subscribe((accounts: TransactionAccount[]) =>{
                        this.data = accounts.find(ta => ta.id === params['id']);
                    });
                }
            }
        );
    }

    ngOnDestroy() {
        this.routeSubscription.unsubscribe();
    }
    
    isSavable(): boolean {
        return this.form.valid && (this.adding || this.form.value.title!=this.data.title);
    }

    isDeletable(): boolean {
        return !this.adding;
    }

    submit() {
        if(!this.form.valid){
            return;
        }
        if (this.adding) {
            this.data = { id: uuidv4(), title: this.form.value.title }
            this.transactionsService.transactionsAccountPost(this.data).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        } else {
            this.data.title = this.form.value.title;
            this.transactionsService.transactionsAccountPut(this.data).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
       
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
