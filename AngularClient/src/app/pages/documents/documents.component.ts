import { Component, OnInit } from '@angular/core';
import { DatasetState, Document,DocumentDatasetInfo,DocumentDatasetService,DocumentsService } from 'app/api/generated';
import { BehaviorSubject, timer } from 'rxjs';
import * as _ from 'lodash';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { DocumentDatasetServiceFacade } from 'app/api/documentDatasetServiceFacade';

@Component({
    selector: 'documents',
    moduleId: module.id,
    templateUrl: 'documents.component.html',
    styleUrls: ['./documents.component.scss']
})
export class DocumentsComponent implements OnInit{
    data: Document[];
    numberOfRecords: number = 100;
    sortColumn: string = 'number';
    sortOrder: number = -1;
    dataSubject = new BehaviorSubject(null);
    showAllRecords: boolean = false;
    documentsOpened: boolean;
    constructor (private documentDatasetService: DocumentDatasetService, private documentsService: DocumentsService, private router: Router, private route: ActivatedRoute) {}

    ngOnInit(){
        this.documentDatasetService.documentDatasetGet().pipe(take(1)).subscribe((info: DocumentDatasetInfo) => {
            this.documentsOpened = info.state == DatasetState.Opened;
            this.documentsService.documentsGet().pipe(take(1)).subscribe( (documents: Document[]) => {
                this.data = documents;
                this.prepareView();
            });
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
        this.prepareView();
    }

    showAll() {
        this.showAllRecords = true;
        this.prepareView();
    }

    showSome(){
        this.showAllRecords = false;
        this.prepareView();
    }

    prepareView() {
        let data = this.data;
        data = data.sort((a,b) => (a[this.sortColumn] > b[this.sortColumn]) ? this.sortOrder : ((b[this.sortColumn] > a[this.sortColumn]) ? this.sortOrder * (-1) : 0))
        if (!this.showAllRecords) {
            data = data.slice(0, this.numberOfRecords);
        }
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
}
