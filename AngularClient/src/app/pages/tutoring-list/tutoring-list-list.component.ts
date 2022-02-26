import { Component, OnInit } from '@angular/core';
import { TutoringListService } from 'app/api/generated';
import { GridColumn } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { forkJoin } from 'rxjs';

@Component({
    moduleId: module.id,
    template: `
        <list-page 
        name="tutoringList" 
        [columns]="columns" 
        [data]="data" 
        initialSortColumn="title" 
        initialSortOrder=1>
        </list-page>
    `
})
export class TutoringListListComponent implements OnInit{
    data: any[]; 
    columns: GridColumn[];
    
    constructor (private tutoringListService: TutoringListService) {}

    ngOnInit(){
        forkJoin([this.tutoringListService.tutoringListGet()])
        .pipe(take(1)).subscribe(([tutoringList]) =>{
            this.data = tutoringList;

            this.columns = [ 
                { title: 'Tytuł', dataProperty: 'title', component: 'text'},
                { title: 'Opis', dataProperty: 'description', component: 'text'}
            ];
         });
    }
}
