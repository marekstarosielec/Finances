import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from "@angular/core";
import { FormControl, FormGroup, ValidatorFn, Validators } from "@angular/forms";
import * as l from 'lodash';
import * as fs from 'fast-sort';
import { FormattedAmountPipe } from "app/pipes/formattedAmount.component";
import { ToolbarElement, ToolbarElementAction, ToolbarElementWithData } from "../models/toolbar";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { FormattedNumberPipe } from "app/pipes/formattedNumber.component";
import { FormattedAmountWithEmptyPipe } from "app/pipes/formattedAmountWithEmpty.component";
import { Subscription } from "rxjs";
import { __values } from "tslib";
export interface DetailsViewField {
    title: string;
    dataProperty: string;
    component: string;
    readonly?: boolean;
    options?: any;
    required?: boolean;
    defaultValue?: any;
}

export interface DetailsViewFieldListOptions {
    referenceList: any[];
    referenceListIdField: string;
    usageIndexPeriodDays?: number;
    usageIndexPeriodDateProperty?: string;
    usageIndexThreshold?: number;
    usageIndexData?: any[];
}

export interface DetailsViewFieldAmountOptions {
    currencyList: any[];
    currencyListIdField: string;
    currencyDataProperty: string;
    allowEmpty: boolean;
}

export interface DetailsViewDefinition {
    fields: DetailsViewField[]
}

@Component({
    selector: 'details-view',
    templateUrl: 'details-view.component.html',
    styleUrls: ['./details-view.component.scss']
})

export class DetailsViewComponent implements OnInit, OnDestroy{ 
    @Input() name: string;
    form = new FormGroup({});

    private _viewDefinition: DetailsViewDefinition;
    get viewDefinition(): DetailsViewDefinition {
        return this._viewDefinition;
    }
    @Input()
    set viewDefinition(value: DetailsViewDefinition) {
        this._viewDefinition = value;
        this.createForm();
        if (this.dataIsInitialized){
            this.fillFormWithData();
        }
    }

    private _data: any;
    get data(): any {
        return this._data;
    }
    @Input()
    set data(value: any) {
        if (!value) {
            return;
        }
        this._data = value;
        this.fillFormWithData();
        this.dataIsInitialized = true;    
    }

    @Input() public toolbarElements: ToolbarElement[];
    @Output() public toolbarElementClick = new EventEmitter<ToolbarElementWithData>();
    @Output() public valueChange = new EventEmitter<FormGroup>();
    valueChangeEdited: boolean;
    valueChangeSubscribtion: Subscription;
    toolbarElementAction: typeof ToolbarElementAction = ToolbarElementAction;
    numberRegEx = /\-?\d*\.?\d{1,2}/;
    dataIsInitialized: boolean = false;
    constructor(private modalService: NgbModal) {

    }
    
    ngOnInit() {
        if (!this.toolbarElements) {
            this.toolbarElements = [
                { name: 'save', title: 'Zapisz', defaultAction: ToolbarElementAction.SaveChanges},
                { name: 'delete', title: 'Usuń', defaultAction: ToolbarElementAction.Delete}];
        }
    }

    ngOnDestroy() {
        if (this.valueChangeSubscribtion){
            this.valueChangeSubscribtion.unsubscribe();
        }
    }

    toolbarElementClicked(toolbarElement: ToolbarElement) {
        if (toolbarElement.defaultAction === ToolbarElementAction.SaveChanges) {
            if (!this.form.valid) {
                return;
            }   
            if (!this.data) {
                this._data = {};
                this.dataIsInitialized = true; 
            }
            let data = this.getDataFromForm();
            this.toolbarElementClick.emit({ toolbarElement: toolbarElement, data: data} as ToolbarElementWithData);
        } else {
            let data = this.getDataFromForm();
            this.toolbarElementClick.emit({ toolbarElement: toolbarElement, data: data} as ToolbarElementWithData);
        }
    }   

