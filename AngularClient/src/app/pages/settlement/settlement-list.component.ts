import { Component, OnInit } from '@angular/core';
import { GridColumn, RowClickedData } from 'app/shared/grid/grid.component';
import { take } from 'rxjs/operators';
import { forkJoin } from 'rxjs';
import { SettlementService } from 'app/api/generated';
import { Summary } from '../list-page/list-page.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup } from '@angular/forms';

@Component({
    moduleId: module.id,
    template: `
        <list-page 
        name="settlement" 
        [columns]="columns" 
        [data]="data" 
        initialSortColumn="date" 
        initialSortOrder=-1
        (rowClicked)="rowClickedEvent($event)"
        [summaries]="summaries">
        </list-page>
    `
})
export class SettlementListComponent implements OnInit{
    data: any[]; 
    columns: GridColumn[];
    summaries: Summary[] = [];
    
    constructor (private settlementService: SettlementService, 
        private router: Router, 
        private route: ActivatedRoute) {}

    ngOnInit(){
        forkJoin([this.settlementService.settlementGet()])
        .pipe(take(1)).subscribe(([settlementList]) =>{
            

            this.data = settlementList;
            this.columns = [ 
                { title: 'Data', dataProperty: 'title', component: 'text'},
                { title: 'Zysk', dataProperty: 'revenue', pipe: 'number', component: 'text', alignment: 'right'},
                { title: 'Komentarz', dataProperty: 'comment', component: 'text'}
            ];
         });
    }

    rowClickedEvent(rowClickedData: RowClickedData) {
        if (!rowClickedData.row['transaction']) {
            this.router.navigate([rowClickedData.row['id']], { relativeTo: this.route});
        } else {
            this.router.navigate(['transactions', rowClickedData.row['id']]);
        } 
    }
}
