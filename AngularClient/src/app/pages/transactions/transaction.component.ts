import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Transaction, TransactionAccount, TransactionCategory, TransactionsService } from 'app/api/generated';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import {v4 as uuidv4} from 'uuid';
import { Location } from '@angular/common';

// interface TransactionHeader {
//     column: string;
//     showInBankInfo: boolean;
//     title: string;
//     isError?: boolean;
//     value?: string;
// }

// const TransactionHeaders: TransactionHeader[] = [
//     { column: 'scrappingDate', showInBankInfo: false, title: 'Data scrapowania' },
//     { column: 'status', showInBankInfo: true, title: 'Status' },
//     { column: 'tags', showInBankInfo: true, title: 'Tagi' },
//     { column: 'to', showInBankInfo: true, title: 'Do' },
//     { column: 'sellerCity', showInBankInfo: true, title: 'Miasto sprzedawcy' },
//     { column: 'sellerCountry', showInBankInfo: true, title: 'Data operacji' },
//     { column: 'operationDate', showInBankInfo: true, title: 'Z rachunku powiązanego' },
//     { column: 'fromConnectedAccount', showInBankInfo: true, title: 'Z rachunku powiązanego' },
//     { column: 'debitCardNo', showInBankInfo: true, title: 'Numer karty debetowej' },
//     { column: 'type', showInBankInfo: true, title: 'Rodzaj operacji' },
//     { column: 'operationDescription', showInBankInfo: true, title: 'Opis operacji' },
//     { column: 'name', showInBankInfo: true, title: 'Nazwa odbiorcy' },
//     { column: 'originalCurrencyAmount', showInBankInfo: true, title: 'Kwota w walucie rozliczeniowej' },
//     { column: 'settlementCurrencyAmount', showInBankInfo: true, title: 'Kwota w walucie rachunku' },
//     { column: 'accountCurrencyAmount', showInBankInfo: true, title: '' },
//     { column: 'afterOperationBalance', showInBankInfo: true, title: 'Saldo po operacji' },
//     { column: 'settlementDate', showInBankInfo: true, title: 'Data rozliczenia' },
//     { column: 'operationNumber', showInBankInfo: true, title: 'Numer operacji' },
//     { column: 'puchaseByCardFrom', showInBankInfo: true, title: 'Zakup przy użyciu karty od' },
//     { column: 'transactionPlace', showInBankInfo: true, title: 'Miejsce transakcji' },
//     { column: 'transactionTitle', showInBankInfo: true, title: 'Tytuł przelewu' },
//     { column: 'fromSenderAccount', showInBankInfo: true, title: 'Z rachunku nadawcy' },
//     { column: 'sender', showInBankInfo: true, title: 'Nadawca' },
//     { column: 'senderAddress', showInBankInfo: true, title: 'Adres nadawcy' },
//     { column: 'senderCity', showInBankInfo: true, title: 'Miejscowość nadawcy' },
//     { column: 'toAccount', showInBankInfo: true, title: 'Na rachunek' },
//     { column: 'receiverBank', showInBankInfo: true, title: 'Bank odbiorcy' },
//     { column: 'receiverAddress', showInBankInfo: true, title: 'Adres odbiorcy' },
//     { column: 'receiverCity', showInBankInfo: true, title: 'Miejscowość odbiorcy' },
//     { column: 'operationAmount', showInBankInfo: true, title: 'Kwota operacji' },
//     { column: 'bookingDate', showInBankInfo: true, title: 'Data księgowania' },
//     { column: 'operationTitle', showInBankInfo: true, title: 'Tytuł operacji' },
//     { column: 'ownAccount', showInBankInfo: true, title: 'Konto własne' },
//     { column: 'scrapID', showInBankInfo: false, title: 'ScrapID' },
//     { column: 'newScrap', showInBankInfo: false, title: 'NewScrap' },
//     { column: 'senderName', showInBankInfo: true, title: 'Nazwa nadawcy' },
//     { column: 'senderBank', showInBankInfo: true, title: 'Bank nadawcy' },
//     { column: 'innerTransferGoingFrom', showInBankInfo: true, title: 'Przelew wewnętrzny wychodzący od' },
//     { column: 'outerTransferGoingFrom', showInBankInfo: true, title: 'Przelew zewnętrzny wychodzący od' },
//     { column: 'transactionDate', showInBankInfo: true, title: 'Data transakcji' },
//     { column: 'ownTransferFrom', showInBankInfo: true, title: 'Przelew własny od' },
//     { column: 'mTransferGoingFrom', showInBankInfo: true, title: 'Przelew mTransfer wychodzący od' },
//     { column: 'innerTransferComingFrom', showInBankInfo: true, title: 'Przelew wewnętrzny przychodzący od' },
//     { column: 'savingTransfer', showInBankInfo: true, title: 'Przelew na twoje cele od' },
//     { column: 'cashWithdrawal', showInBankInfo: true, title: 'Wypłata gotówki kartą' },
//     { column: 'nationalATMCommision', showInBankInfo: true, title: 'Prowizja-wypłata bankomat krajowy od' },
//     { column: 'outerTransferComingFrom', showInBankInfo: true, title: 'Przelew zewnętrzny przychodzący od' },
//     { column: 'original', showInBankInfo: false, title: 'Oryginał' },
//     { column: 'transactionText', showInBankInfo: true, title: 'Tekst tranzakcji' },
//     { column: 'accountName', showInBankInfo: false, title: 'Nazwa konta' },
//     { column: 'referenceNumber', showInBankInfo: true, title: 'Numer referencyjny' },
//     { column: 'transactionAmount', showInBankInfo: true, title: 'Kwota przelewu' },
//     { column: 'senderAccount', showInBankInfo: true, title: 'Rachunek nadawcy' },
//     { column: 'commisionsPaidBy', showInBankInfo: true, title: 'Prowizje i opłaty pokrywa' },
//     { column: 'receiver', showInBankInfo: true, title: 'Odbiorca' },
//     { column: 'dateAndTimeOfReceiving', showInBankInfo: true, title: 'Data i godzina otrzymania' },
//     { column: 'purchaseByCardPhone', showInBankInfo: true, title: 'Zakup przy użyciu karty - transakcja telefonem od' },
//     { column: 'date', showInBankInfo: false, title: 'Data' },
//     { column: 'account', showInBankInfo: false, title: 'Konto' },
//     { column: 'category', showInBankInfo: false, title: 'Kategoria' },
//     { column: 'amount', showInBankInfo: false, title: 'Kwota' },
//     { column: 'description', showInBankInfo: false, title: 'Opis' },
//     { column: 'comment', showInBankInfo: false, title: 'Komentarz' },
//     { column: 'invoice', showInBankInfo: false, title: 'Faktura' },
//     { column: 'details', showInBankInfo: false, title: 'Szczegóły' },
//     { column: 'person', showInBankInfo: false, title: 'Osoba' }
// ]