    private getDataFromForm() {
        if (!this.dataIsInitialized || !this.data) {
            return;
        }
        let data = { ...this.data} ?? {};
        
        this.viewDefinition.fields.forEach((field : DetailsViewField) => {
            if (field.component === 'date'){
                let month =  '0' + this.form.controls[field.dataProperty].value.month;
                if (month.length > 2) {
                    month = month.substring(month.length-2);
                }
                let day =  '0' + this.form.controls[field.dataProperty].value.day;
                if (day.length > 2) {
                    day = day.substring(day.length-2);
                }
                data[field.dataProperty] = this.form.controls[field.dataProperty].value.year + '-' + month + '-' + day + 'T00:00:00Z';
            } else if (field.component === 'amount') {
                if (field.options?.allowEmpty && !this.form.controls[field.dataProperty].value){
                    //No value set.
                } else {
                    data[field.dataProperty] = +(this.form.controls[field.dataProperty].value?.replace(",",".").replace(" ","").replace(" ",""));
                    if (field.options?.currencyDataProperty) {
                        data[field.options?.currencyDataProperty] = this.form.controls[field.options?.currencyDataProperty].value;
                    }
                }
            } else if (field.component === 'checkbox') {
                data[field.dataProperty] = this.form.controls[field.dataProperty].value ? true : false;
            } else if (field.component === 'number') {
                data[field.dataProperty] = +(this.form.controls[field.dataProperty].value?.replace(",",".").replace(" ","").replace(" ",""));
            } else {
                data[field.dataProperty] = this.form.controls[field.dataProperty].value;
            }
        });
        
       return data;
    }

    private createForm() {
        let controls = {};
        if (!this.viewDefinition?.fields) {
            return;
        }
        this.viewDefinition.fields.forEach((field : DetailsViewField) => {
            const validators : ValidatorFn[] = [];
            if (field.required) {
                validators.push(Validators.required);
            }

            if (field.component==='amount') {
                validators.push(Validators.pattern(this.numberRegEx))
                if (field.options?.currencyDataProperty) {
                    controls[field.options.currencyDataProperty] = new FormControl(undefined, []);
                }
                controls[field.dataProperty] = new FormControl(undefined, validators);
            }
            else if (field.component === 'date') {
                controls[field.dataProperty] = new FormControl(undefined, validators);  
            } else if (field.component === 'list') {
                controls[field.dataProperty] = new FormControl(undefined, validators);
            } else if (field.component === 'text') {
                controls[field.dataProperty] = new FormControl(undefined, validators);
            } else if (field.component === 'multiline-text') {
                controls[field.dataProperty] = new FormControl(undefined, validators);
            } else if (field.component === 'checkbox') {
                controls[field.dataProperty] = new FormControl(undefined, validators);
            } else if (field.component === 'number') {
                controls[field.dataProperty] = new FormControl(undefined, validators);
            }
        });
        this.form = new FormGroup(controls);
        if (this.valueChangeSubscribtion){
            this.valueChangeSubscribtion.unsubscribe();
        }

        this.valueChangeSubscribtion= this.form.valueChanges.subscribe(_ => {
            if (this.valueChangeEdited || !this.dataIsInitialized) {
                return;
            }
            this.valueChangeEdited = true;
            let currentData = this.getDataFromForm();
            const dataBeforeChange = { ...currentData};
            this.valueChange.emit(currentData);

            let fieldsToUpdate: DetailsViewField[];
            fieldsToUpdate = [];
            Object.keys(currentData).forEach(key => {
                if (currentData[key] != dataBeforeChange[key]) {
                    fieldsToUpdate.push(this.viewDefinition.fields.filter(f => f.dataProperty === key)[0])
                }
            });
            if (fieldsToUpdate.length > 0){
                this._data = currentData;
                this.fillFormWithData(fieldsToUpdate);
            }
            this.valueChangeEdited = false;
        });
    }

