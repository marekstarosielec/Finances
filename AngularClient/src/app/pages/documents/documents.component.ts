import { Component, OnInit } from '@angular/core';
import { DatasetState, Document,DocumentDatasetInfo,DocumentDatasetService,DocumentsService } from 'app/api/generated';
import { BehaviorSubject } from 'rxjs';
import { ActivatedRoute, Params, Router } from '@angular/router';
import * as _ from 'fast-sort';
import { take } from 'rxjs/operators';
import { DateChange } from 'app/shared/date-filter/date-filter.component';

@Component({
    selector: 'documents',
    moduleId: module.id,
    templateUrl: 'documents.component.html',
    styleUrls: ['./documents.component.scss']
})
export class DocumentsComponent implements OnInit{
    data: Document[];
    sortColumn: string = 'number';
    sortOrder: number = -1;
    dataSubject = new BehaviorSubject(null);
    filteredNumberOfRecords: number = 0;
    currentNumberOfRecords: number = 0;
    totalNumberOfRecords: number = 0;
    maximumVisibleNumberOfRecords: number = 100;
    documentsOpened: boolean;
    
    dateFromFilter: Date;
    dateToFilter: Date;
   
    constructor (
        private documentDatasetService: DocumentDatasetService, 
        private documentsService: DocumentsService, 
        private router: Router, 
        private route: ActivatedRoute) {}

    ngOnInit(){
        this.documentDatasetService.documentDatasetGet().pipe(take(1)).subscribe((info: DocumentDatasetInfo) => {
            this.documentsOpened = info.state == DatasetState.Opened;
            this.documentsService.documentsGet().pipe(take(1)).subscribe( (documents: Document[]) => {
                this.data = documents;
                this.totalNumberOfRecords = documents.length;
                this.filteredNumberOfRecords = documents.length;
                this.prepareView();
            });
        });
        this.route.queryParams.subscribe((qp: Params) => {
            this.maximumVisibleNumberOfRecords = qp.limit ?? 100;
            if (this.maximumVisibleNumberOfRecords < 0) this.maximumVisibleNumberOfRecords = 100;
            this.sortColumn = qp.sortColumn ?? 'number';
            this.sortOrder = qp.sortOrder ?? -1;
            if (qp.from)
                this.dateFromFilter = new Date(qp.from);
            else
                this.dateFromFilter = undefined;
            
            if (qp.to)
                this.dateToFilter = new Date(qp.to);
            else
                this.dateToFilter = undefined;
            this.prepareView();
        });
    }

    sort(column: string)
    {
        if (column === this.sortColumn){
            this.sortOrder = this.sortOrder * (-1);
        } else {
            this.sortColumn = column;
            this.sortOrder = -1;
        }
        this.router.navigate(['/documents'], { queryParams: { sortColumn: this.sortColumn, sortOrder: this.sortOrder }, queryParamsHandling: "merge" });
    }

    showAll() {
        this.router.navigate(['/documents'], { queryParams: { limit: 0 }, queryParamsHandling: "merge" });
    }

    showSome(){
        this.router.navigate(['/documents'], { queryParams: {  limit: 100 }, queryParamsHandling: "merge" });
    }

    prepareView() {
        if (!this.data)
        {
            this.currentNumberOfRecords = 0;
            this.filteredNumberOfRecords = 0;
            return;
        }

        let data = this.data; 

        if (this.dateFromFilter != undefined){
            data = data.filter(d => new Date(d.date) >= this.dateFromFilter);            
        }
        if (this.dateToFilter != undefined){
            data = data.filter(d => new Date(d.date) <= this.dateToFilter);            
        }
        
        this.filteredNumberOfRecords = data.length;
        
        if (this.sortOrder == -1)
            data = _.sort(data).by([
                { desc: t => t[this.sortColumn]},
                { asc: t => t.id}
            ]);
        else
            data = _.sort(data).by([
                { asc: t => t[this.sortColumn]},
                { asc: t => t.id}
            ]);
        
        if (this.maximumVisibleNumberOfRecords && this.maximumVisibleNumberOfRecords != 0) {
            data = data.slice(0, this.maximumVisibleNumberOfRecords);
        }
        this.currentNumberOfRecords = data.length;
        this.dataSubject.next(data);
    }

    selectRecord(id) {
        this.router.navigate([id], { relativeTo: this.route});
    }

    addNew() {
        this.router.navigate(["new"], { relativeTo: this.route});
    }

    openDocument(document: Document) {
        let fileName = document.number.toString();
        while (fileName.length < 5)
            fileName = '0' + fileName;
        fileName = 'MX' + fileName + '.' + document.extension;
        window.open("http://127.0.0.1:8080/" +fileName, "_blank", "noopener noreferrer");
    }

    filterByDate(event: DateChange) : void {
        this.dateFromFilter = event.dateFrom;
        this.dateToFilter = event.dateTo;
    }

    filterByDateApply() : void {
        let from: string;
        let to: string;
        if (this.dateFromFilter != undefined)
            from = this.dateFromFilter.getFullYear() + '-' + (this.dateFromFilter.getMonth()+1) + '-' + this.dateFromFilter.getDate();
        if (this.dateToFilter != undefined)
            to = this.dateToFilter.getFullYear() + '-' + (this.dateToFilter.getMonth()+1) + '-' + this.dateToFilter.getDate();
        
        this.router.navigate(['/documents'], { queryParams: {  
            from: from, 
            to: to, 
            }, queryParamsHandling: "merge" });
    }
}
