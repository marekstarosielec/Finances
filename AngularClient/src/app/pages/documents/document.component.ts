import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import {Document, DocumentsService } from 'app/api/generated';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import {v4 as uuidv4} from 'uuid';
import { Location } from '@angular/common';

@Component({
    selector: 'document',
    moduleId: module.id,
    templateUrl: 'document.component.html',
    styleUrls: ['./document.component.scss']
})
export class DocumentComponent implements OnInit, OnDestroy{
    private routeSubscription: Subscription;
    data: Document;
    adding: boolean = false;
    form = new FormGroup({
        id: new FormControl(''),
        number: new FormControl('', [Validators.required]),
        date: new FormControl('', []),
        pages: new FormControl('', []),
        description: new FormControl('', []),
        category: new FormControl('', []),
        invoiceNumber: new FormControl('', []),
        company: new FormControl('', []),
        person: new FormControl('', []),
        car: new FormControl('', []),
        relatedObject: new FormControl('', []),
        guarantee: new FormControl('', []),
        extension: new FormControl('', []),
    });
    
    constructor (private documentsService: DocumentsService, private route: ActivatedRoute, private location: Location,
        private modalService: NgbModal) {}

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe(
            (params: Params) => {
                if (params['id']==='new'){
                    this.adding = true;
                    let date = new Date();
                    this.form.controls['date'].setValue({year: date.getFullYear(), month:date.getMonth()+1, day: date.getDate()});
                    this.form.controls['pages'].setValue(null);
                    this.documentsService.documentsNumberGet().pipe(take(1)).subscribe((maxNumber: number) => {
                        this.form.controls['number'].setValue(maxNumber+1);
                    })
                } else { 
                    this.adding = false;
                    this.documentsService.documentsIdGet(params['id']).pipe(take(1)).subscribe((result: Document) => {
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
            this.documentsService.documentsDocumentPost(this.form.value).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        } else {
            this.documentsService.documentsDocumentPut(this.form.value).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
       
    }

    delete() {
        this.documentsService.documentsDocumentIdDelete(this.data.id).pipe(take(1)).subscribe(() =>
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
