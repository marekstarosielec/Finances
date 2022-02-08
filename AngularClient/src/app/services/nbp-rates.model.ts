export interface NbpRates { 
    table: string;
    currency: string;
    code: string;
    rates: NbpRate[];
}

export interface NbpRate { 
    no: string;
    effectiveDate: string;
    mid: number;
}