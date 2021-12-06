import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { TransactionAccount, TransactionCategory, TransactionsService } from 'app/api/generated';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import { Location } from '@angular/common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import {v4 as uuidv4} from 'uuid';

@Component({
    selector: 'transaction-category',
    moduleId: module.id,
    templateUrl: 'transaction-category.component.html',
    styleUrls: ['./transaction-category.component.scss']
})
export class TransactionCategoryComponent implements OnInit, OnDestroy{
    private routeSubscription: Subscription;
    data: TransactionCategory;
    adding: boolean = false;
    form = new FormGroup({
        id: new FormControl('', []),
        usageIndex: new FormControl('', []),
        title: new FormControl('', [Validators.required, Validators.minLength(3)]),
        deleted: new FormControl('', [])
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
                    this.transactionsService.transactionsCategoriesGet().subscribe((result: TransactionCategory[]) =>{
                        this.data = result.find(ta => ta.id === params['id']);
                        this.form.setValue(this.data);
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
        if (this.adding) {
            this.form.value.id = uuidv4();
            this.transactionsService.transactionsCategoryPost(this.form.value).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        } else {
            this.transactionsService.transactionsCategoryPut(this.form.value).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
    }

    delete() {
        this.transactionsService.transactionsCategoryIdDelete(this.data.id).pipe(take(1)).subscribe(() =>
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
