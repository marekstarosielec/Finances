import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DatasetServiceFacade } from 'app/api/DatasetServiceFacade';
import { DatasetState } from 'app/api/generated';
import { Subscription } from 'rxjs';

@Component({
    selector: 'close-dataset',
    moduleId: module.id,
    templateUrl: 'close-dataset.component.html',
    styleUrls:['close-dataset.component.scss']
})

export class CloseDatasetComponent implements OnInit{
    private dataServiceSubscription: Subscription;
    public todayDate : Date = new Date();
    error:string;
    form = new FormGroup({
        password: new FormControl('', [Validators.required, Validators.minLength(8)]),
        password2: new FormControl('', [Validators.required, Validators.minLength(8)])
    });
           
    constructor(private router: Router, private datasetServiceFacade: DatasetServiceFacade) {
    }

    ngOnInit(){
        this.error = '';
        this.dataServiceSubscription = this.datasetServiceFacade.getDatasetInfo().subscribe(result =>{
            if (result.state == DatasetState.Closing) 
                this.router.navigate(['/loading']);
            if (result.state == DatasetState.ClosingError) 
                this.error = result.error;
        });
    }

    ngOnDestroy() {
        this.dataServiceSubscription.unsubscribe();
    }

    submit(){
        if (this.form.controls['password'].value != this.form.controls['password2'].value) {
            this.form.controls['password2'].setErrors({ differentPassword: true })
            return;
        }
        else {
            this.form.controls['password2'].setErrors(null);
        }
        if(!this.form.valid){
            return;
        }
        this.datasetServiceFacade.closeDataset(this.form.value.password);
    }
}
