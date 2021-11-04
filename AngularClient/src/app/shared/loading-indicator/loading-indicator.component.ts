import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DatasetServiceFacade } from 'app/api/DatasetServiceFacade';
import { DatasetState } from 'app/api/generated';
import { Subscription } from 'rxjs';

@Component({
    selector: 'loading-indicator',
    templateUrl: 'loading-indicator.component.html',
    styleUrls: ['./loading-indicator.component.scss']
})

export class LoadingIndicatorComponent implements OnInit, OnDestroy{
    private dataServiceSubscription: Subscription;
    
    constructor(private router:Router,  private dataServiceFacade: DatasetServiceFacade) {
    }

    ngOnInit() {
        
        this.dataServiceSubscription = this.dataServiceFacade.getDatasetInfo().subscribe(t => {
            if (t.state === DatasetState.Opened) {
                this.router.navigate(["/dashboard"]);
            }
            else if (t.state === DatasetState.Closed) {
                this.router.navigate(["/opendataset"]);
            }
            else {
                setTimeout(() => this.dataServiceFacade.refreshDataset(), 1000);
            }
        });
    }

    ngOnDestroy()
    {
        this.dataServiceSubscription.unsubscribe();
    }
}
