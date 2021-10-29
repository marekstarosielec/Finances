import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DatasetServiceFacade } from 'app/api/DatasetServiceFacade';
import { DatasetInfo, DatasetService, DatasetState } from 'app/api/generated';
import { Observable, Subscription, interval } from 'rxjs';

import { mergeMap, take } from 'rxjs/operators';

@Component({
    selector: 'open-dataset',
    moduleId: module.id,
    templateUrl: 'open-dataset.component.html'
})

export class OpenDatasetComponent implements OnInit, OnDestroy{

    private dataServiceSubscription: Subscription;
    public isOpening: boolean;
    form = new FormGroup({
        password: new FormControl('', [Validators.required, Validators.minLength(8)])
    });
        
    submit(){
        if(!this.form.valid){
            return;
        }
        this.datasetServiceFacade.openDataset();
    }
      
    constructor(private router: Router, private datasetServiceFacade: DatasetServiceFacade) {
        this.dataServiceSubscription = this.datasetServiceFacade.getDatasetInfo().subscribe((datasetInfo: DatasetInfo) => {
            if (datasetInfo.state === DatasetState.Opening)
            {
                this.isOpening = true;
                let innerSub = interval(1000).pipe(
                    mergeMap(() => {
                        this.datasetServiceFacade.refreshDataset();
                        return this.datasetServiceFacade.getDatasetInfo()
                    })
                    )
                    .subscribe(data => {
                         if (data.state === DatasetState.Open)
                        {
                            innerSub.unsubscribe();
                            this.router.navigate(['/dashboard']);
        
                        }
                    });
            }
        });
    }

    ngOnInit(){
    }

    ngOnDestroy() {
        this.dataServiceSubscription.unsubscribe();
    }
}
