import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { DocumentsService, Document, CaseListService, FileService, DecompressFileResult, SettlementService } from "app/api/generated";
import { DetailsViewComponent, DetailsViewDefinition, DetailsViewField, DetailsViewFieldListOptions } from "app/shared/details-view/details-view.component";
import { ToolbarElement, ToolbarElementAction, ToolbarElementWithData } from "app/shared/models/toolbar";
import { forkJoin, Subscription } from "rxjs";
import { take } from "rxjs/operators";
import { Location } from '@angular/common';
import { SettingsService } from "app/api/settingsService";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
    moduleId: module.id,
    template: `
        <details-view 
        #detailsView
        [viewDefinition]="viewDefinition" 
        [data]="data"  
        [toolbarElements]="toolbarElements" 
        (toolbarElementClick)="toolbarElementClick($event)"></details-view>

        <ng-template #password let-modal>
            <div class="modal-header">
                <h4 class="modal-title" id="modal-basic-title">Podaj hasło</h4>
            </div>
            <div class="modal-body">
                <input 
                    #password
                    id="password" 
                    type="password" 
                    class="form-control">
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-light" (click)="modal.close('')">Anuluj</button>
                <button type="button" class="btn btn-outline-danger" (click)="modal.close(password.value)">Otwórz</button>
            </div>
        </ng-template>
    `
})

