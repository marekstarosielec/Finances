import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Balance, BalancesService, Transaction, TransactionAccount, TransactionCategory, TransactionsService } from 'app/api/generated';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import {v4 as uuidv4} from 'uuid';
import { Location } from '@angular/common';

@Component({
    selector: 'balance',
    moduleId: module.id,
    templateUrl: 'balance.component.html',
    styleUrls: ['./balance.component.scss']
})
export class BalanceComponent implements OnInit, OnDestroy{
    private routeSubscription: Subscription;
    data: Balance;
    accounts: TransactionAccount[];
    adding: boolean = false;
    form = new FormGroup({
        id: new FormControl(''),
        date: new FormControl('', []),
        account: new FormControl('', [Validators.required]),
        amount: new FormControl('', [Validators.required]),
    });
    
    constructor (private balancesService: BalancesService, private transactionsService: TransactionsService, private route: ActivatedRoute, private location: Location,
        private modalService: NgbModal) {}

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe(
            (params: Params) => {
                this.transactionsService.transactionsAccountsGet().pipe(take(1)).subscribe((result: TransactionAccount[]) => {
                    this.accounts = result;
                });
                if (params['id']==='new'){
                    this.adding = true;
                    let date = new Date();
                    this.form.controls['date'].setValue({year: date.getFullYear(), month:date.getMonth()+1, day: date.getDate()});
                } else { 
                    this.adding = false;
                    this.balancesService.balancesIdGet(params['id']).pipe(take(1)).subscribe((result: Balance) => {
                        this.data = result;
                        this.form.setValue(result);
                        let date = new Date(result.date);
                        this.form.controls['date'].setValue({year: date.getFullYear(), month:date.getMonth()+1, day: date.getDate()});
                    });
                }
            }
          );
    }

   ngOnDestroy() {
        this.routeSubscription.unsubscribe();
   }

    isSavable(): boolean {
        return this.form.valid && (this.adding || this.isFormChanged());
    }

    isFormChanged() : boolean {
        if (!this.data)
            return true;
        
        var props = Object.getOwnPropertyNames(this.data);
        for (var i = 0; i < props.length; i++) {
            if (this.data[props[i]] !== this.form.value[props[i]]) {
                return true;
            }
        }
        
        return false;
    }

    isDeletable(): boolean {
        return !this.adding;
    }

    submit() {
        if(!this.form.valid){
            return;
        }
        let resultDate = new Date();
        resultDate.setUTCFullYear(this.form.value.date.year);
        resultDate.setUTCMonth(this.form.value.date.month-1);
        resultDate.setUTCDate(this.form.value.date.day);
        this.form.value.date=resultDate;
        
        if (this.adding) {
            this.form.value.id = uuidv4();
            this.balancesService.balancesBalancePost(this.form.value).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        } else {
            this.balancesService.balancesBalancePut(this.form.value).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
       
    }

    delete() {
        this.balancesService.balancesBalanceIdDelete(this.data.id).pipe(take(1)).subscribe(() =>
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
