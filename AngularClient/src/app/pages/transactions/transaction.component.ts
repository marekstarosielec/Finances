import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CurrenciesService, Currency, Transaction, TransactionAccount, TransactionCategory, TransactionsService } from 'app/api/generated';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import {v4 as uuidv4} from 'uuid';
import { Location } from '@angular/common';
import * as _ from 'fast-sort';

@Component({
    selector: 'transaction',
    moduleId: module.id,
    templateUrl: 'transaction.component.html',
    styleUrls: ['./transaction.component.scss']
})
export class TransactionComponent implements OnInit, OnDestroy{
    private routeSubscription: Subscription;
    data: Transaction;
    accounts: TransactionAccount[];
    categories: TransactionCategory[];
    currencies: Currency[];
    adding: boolean = false;
    form = new FormGroup({
        scrappingDate: new FormControl(undefined, []),
        status: new FormControl('', []),
        id: new FormControl('', []),
        date: new FormControl('', []),
        source: new FormControl('', []),
        account: new FormControl('', [Validators.required]),
        category: new FormControl('', [Validators.required]),
        amount: new FormControl('', [Validators.required]),
        description: new FormControl('', []),
        title: new FormControl('', []),
        text: new FormControl('', []),
        bankInfo: new FormControl('', []),
        comment: new FormControl('', []),
        currency: new FormControl('', []),
        details: new FormControl('', []),
        person: new FormControl('', [])
    });
    
    constructor (private transactionsService: TransactionsService, private route: ActivatedRoute, private location: Location,
        private router: Router, private modalService: NgbModal, private currenciesService: CurrenciesService) {}

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe(
            (params: Params) => {
                this.currenciesService.currenciesGet().pipe(take(1)).subscribe((result: Currency[]) => {
                    this.currencies = result;
                });
                this.transactionsService.transactionsAccountsGet().pipe(take(1)).subscribe((result: TransactionAccount[]) => {
                    this.accounts = result;
                });
                this.transactionsService.transactionsCategoriesGet().pipe(take(1)).subscribe((result: TransactionCategory[]) => {
                    this.categories = _.sort(result).by([
                        { desc: c => c.usageIndex},
                        { asc: c => c.title}
                    ]);
                });
                if (params['id']==='new'){
                    this.adding = true;
                    let date = new Date();
                    this.form.controls['currency'].setValue('PLN');
                    this.form.controls['date'].setValue({year: date.getFullYear(), month:date.getMonth()+1, day: date.getDate()});
                } else { 
                    this.adding = false;
                    this.transactionsService.transactionsIdGet(params['id']).pipe(take(1)).subscribe((result: Transaction) => {
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
        var date = new Date();
        date.setUTCFullYear(this.form.value.date.year, this.form.value.date.month-1, this.form.value.date.day);
        date.setUTCHours(0,0,0,0);
        this.form.value.date=date;

        if (this.adding) {
            this.form.value.id = uuidv4();
            this.transactionsService.transactionsTransactionPost(this.form.value).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        } else {
            this.transactionsService.transactionsTransactionPut(this.form.value).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
       
    }

    delete() {
        this.transactionsService.transactionsTransactionIdDelete(this.data.id).pipe(take(1)).subscribe(() =>
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

    addAutoCategory() {
        this.router.navigate(['transaction-auto-categories','new'], { queryParams: {  bankInfo: encodeURIComponent(this.data.bankInfo) }})
    }
}
