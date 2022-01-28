import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from "@angular/core";
import { FormControl, FormGroup, ValidatorFn, Validators } from "@angular/forms";
import * as l from 'lodash';
import * as fs from 'fast-sort';
import { FormattedAmountPipe } from "app/pipes/formattedAmount.component";
import { ToolbarElement, ToolbarElementAction, ToolbarElementWithData } from "../models/toolbar";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

export interface DetailsViewField {
    title: string;
    dataProperty: string;
    component: string;
    readonly?: boolean;
    options?: any;
    required?: boolean;
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
        this.prepareView();
    }

    private _data: any;
    get data(): any {
        return this._data;
    }
    @Input()
    set data(value: any) {
        this._data = value;
        this.prepareView();
    }
    @Input() public toolbarElements: ToolbarElement[];
    @Output() public toolbarElementClick = new EventEmitter<ToolbarElementWithData>();
    toolbarElementAction: typeof ToolbarElementAction = ToolbarElementAction;
    numberRegEx = /\-?\d*\.?\d{1,2}/;

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
  
    }

    toolbarElementClicked(toolbarElement: ToolbarElement) {
        console.log(toolbarElement);
        if (toolbarElement.defaultAction === ToolbarElementAction.SaveChanges) {
            if (!this.form.valid) {
                return;
            }
            
            let data = this.data ?? {};

            this.viewDefinition.fields.forEach((field : DetailsViewField) => {
                if (field.component === 'date'){
                    let month =  '0' + this.form.value.date.month;
                    if (month.length > 2) {
                        month = month.substring(month.length-2);
                    }
                    let day =  '0' + this.form.value.date.day;
                    if (day.length > 2) {
                        day = day.substring(day.length-2);
                    }
                    data[field.dataProperty] = this.form.value.date.year + '-' + month + '-' + day + 'T00:00:00Z';
                } else if (field.component === 'amount') {
                    data[field.dataProperty] = +(this.form.controls[field.dataProperty].value.replace(",",".").replace(" ",""));
                    if (field.options?.currencyDataProperty) {
                        data[field.options?.currencyDataProperty] = this.form.controls[field.options?.currencyDataProperty].value;
                    }
                } else {
                    data[field.dataProperty] = this.form.controls[field.dataProperty].value;
                }
            });
            this.toolbarElementClick.emit({ toolbarElement: toolbarElement, data: data} as ToolbarElementWithData);
        } 
    }

    private prepareView() {
        let controls = {};
        if (!this.viewDefinition?.fields) {
            return;
        }
        let data = {};
        this.viewDefinition.fields.forEach((field : DetailsViewField) => {
            const validators : ValidatorFn[] = [];
            if (field.required && field.component !== 'list') {
                validators.push(Validators.required);
            }

            if (field.component==='amount') {
                validators.push(Validators.pattern(this.numberRegEx))
                if (field.options?.currencyDataProperty) {
                    controls[field.options.currencyDataProperty] = new FormControl(undefined, []);
                    data[field.options.currencyDataProperty] = this.data ? this.data[field.options.currencyDataProperty] : '';
                }
                if (this.data) {
                    const amountPipe = new FormattedAmountPipe();
                    data[field.dataProperty] = amountPipe.transform(this.data[field.dataProperty]);
                } else {
                    data[field.dataProperty] = '';
                }
                controls[field.dataProperty] = new FormControl(undefined, validators);
            }
            else if (field.component === 'date') {
                let date = new Date();
                if (this.data) { 
                    date = new Date(this.data[field.dataProperty]);
                }
                const dtpDate = {year: date.getFullYear(), month:date.getMonth()+1, day: date.getDate()};
                data[field.dataProperty] = dtpDate;
                controls[field.dataProperty] = new FormControl(undefined, validators);  
            } else if (field.component === 'list') {
                if (!field.options) {
                    field.options = {};
                }
                field.options.referenceList = this.buildReferenceListOnUsageIndex(field);
                controls[field.dataProperty] = new FormControl(undefined, validators);
                data[field.dataProperty] = this.data ? this.data[field.dataProperty] : '';
            } else if (field.component === 'text') {
                controls[field.dataProperty] = new FormControl(undefined, validators);
                data[field.dataProperty] = this.data ? this.data[field.dataProperty] : '';
            } else if (field.component === 'multiline-text') {
                controls[field.dataProperty] = new FormControl(undefined, validators);
                data[field.dataProperty] = this.data ? this.data[field.dataProperty] : '';
            }
        });
        this.form = new FormGroup(controls);
        this.form.setValue(data);
    }

    private buildReferenceListOnUsageIndex(field: DetailsViewField) : any[] {
        if (!field?.options?.usageIndexData || !field?.options?.usageIndexPeriodDateProperty || !field?.options?.usageIndexPeriodDays || !field?.options?.usageIndexThreshold) {
            return field.options?.referenceList;
        }
        const usageIndexPeriodStart = new Date(new Date().setDate(new Date().getDate() - field.options.usageIndexPeriodDays));
        const usageFilter = field.options.usageIndexData.filter(r => new Date(r[field.options.usageIndexPeriodDateProperty])>=usageIndexPeriodStart )
        const primaryCandidates = l.countBy(usageFilter, field.dataProperty);
        let primaryCandidatesArray = [];  
        Object.keys(primaryCandidates).map(function(key){  
            let result = { id: key, usageIndex:primaryCandidates[key]};
            result[field.options.referenceListIdField] = key;
            primaryCandidatesArray.push(result);
            return primaryCandidatesArray;  
        });   
        let resultList = primaryCandidatesArray.filter(l => l.usageIndex >= field.options.usageIndexThreshold);
        resultList = fs.sort(resultList).by([
            { asc: l => l.id}
        ]);
        field.options?.referenceList.forEach(element => {
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