import { formatDate } from "@angular/common";
import { Component, Input, OnDestroy, OnInit } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { __classPrivateFieldSet } from "tslib";

export interface DetailsViewField {
    title: string;
    dataProperty: string;
    component: string;
    readonly?: boolean;
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


    ngOnInit() {

    }

    ngOnDestroy() {
  
    }

    private prepareView() {
        let controls = {};
        if (!this.viewDefinition?.fields || !this.data) {
            return;
        }
        let data = {};
        this.viewDefinition.fields.forEach((field : DetailsViewField) => {
            if (field.component === 'date') {
                console.log(formatDate(this.data[field.dataProperty], 'yyyy-MM-dd', 'en'));
                controls[field.dataProperty] = new FormControl(formatDate(this.data[field.dataProperty], 'yyyy-MM-dd', 'en'), []);
            } else {
                controls[field.dataProperty] = new FormControl(undefined, []);
            }
            data[field.dataProperty] = this.data[field.dataProperty];
        })
        this.form = new FormGroup(controls);
        this.form.setValue(data);
    }
}