    private fillFormWithData(fieldsToUpdate?: DetailsViewField[]) {
        if (!fieldsToUpdate){
            fieldsToUpdate = this._viewDefinition?.fields;
        }
        if (!fieldsToUpdate) {
            return;
        }
        fieldsToUpdate.forEach((field : DetailsViewField) => {
            if (field.component==='amount') {
                if (field.options?.currencyDataProperty) {
                    this.form.controls[field.options.currencyDataProperty].setValue(this.data ? this.data[field.options.currencyDataProperty] : 'PLN') ;
                }
                if (this.data) {
                    if (field.options?.allowEmpty){
                        const amountWithEmptyPipe = new FormattedAmountWithEmptyPipe();
                         this.form.controls[field.dataProperty].setValue(amountWithEmptyPipe.transform(this.data[field.dataProperty]));
                    } else {
                        const amountPipe = new FormattedAmountPipe();
                         this.form.controls[field.dataProperty].setValue(amountPipe.transform(this.data[field.dataProperty]));
                    }
                } else {
                    this.form.controls[field.dataProperty].setValue('');
                }
            }
            else if (field.component === 'date') {
                let date = new Date();
                if (this.data) { 
                    date = new Date(this.data[field.dataProperty]);
                } else if (field.defaultValue){
                    date = new Date(field.defaultValue);
                }
                const dtpDate = {year: date.getFullYear(), month:date.getMonth()+1, day: date.getDate()};
                this.form.controls[field.dataProperty].setValue(dtpDate);
            } else if (field.component === 'list') {
                if (!field.options) {
                    field.options = {};
                }
                field.options.referenceList = this.buildReferenceListOnUsageIndex(field);
                 this.form.controls[field.dataProperty].setValue(this.data ? this.data[field.dataProperty] : '');
            } else if (field.component === 'text') {
                this.form.controls[field.dataProperty].setValue(this.data ? this.data[field.dataProperty] : '');
            } else if (field.component === 'multiline-text') {
                this.form.controls[field.dataProperty].setValue(this.data ? this.data[field.dataProperty] : '');
            } else if (field.component === 'checkbox') {
                this.form.controls[field.dataProperty].setValue(this.data ? this.data[field.dataProperty] : '');
            } else if (field.component === 'number') {
                let source = '';
                if (this.data) {
                    const numberPipe = new FormattedNumberPipe();
                    source = numberPipe.transform(this.data[field.dataProperty]);
                }
                this.form.controls[field.dataProperty].setValue(source);
            }
            if (!this.data && field.defaultValue && field.component !== 'date') {
                this.form.controls[field.dataProperty].setValue(field.defaultValue);
            }
        });
    }

    private buildReferenceListOnUsageIndex(field: DetailsViewField) : any[] {
        const referenceList = field.options?.referenceListIdField 
            ? fs.sort(field.options?.referenceList).by([
                { asc: l => l[field.options?.referenceListIdField]}
            ])
            :field.options?.referenceList;

        if (!field?.options?.usageIndexData || !field?.options?.usageIndexPeriodDateProperty || !field?.options?.usageIndexPeriodDays || !field?.options?.usageIndexThreshold) {
            return referenceList;
        }
        const usageIndexPeriodStart = new Date(new Date().setDate(new Date().getDate() - field.options.usageIndexPeriodDays));
        const usageFilter = field.options.usageIndexData.filter(r => new Date(r[field.options.usageIndexPeriodDateProperty])>=usageIndexPeriodStart)
        const primaryCandidates = l.countBy(usageFilter, field.dataProperty);
        let primaryCandidatesArray = [];  
        Object.keys(primaryCandidates).map(function(key){  
            if (key != 'null') {
                let result = { id: key, usageIndex:primaryCandidates[key]};
                result[field.options.referenceListIdField] = key;
                primaryCandidatesArray.push(result);
                return primaryCandidatesArray;  
            }
        });   
        let resultList = primaryCandidatesArray.filter(l => l.usageIndex >= field.options.usageIndexThreshold);
        resultList = fs.sort(resultList).by([
            { asc: l => l.id}
        ]);
        referenceList.forEach(element => {
            if (resultList.findIndex(p => p[field.options.referenceListIdField] === element[field.options.referenceListIdField]) === -1) {
                let result = {};
                result[field.options.referenceListIdField] = element[field.options.referenceListIdField];
                resultList.push(result);
            }
        });
        return resultList;
    }

    open(content) {
        this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
           if (result === 'delete') {
                const toolbarElement = this.toolbarElements.filter(t => t.defaultAction === ToolbarElementAction.Delete)[0];
                this.toolbarElementClick.emit({ toolbarElement: toolbarElement, data: this.data} as ToolbarElementWithData)
           }
        }, (reason) => { });
    }
}