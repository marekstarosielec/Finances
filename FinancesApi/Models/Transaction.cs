namespace FinancesApi.Models
{
    public class Transaction
    {
        //Data scrapowania
        public string ScrappingDate { get; set; }
    
        //Status	
        public string Status { get; set; }

        //Tagi
        public string Tags { get; set; }

        //Do
        public string To { get; set; }

        //Miasto sprzedawcy
        public string SellerCity { get; set; }

        //Kraj sprzedawcy
        public string SellerCountry { get; set; }

        //Data operacji
        public string OperationDate { get; set; }

        //Z rachunku powiązanego
        public string FromConnectedAccount { get; set; }

        //Numer karty debetowej
        public string DebitCardNo { get; set; }

        //Rodzaj operacji
        public string Type { get; set; }

        //Opis operacji
        public string OperationDescription { get; set; }

        //Nazwa odbiorcy
        public string Name { get; set; }

        //Kwota w walucie oryginalnej
        public string OriginalCurrencyAmount { get; set; }

        //Kwota w walucie rozliczeniowej
        public string SettlementCurrencyAmount { get; set; }

        //Kwota w walucie rachunku
        public string AccountCurrencyAmount { get; set; }

        //Saldo po operacji
        public string AfterOperationBalance { get; set; }

        //Data rozliczenia
        public string SettlementDate { get; set; }

        //Numer operacji
        public string OperationNumber { get; set; }

        //Zakup przy użyciu karty od
        public string PuchaseByCardFrom { get; set; }

        //Miejsce transakcji
        public string TransactionPlace { get; set; }

        //Tytuł przelewu
        public string TransactionTitle { get; set; }

        //Z rachunku nadawcy
        public string FromSenderAccount { get; set; }

        //Nadawca
        public string Sender { get; set; }

        //Adres nadawcy
        public string SenderAddress { get; set; }

        //Miejscowość nadawcy
        public string SenderCity { get; set; }

        //Na rachunek
        public string ToAccount { get; set; }

        //Bank odbiorcy
        public string ReceiverBank { get; set; }

        //Adres odbiorcy
        public string ReceiverAddress { get; set; }

        //Miejscowość odbiorcy
        public string ReceiverCity { get; set; }

        //Kwota operacji
        public string OperationAmount { get; set; }

        //Data księgowania
        public string BookingDate { get; set; }

        //Tytuł operacji
        public string OperationTitle { get; set; }

        //Konto własne
        public string OwnAccount { get; set; }

        //ScrapID
        public string ScrapID { get; set; }

        //NewScrap
        public int? NewScrap { get; set; }
    
        //Nazwa nadawcy
        public string SenderName { get; set; }

        //Bank nadawcy
        public string SenderBankName { get; set; }

        //Przelew wewnętrzny wychodzący od
        public string InnerTransferGoingFrom { get; set; }

        //Przelew zewnętrzny wychodzący od
        public string OuterTransferGoingFrom { get; set; }

        //Data transakcji
        public string TransactionDate { get; set; }

        //Przelew własny od
        public string OwnTransferFrom { get; set; }

        //Przelew mTransfer wychodzący od
        public string mTransferGoingFrom { get; set; }

        //Przelew wewnętrzny przychodzący od
        public string InnerTransferComingFrom { get; set; }

        //Przelew na twoje cele od
        public string SavingTransfer { get; set; }

        //Wypłata gotówki kartą
        public string CashWithdrawal { get; set; }

        //Prowizja-wypłata bankomat krajowy od
        public string NationalATMCommision { get; set; }

        //Przelew zewnętrzny przychodzący od
        public string OuterTransferComingFrom { get; set; }

        //Oryginał
        public string Original { get; set; }

        //Tekst tranzakcji
        public string TransactionText { get; set; }

        //Nazwa konta
        public string AccountName { get; set; }

        //Numer referencyjny
        public int? ReferenceNumber { get; set; }

        //Kwota przelewu
        public string TransactionAmount { get; set; }

        //Rachunek nadawcy
        public string SenderAccount { get; set; }

        //Prowizje i opłaty pokrywa
        public string CommisionsPaidBy { get; set; }

        //Odbiorca
        public string Receiver { get; set; }

        //Data i godzina otrzymania
        public string DateAndTimeOfReceiving { get; set; }

        //Zakup przy użyciu karty - transakcja telefonem od
        public string PurchaseByCardPhone { get; set; }

        //Reserved18
        public string Reserved18 { get; set; }

        //Data
        public string Date { get; set; }

        //Konto
        public string Account { get; set; }

        //Kategoria
        public string Category { get; set; }

        //Kwota
        public string Amount { get; set; }

        //Opis
        public string Description { get; set; }

        //Komentarz
        public string Comment { get; set; }

        //Faktura
        public string Invoice { get; set; }

        //Szczegóły
        public string Details { get; set; }

        //Osoba
        public string Person { get; set; }
    }
}
