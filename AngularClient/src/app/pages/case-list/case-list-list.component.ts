import { Component, OnInit } from '@angular/core';
import { CaseListService} from 'app/api/generated';
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { forkJoin } from 'rxjs';

@Component({
    moduleId: module.id,
    template: `
        <list-page 
        name="case" 
        [columns]="columns" 
        [data]="data" 
        initialSortColumn="name" 
        initialSortOrder=1>
        </list-page>
    `
})
export class CaseListListComponent implements OnInit{
    data: any[]; 
    columns: GridColumn[];
    
    constructor (private caseListService: CaseListService) {}

    ngOnInit(){
        forkJoin([this.caseListService.caseListGet()])
        .pipe(take(1)).subscribe(([caseList]) =>{
            this.data = caseList;

            this.columns = [ 
                { title: 'Nazwa', dataProperty: 'name', component: 'text'}
            ];
        });
    }
}
