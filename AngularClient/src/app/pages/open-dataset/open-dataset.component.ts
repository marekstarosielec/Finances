import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DatasetService } from 'app/api/generated';

@Component({
    selector: 'open-dataset',
    moduleId: module.id,
    templateUrl: 'open-dataset.component.html'
})

export class OpenDatasetComponent implements OnInit{

    form = new FormGroup({
        password: new FormControl('', [Validators.required, Validators.minLength(8)])
    });
        
    submit(){
        if(!this.form.valid){
            return;
        }
        this.datasetService.datasetOpenPost().subscribe(t => {
            console.log(t);
        })
    }
      
    constructor(private datasetService: DatasetService) {
        
    }

    ngOnInit(){
    }
}
