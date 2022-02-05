import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Params } from "@angular/router";
import { DocumentsService, TransactionsService } from "app/api/generated";
import { DetailsViewDefinition, DetailsViewField, DetailsViewFieldListOptions } from "app/shared/details-view/details-view.component";
import { ToolbarElementAction, ToolbarElementWithData } from "app/shared/models/toolbar";
import { forkJoin, Subscription } from "rxjs";
import { take } from "rxjs/operators";
import { Location } from '@angular/common';

@Component({
    moduleId: module.id,
    template: `
        <details-view [viewDefinition]="viewDefinition" [data]="data"  (toolbarElementClick)="toolbarElementClick($event)"></details-view>
    `
})

export class DocumentComponent implements OnInit, OnDestroy {
    viewDefinition: DetailsViewDefinition;
    data: any;
    private routeSubscription: Subscription;
    
    constructor(private documentsService: DocumentsService, 
        private route: ActivatedRoute, 
        private location: Location) {  
    }

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe((params: Params) => {
            forkJoin([
                this.documentsService.documentsGet()])
                .pipe(take(1)).subscribe(([documents]) => {
                    this.data = documents.filter(t => t.id == params['id'])[0];
                    let allDocumentNumbers = documents.map(item => item.number).filter(val => !isNaN(val));
                    const maxNumber = Math.max(...allDocumentNumbers) + 1; 
                    if (params['type'] === 'phone') {
                        let defaultDate = '';
                        let invoiceNumberDate='XX/XX'
                        const lastInvoice = documents.filter(t => t.company?.toUpperCase() == 'PLAY' && t.category?.toUpperCase()==='FAKTURA OTRZYMANA').sort((d1,d2) => d1.date < d2.date ? 1 : -1)[0];
                        if (lastInvoice) {
                            let lastDate = new Date(lastInvoice.date);
                            lastDate.setMonth(lastDate.getMonth() + 1);
                            defaultDate = lastDate.toISOString();
                            let month = (lastDate.getMonth() + 1).toString();
                            let year = (lastDate.getFullYear()).toString().slice(2);
                            
                            invoiceNumberDate=month + '/' + year;
                            if (invoiceNumberDate.length < 5) {
                                invoiceNumberDate = '0' + invoiceNumberDate;
                            }
                        }
                        this.viewDefinition = {
                            fields: [
                                { title: 'Numer', dataProperty: 'number', component: 'text', required: true, defaultValue: maxNumber } as DetailsViewField,
                                { title: 'Data', dataProperty: 'date', component: 'date', required: true, defaultValue: defaultDate} as DetailsViewField,
                                { title: 'Firma', dataProperty: 'company', component: 'text', defaultValue: 'Play'} as DetailsViewField,
                                { title: 'Ilość stron', dataProperty: 'pages', component: 'text', defaultValue: '1'} as DetailsViewField,
                                { title: 'Opis', dataProperty: 'description', component: 'text', defaultValue: 'Telefon'} as DetailsViewField,
                                { title: 'Kategoria', dataProperty: 'category', component: 'text', defaultValue: 'Faktura otrzymana'} as DetailsViewField,
                                { title: 'Faktura', dataProperty: 'invoiceNumber', component: 'text', defaultValue: 'F/XXXXXXXX/' + invoiceNumberDate} as DetailsViewField
                            ]
                        };
                    } else if (params['type'] === 'internet') {
                        let defaultDate = '';
                        let invoiceNumberDate='XX/XXXX'
                        const lastInvoice = documents.filter(t => t.company?.toUpperCase() == 'WAVE' && t.category?.toUpperCase()==='FAKTURA OTRZYMANA').sort((d1,d2) => d1.date < d2.date ? 1 : -1)[0];
                        if (lastInvoice) {
                            let lastDate = new Date(lastInvoice.date);
                            lastDate.setMonth(lastDate.getMonth() + 1);
                            defaultDate = lastDate.toISOString();

                            let month = (lastDate.getMonth() + 1).toString();
                            let year = (lastDate.getFullYear()).toString();
                            
                            invoiceNumberDate=month + '/' + year;
                            if (invoiceNumberDate.length < 7) {
                                invoiceNumberDate = '0' + invoiceNumberDate;
                            }
                        }
                        this.viewDefinition = {
                            fields: [
                                { title: 'Numer', dataProperty: 'number', component: 'text', required: true, defaultValue: maxNumber } as DetailsViewField,
                                { title: 'Data', dataProperty: 'date', component: 'date', required: true, defaultValue: defaultDate} as DetailsViewField,
                                { title: 'Firma', dataProperty: 'company', component: 'text', defaultValue: 'Wave'} as DetailsViewField,
                                { title: 'Ilość stron', dataProperty: 'pages', component: 'text', defaultValue: '1'} as DetailsViewField,
                                { title: 'Opis', dataProperty: 'description', component: 'text', defaultValue: 'Internet'} as DetailsViewField,
                                { title: 'Kategoria', dataProperty: 'category', component: 'text', defaultValue: 'Faktura otrzymana'} as DetailsViewField,
                                { title: 'Faktura', dataProperty: 'invoiceNumber', component: 'text', defaultValue: '1/' + invoiceNumberDate + '/F/6813'} as DetailsViewField
                            ]
                        };
                    } else if (params['type'] === 'ciklumTools') {
                        let defaultDate = '';
                        const lastInvoice = documents.filter(t => t.company?.toUpperCase() == 'CIKLUM' && t.category?.toUpperCase()==='FAKTURA OTRZYMANA').sort((d1,d2) => d1.date < d2.date ? 1 : -1)[0];
                        if (lastInvoice) {
                            let lastDate = new Date(lastInvoice.date);
                            lastDate.setDate(1);
                            lastDate.setMonth(lastDate.getMonth() + 2);
                            lastDate.setDate(0);
                            defaultDate = lastDate.toISOString();
                        }
                        this.viewDefinition = {
                            fields: [
                                { title: 'Numer', dataProperty: 'number', component: 'text', required: true, defaultValue: maxNumber } as DetailsViewField,
                                { title: 'Data', dataProperty: 'date', component: 'date', required: true, defaultValue: defaultDate} as DetailsViewField,
                                { title: 'Firma', dataProperty: 'company', component: 'text', defaultValue: 'Ciklum'} as DetailsViewField,
                                { title: 'Ilość stron', dataProperty: 'pages', component: 'text', defaultValue: '1'} as DetailsViewField,
                                { title: 'Opis', dataProperty: 'description', component: 'text', defaultValue: 'Ciklum narzędzia'} as DetailsViewField,
                                { title: 'Kategoria', dataProperty: 'category', component: 'text', defaultValue: 'Faktura otrzymana'} as DetailsViewField,
                                { title: 'Faktura', dataProperty: 'invoiceNumber', component: 'text', defaultValue: 'PL0XXXX'} as DetailsViewField
                            ]
                        };
                    } else if (params['type'] === 'fuel') {
                         this.viewDefinition = {
                            fields: [
                                { title: 'Numer', dataProperty: 'number', component: 'text', required: true, defaultValue: maxNumber } as DetailsViewField,
                                { title: 'Data', dataProperty: 'date', component: 'date', required: true} as DetailsViewField,
                                { title: 'Firma', dataProperty: 'company', component: 'text'} as DetailsViewField,
                                { title: 'Ilość stron', dataProperty: 'pages', component: 'text', defaultValue: '1'} as DetailsViewField,
                                { title: 'Opis', dataProperty: 'description', component: 'text', defaultValue: 'Paliwo'} as DetailsViewField,
                                { title: 'Kategoria', dataProperty: 'category', component: 'text', defaultValue: 'Faktura otrzymana'} as DetailsViewField,
                                { title: 'Faktura', dataProperty: 'invoiceNumber', component: 'text'} as DetailsViewField,
                                { title: 'Samochód', dataProperty: 'car', component: 'text'} as DetailsViewField
                            ]
                        };
                    } else if (params['type'] === 'mazda') {
                        let defaultDate = '';
                        let invoiceNumberDate='XX/XX'
                        const lastInvoice = documents.filter(t => t.company?.toUpperCase() == 'SANTANDER CONSUMER MULTIRENT' && t.category?.toUpperCase()==='FAKTURA OTRZYMANA').sort((d1,d2) => d1.date < d2.date ? 1 : -1)[0];
                        if (lastInvoice) {
                            let lastDate = new Date(lastInvoice.date);
                            lastDate.setMonth(lastDate.getMonth() + 1);
                            defaultDate = lastDate.toISOString();

                            let month = (lastDate.getMonth() + 1).toString();
                            let year = (lastDate.getFullYear()).toString().slice(2);
                            
                            invoiceNumberDate=month + '/' + year;
                            if (invoiceNumberDate.length < 5) {
                                invoiceNumberDate = '0' + invoiceNumberDate;
                            }
                        }
                        this.viewDefinition = {
                            fields: [
                                { title: 'Numer', dataProperty: 'number', component: 'text', required: true, defaultValue: maxNumber } as DetailsViewField,
                                { title: 'Data', dataProperty: 'date', component: 'date', required: true, defaultValue: defaultDate} as DetailsViewField,
                                { title: 'Firma', dataProperty: 'company', component: 'text', defaultValue: 'Santander Consumer Multirent'} as DetailsViewField,
                                { title: 'Ilość stron', dataProperty: 'pages', component: 'text', defaultValue: '1'} as DetailsViewField,
                                { title: 'Kategoria', dataProperty: 'category', component: 'text', defaultValue: 'Faktura otrzymana'} as DetailsViewField,
                                { title: 'Faktura', dataProperty: 'invoiceNumber', component: 'text', defaultValue: 'RLXXXXX/' + invoiceNumberDate} as DetailsViewField,
                                { title: 'Samochód', dataProperty: 'car', component: 'text', defaultValue: 'GA839ES'} as DetailsViewField
                            ]
                        };
                    } else if (params['type'] === 'invoice') {
                        this.viewDefinition = {
                           fields: [
                               { title: 'Numer', dataProperty: 'number', component: 'text', required: true, defaultValue: maxNumber } as DetailsViewField,
                               { title: 'Data', dataProperty: 'date', component: 'date', required: true} as DetailsViewField,
                               { title: 'Firma', dataProperty: 'company', component: 'text'} as DetailsViewField,
                               { title: 'Ilość stron', dataProperty: 'pages', component: 'text', defaultValue: '1'} as DetailsViewField,
                               { title: 'Opis', dataProperty: 'description', component: 'text'} as DetailsViewField,
                               { title: 'Kategoria', dataProperty: 'category', component: 'text', defaultValue: 'Faktura otrzymana'} as DetailsViewField,
                               { title: 'Faktura', dataProperty: 'invoiceNumber', component: 'text'} as DetailsViewField,
                               { title: 'Osoba', dataProperty: 'person', component: 'text'} as DetailsViewField,
                               { title: 'Samochód', dataProperty: 'car', component: 'text'} as DetailsViewField,
                               { title: 'Rzecz', dataProperty: 'relatedObject', component: 'text'} as DetailsViewField,
                               { title: 'Gwarancja', dataProperty: 'guarantee', component: 'text'} as DetailsViewField,
                           ]
                       };
                   } else {
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
                            ]
                        };
                    }
                });
        });
    }

    ngOnDestroy(){
        this.routeSubscription.unsubscribe();
    }

    toolbarElementClick(toolbarElementWithData: ToolbarElementWithData) {
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
        }
    }
}