export class DocumentComponent implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: any;
    toolbarElements: ToolbarElement[] = [];
    private routeSubscription: Subscription;
    @ViewChild('detailsView', { static: true }) component: ElementRef<DetailsViewComponent>;
    @ViewChild('password', { static: true }) password: ElementRef;
    documents: Document[];

    constructor(private documentsService: DocumentsService,  private caseListService: CaseListService,
        private route: ActivatedRoute, 
        private settingService: SettingsService,
        private fileService: FileService,
        private settlementService: SettlementService,
        private modalService: NgbModal,
        private location: Location) {  
    }

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe((params: Params) => {
            forkJoin([
                this.documentsService.documentsGet(), this.caseListService.caseListGet(), this.settlementService.settlementGet()])
                .pipe(take(1)).subscribe(([documents, caseList, settlementList]) => {
                    this.documents = documents as Document[];
                    this.data = documents.filter(t => t.id == params['id'])[0];
                    let allDocumentNumbers = documents.map(item => item.number).filter(val => !isNaN(val));
                    const maxNumber = Math.max(...allDocumentNumbers) + 1; 
                    this.viewDefinition = {
                        fields: [
                            { title: 'Numer', dataProperty: 'number', component: 'text', required: true, defaultValue: maxNumber } as DetailsViewField,
                            { title: 'Data', dataProperty: 'date', component: 'date', required: true} as DetailsViewField,
                            { title: 'Firma', dataProperty: 'company', component: 'text'} as DetailsViewField,
                            { title: 'Ilość stron', dataProperty: 'pages', component: 'text'} as DetailsViewField,
                            { title: 'Opis', dataProperty: 'description', component: 'text'} as DetailsViewField,
                            { title: 'Kategoria', dataProperty: 'category', component: 'text'} as DetailsViewField,
                            { title: 'Faktura', dataProperty: 'invoiceNumber', component: 'text'} as DetailsViewField,
                            { title: 'Osoba', dataProperty: 'person', component: 'text'} as DetailsViewField,
                            { title: 'Samochód', dataProperty: 'car', component: 'text'} as DetailsViewField,
                            { title: 'Rzecz', dataProperty: 'relatedObject', component: 'text'} as DetailsViewField,
                            { title: 'Gwarancja', dataProperty: 'guarantee', component: 'text'} as DetailsViewField,
                            { title: 'Sprawa', dataProperty: 'caseName', component: 'list', required: false, options: { referenceList: caseList, referenceListIdField: 'name'} as DetailsViewFieldListOptions} as DetailsViewField,
                            { title: 'Rozliczenie', dataProperty: 'settlement', component: 'list', required: false, options: { referenceList: settlementList, referenceListIdField: 'title' } as DetailsViewFieldListOptions} as DetailsViewField,
                        ]
                    };

                    this.toolbarElements.push(
                        { name: 'save', title: 'Zapisz', defaultAction: ToolbarElementAction.SaveChanges},
                        { name: 'delete', title: 'Usuń', defaultAction: ToolbarElementAction.Delete},
                        { name: 'open', image: 'nc-image'},
                        { name: 'phone', title: 'Telefon', align: 'right'},
                        { name: 'internet', title: 'Internet', align: 'right'},
                        { name: 'ciklumTools', title: 'Ciklum narzędzia', align: 'right'},
                        { name: 'fuel', title: 'Paliwo', align: 'right'},
                        { name: 'mazda', title: 'Mazda', align: 'right'},
                        { name: 'invoice', title: 'Faktura', align: 'right'},
                        { name: 'dra', title: 'ZUS DRA', align: 'right'},
                        { name: 'jpk', title: 'JPK', align: 'right'},
                        { name: 'jpk_upo', title: 'UPO JPK', align: 'right'},
                        );
                });
        });
    }

    ngOnDestroy(){
        this.routeSubscription.unsubscribe();
    }

    toolbarElementClick(toolbarElementWithData: ToolbarElementWithData) {
        const component = this.component as unknown as DetailsViewComponent;
        if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.SaveChanges) {
            if (this.data) {
                this.documentsService.documentsDocumentPut(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            } else {
                this.documentsService.documentsDocumentPost(toolbarElementWithData.data).pipe(take(1)).subscribe(() =>
                {
                    this.location.back();
                });
            }
        } else if (toolbarElementWithData.toolbarElement.defaultAction === ToolbarElementAction.Delete) {
            this.documentsService.documentsDocumentIdDelete(toolbarElementWithData.data.id).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        } else if (toolbarElementWithData.toolbarElement.name === 'invoice') {
            component.form.controls['pages'].setValue(1);
        } else if (toolbarElementWithData.toolbarElement.name === 'open') {
            if (!this.settingService.CurrentPassword)
            {
                this.modalService.open(this.password, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
                    this.settingService.CurrentPassword = result;
                    this.openFile();
                }, (reason) => { });
            } else {
                this.openFile();
            }
        } else if (toolbarElementWithData.toolbarElement.name === 'mazda') {
            let defaultDate = new Date();
            let invoiceNumberDate='XX/XX'
            const lastInvoice = this.documents.filter(t => t.company?.toUpperCase() == 'SANTANDER CONSUMER MULTIRENT' && t.category?.toUpperCase()==='FAKTURA OTRZYMANA').sort((d1,d2) => d1.date < d2.date ? 1 : -1)[0];
            if (lastInvoice) {
                let lastDate = new Date(lastInvoice.date);
                lastDate.setMonth(lastDate.getMonth() + 1);
                defaultDate = lastDate;

                let month = (lastDate.getMonth() + 1).toString();
                let year = (lastDate.getFullYear()).toString().slice(2);
                
                invoiceNumberDate=month + '/' + year;
                if (invoiceNumberDate.length < 5) {
                    invoiceNumberDate = '0' + invoiceNumberDate;
                }
            }
            component.form.controls['date'].setValue({ year: defaultDate.getFullYear(), month: defaultDate.getMonth() + 1, day: defaultDate.getDate()});
            component.form.controls['company'].setValue('Santander Consumer Multirent');
            component.form.controls['pages'].setValue(1);
            component.form.controls['category'].setValue('Faktura otrzymana');
            component.form.controls['invoiceNumber'].setValue('RLXXXXX/' + invoiceNumberDate);
            component.form.controls['car'].setValue('GA839ES');
        } else if (toolbarElementWithData.toolbarElement.name === 'fuel') {
            component.form.controls['pages'].setValue(1);
            component.form.controls['description'].setValue('Paliwo');
            component.form.controls['category'].setValue('Faktura otrzymana');
        } else if (toolbarElementWithData.toolbarElement.name === 'ciklumTools') {
            let defaultDate = new Date();
            const lastInvoice = this.documents.filter(t => t.company?.toUpperCase() == 'CIKLUM' && t.category?.toUpperCase()==='FAKTURA OTRZYMANA').sort((d1,d2) => d1.date < d2.date ? 1 : -1)[0];
            if (lastInvoice) {
                let lastDate = new Date(lastInvoice.date);
                lastDate.setDate(1);
                lastDate.setMonth(lastDate.getMonth() + 2);
                lastDate.setDate(0);
                defaultDate = lastDate;
            }
            component.form.controls['date'].setValue({ year: defaultDate.getFullYear(), month: defaultDate.getMonth() + 1, day: defaultDate.getDate()});
            component.form.controls['company'].setValue('Ciklum');
            component.form.controls['pages'].setValue(1);
            component.form.controls['category'].setValue('Faktura otrzymana');
            component.form.controls['invoiceNumber'].setValue('PL0XXXX');
            component.form.controls['description'].setValue('Ciklum narzędzia');
        } else if (toolbarElementWithData.toolbarElement.name === 'internet') {
            let defaultDate = new Date();
            let invoiceNumberDate='XX/XXXX'
            const lastInvoice = this.documents.filter(t => t.company?.toUpperCase() == 'WAVE' && t.category?.toUpperCase()==='FAKTURA OTRZYMANA').sort((d1,d2) => d1.date < d2.date ? 1 : -1)[0];
            if (lastInvoice) {
                let lastDate = new Date(lastInvoice.date);
                lastDate.setMonth(lastDate.getMonth() + 1);
                defaultDate = lastDate;

                let month = (lastDate.getMonth() + 1).toString();
                let year = (lastDate.getFullYear()).toString();
                
                invoiceNumberDate=month + '/' + year;
                if (invoiceNumberDate.length < 7) {
                    invoiceNumberDate = '0' + invoiceNumberDate;
                }
            }
            component.form.controls['date'].setValue({ year: defaultDate.getFullYear(), month: defaultDate.getMonth() + 1, day: defaultDate.getDate()});
            component.form.controls['company'].setValue('Wave');
            component.form.controls['pages'].setValue(1);
            component.form.controls['category'].setValue('Faktura otrzymana');
            component.form.controls['invoiceNumber'].setValue('1/' + invoiceNumberDate + '/F/6813');
            component.form.controls['description'].setValue('Internet');
        } else if (toolbarElementWithData.toolbarElement.name === 'phone') {
            let defaultDate = new Date();
            let invoiceNumberDate='XX/XX'
            const lastInvoice = this.documents.filter(t => t.company?.toUpperCase() == 'PLAY' && t.category?.toUpperCase()==='FAKTURA OTRZYMANA').sort((d1,d2) => d1.date < d2.date ? 1 : -1)[0];
            if (lastInvoice) {
                let lastDate = new Date(lastInvoice.date);
                lastDate.setMonth(lastDate.getMonth() + 1);
                defaultDate = lastDate;
                let month = (lastDate.getMonth() + 1).toString();
                let year = (lastDate.getFullYear()).toString().slice(2);
                
                invoiceNumberDate=month + '/' + year;
                if (invoiceNumberDate.length < 5) {
                    invoiceNumberDate = '0' + invoiceNumberDate;
                }
            }
            component.form.controls['date'].setValue({ year: defaultDate.getFullYear(), month: defaultDate.getMonth() + 1, day: defaultDate.getDate()});
            component.form.controls['company'].setValue('Play');
            component.form.controls['category'].setValue('Faktura otrzymana');
            component.form.controls['invoiceNumber'].setValue('F/XXXXXXXX/' + invoiceNumberDate);
            component.form.controls['description'].setValue('Telefon');
        } else if (toolbarElementWithData.toolbarElement.name === 'dra') {
            let defaultDate = new Date();
            let descriptionAppendix ='XXXX-XX'
            const last = this.documents.filter(t => t.company?.toUpperCase() == 'ZUS' && t.description?.toUpperCase().startsWith('DRA')).sort((d1,d2) => d1.date < d2.date ? 1 : -1)[0];
            if (last) {
                let lastDate = new Date(last.date);
                lastDate.setMonth(lastDate.getMonth() + 1);
                defaultDate = lastDate;
                let month = (lastDate.getMonth() + 1).toString();
                if (month.length < 2){
                    month = '0' + month;
                }
                let year = (lastDate.getFullYear()).toString();
                
                descriptionAppendix=year+'-'+month;
            }
            component.form.controls['date'].setValue({ year: defaultDate.getFullYear(), month: defaultDate.getMonth() + 1, day: defaultDate.getDate()});
            component.form.controls['company'].setValue('ZUS');
            component.form.controls['description'].setValue('DRA ' + descriptionAppendix);
        }else if (toolbarElementWithData.toolbarElement.name === 'jpk') {
            let defaultDate = new Date();
            let descriptionAppendix ='XXXX-XX'
            const last = this.documents.filter(t => t.company?.toUpperCase() == 'Urząd Skarbowy' && t.description?.toUpperCase().startsWith('JPK')).sort((d1,d2) => d1.date < d2.date ? 1 : -1)[0];
            if (last) {
                let lastDate = new Date(last.date);
                lastDate.setMonth(lastDate.getMonth() + 1);
                defaultDate = lastDate;
                let month = (lastDate.getMonth() + 1).toString();
                if (month.length < 2){
                    month = '0' + month;
                }
                let year = (lastDate.getFullYear()).toString();
                
                descriptionAppendix=year+'-'+month;
            }
            component.form.controls['date'].setValue({ year: defaultDate.getFullYear(), month: defaultDate.getMonth() + 1, day: defaultDate.getDate()});
            component.form.controls['company'].setValue('Urząd Skarbowy');
            component.form.controls['description'].setValue('JPK ' + descriptionAppendix);
            component.form.controls['settlement'].setValue(descriptionAppendix);
        }else if (toolbarElementWithData.toolbarElement.name === 'jpk_upo') {
            let defaultDate = new Date();
            let descriptionAppendix ='XXXX-XX'
            const last = this.documents.filter(t => t.company?.toUpperCase() == 'Urząd Skarbowy' && t.description?.toUpperCase().startsWith('UPO JPK')).sort((d1,d2) => d1.date < d2.date ? 1 : -1)[0];
            if (last) {
                let lastDate = new Date(last.date);
                lastDate.setMonth(lastDate.getMonth() + 1);
                defaultDate = lastDate;
                let month = (lastDate.getMonth() + 1).toString();
                if (month.length < 2){
                    month = '0' + month;
                }
                let year = (lastDate.getFullYear()).toString();
                
                descriptionAppendix=year+'-'+month;
            }
            component.form.controls['date'].setValue({ year: defaultDate.getFullYear(), month: defaultDate.getMonth() + 1, day: defaultDate.getDate()});
            component.form.controls['company'].setValue('Urząd Skarbowy');
            component.form.controls['description'].setValue('UPO JPK ' + descriptionAppendix);
            component.form.controls['settlement'].setValue(descriptionAppendix);
        }
    }

    openFile() {
        if (!this.settingService.CurrentPassword){
            return;
        }
        
        let number = this.data['number'];
        this.fileService.filePost({ number: number, password: this.settingService.CurrentPassword}).pipe(take(1)).subscribe((result: DecompressFileResult) => {
            window.open("http://127.0.0.1:8080" + result.path, "_blank", "noopener noreferrer");
        });
    }
}
