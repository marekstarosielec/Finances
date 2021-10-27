import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DatasetServiceFacade } from 'app/api/DatasetServiceFacade';
import { DatasetService } from 'app/api/generated';

@Component({
    selector: 'close-dataset',
    moduleId: module.id,
    templateUrl: 'close-dataset.component.html'
})

export class CloseDatasetComponent implements OnInit{

    form = new FormGroup({
        password: new FormControl('', [Validators.required, Validators.minLength(8)])
    });
        
    submit(){
        if(!this.form.valid){
            return;
        }
        this.datasetServiceFacade.closeDataset();
    }
      
    constructor(private datasetServiceFacade: DatasetServiceFacade) {
        
    }

    ngOnInit(){
    }
}
