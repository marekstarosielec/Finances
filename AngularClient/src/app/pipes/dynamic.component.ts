import { 
    Pipe, 
    PipeTransform 
 } from '@angular/core';  
import { FormattedAmountPipe } from './formattedAmount.component';
import { FormattedAmountWithEmptyPipe } from './formattedAmountWithEmpty.component';
import { FormattedDatePipe } from './formattedDate.component';
import { FormattedNumberPipe } from './formattedNumber.component';
 
 @Pipe ({ 
    name: 'dynamic' 
 }) 
 
 export class DynamicPipe implements PipeTransform { 
   transform(value: any, pipe: string): string { 
      switch(pipe){
         case "amount": 
            const amountPipe = new FormattedAmountPipe();
            return amountPipe.transform(value);
         case "amountwithempty": 
            const amountWithEmptyPipe = new FormattedAmountWithEmptyPipe();
            return amountWithEmptyPipe.transform(value);
         case "date": 
            const datePipe = new FormattedDatePipe();
            return datePipe.transform(value);
         case "number": 
            const numberPipe = new FormattedNumberPipe();
            return numberPipe.transform(value);
         default:
            return value?.toString();
      }
   } 
 } 