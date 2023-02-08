import { Component } from '@angular/core';
import { DatasetServiceFacade } from './api/DatasetServiceFacade';
import { DatasetInfo } from './api/generated';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent{
  constructor(private datasetServiceFacade: DatasetServiceFacade) {
     this.datasetServiceFacade.getDatasetInfo().subscribe((datasetInfo: DatasetInfo) => {
        console.log("AppComponent datasetInfo:", datasetInfo);
    });
  }
}
