import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DatasetServiceFacade } from 'app/api/DatasetServiceFacade';
import { DatasetState } from 'app/api/generated';
import { Subscription } from 'rxjs';

@Component({
    selector: 'open-dataset',
    moduleId: module.id,
    templateUrl: 'open-dataset.component.html'
})

export class OpenDatasetComponent implements OnInit, OnDestroy{
    private dataServiceSubscription: Subscription;
    
    form = new FormGroup({
        password: new FormControl('', [Validators.required, Validators.minLength(8)])
    });
        
    constructor(private router: Router, private datasetServiceFacade: DatasetServiceFacade) {
    }

    ngOnInit(){
        this.dataServiceSubscription = this.datasetServiceFacade.getDatasetInfo().subscribe(i =>{
            if (i.state == DatasetState.Opening) 
                this.router.navigate(['/loading']);
        });
    }

    ngOnDestroy() {
        this.dataServiceSubscription.unsubscribe();
    }

    submit(){
        if(!this.form.valid){
            return;
        }
        this.datasetServiceFacade.openDataset();
    }
}
