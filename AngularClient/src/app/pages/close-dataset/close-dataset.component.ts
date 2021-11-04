import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DatasetServiceFacade } from 'app/api/DatasetServiceFacade';
import { DatasetState } from 'app/api/generated';
import { Subscription } from 'rxjs';

@Component({
    selector: 'close-dataset',
    moduleId: module.id,
    templateUrl: 'close-dataset.component.html'
})

export class CloseDatasetComponent implements OnInit{
    private dataServiceSubscription: Subscription;
    public todayDate : Date = new Date();
  
    form = new FormGroup({
        password: new FormControl('', [Validators.required, Validators.minLength(8)])
    });
           
    constructor(private router: Router, private datasetServiceFacade: DatasetServiceFacade) {
    }

    ngOnInit(){
        this.dataServiceSubscription = this.datasetServiceFacade.getDatasetInfo().subscribe(i =>{
            if (i.state == DatasetState.Closing) 
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
        this.datasetServiceFacade.closeDataset(this.form.value.password);
    }
}