@Component({
    selector: 'transaction',
    moduleId: module.id,
    templateUrl: 'transaction.component.html',
    styleUrls: ['./transaction.component.scss']
})
export class TransactionComponent implements OnInit, OnDestroy{
    private routeSubscription: Subscription;
    data: Transaction;
    accounts: TransactionAccount[];
    categories: TransactionCategory[];
    adding: boolean = false;
    form = new FormGroup({
        scrappingDate: new FormControl('', []),
        status: new FormControl('', []),
        scrapID: new FormControl('', []),
        date: new FormControl('', []),
        account: new FormControl('', []),
        category: new FormControl('', []),
        amount: new FormControl('', [Validators.required]),
        description: new FormControl('', []),
        comment: new FormControl('', []),
        details: new FormControl('', []),
        person: new FormControl('', [])
    });
    
    constructor (private transactionsService: TransactionsService, private route: ActivatedRoute, private location: Location,
        private modalService: NgbModal) {}

    ngOnInit(){
        this.routeSubscription = this.route.params.subscribe(
            (params: Params) => {
                this.transactionsService.transactionsAccountsGet().pipe(take(1)).subscribe((result: TransactionAccount[]) => {
                    this.accounts = result;
                });
                this.transactionsService.transactionsCategoriesGet().pipe(take(1)).subscribe((result: TransactionCategory[]) => {
                    this.categories = result;
                });
                if (params['id']==='new'){
                    this.adding = true;
                } else { 
                    this.adding = false;
                    this.transactionsService.transactionsIdGet(params['id']).pipe(take(1)).subscribe((result: Transaction) => {
                        this.data = result;
                        this.form.setValue(result);
                        let date = new Date(result.date);
                        this.form.controls['date'].setValue({year: date.getFullYear(), month:date.getMonth()+1, day: date.getDate()});
                    });
                }
            }
          );
    }

   ngOnDestroy() {
        this.routeSubscription.unsubscribe();
   }

//    getBankInfo(transaction:Transaction): TransactionHeader[] {
//         if(!transaction)
//             return;
//         let result: TransactionHeader[] = [];
//         Object.keys(transaction).forEach(key => {
//             let header = TransactionHeaders.find(th => th.column === key);
//             if (!header){
//                 header = { column: key, title: key, isError: true, showInBankInfo: true };
//             }
//             if (transaction[key] && transaction[key] !== '' && header.showInBankInfo) {
//                 header.value = transaction[key];
//                 result.push(header);
//             }
//         });
//         return result;
//     }


    isSavable(): boolean {
        return this.form.valid && (this.adding || this.isFormChanged());
    }

    isFormChanged() : boolean {
        if (!this.data)
            return true;
        
        var props = Object.getOwnPropertyNames(this.data);
        for (var i = 0; i < props.length; i++) {
            if (this.data[props[i]] !== this.form.value[props[i]]) {
                return true;
            }
        }
        
        return false;
    }

    isDeletable(): boolean {
        return !this.adding;
    }

    submit() {
        if(!this.form.valid){
            return;
        }
        this.form.value.date=new Date(this.form.value.date.year, this.form.value.date.month-1, this.form.value.date.day);
        if (this.adding) {
            this.form.value.scrapID = uuidv4();
            this.transactionsService.transactionsTransactionPost(this.form.value).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        } else {
            this.transactionsService.transactionsTransactionPut(this.form.value).pipe(take(1)).subscribe(() =>
            {
                this.location.back();
            });
        }
       
    }

    delete() {
        // this.transactionsService.transactionsAccountIdDelete(this.data.id).pipe(take(1)).subscribe(() =>
        // {
        //     this.location.back();
        // });
    }

    open(content) {
    //     this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
    //         if (result === 'delete')
    //             this.delete();
    //     }, (reason) => { });
      }
}
