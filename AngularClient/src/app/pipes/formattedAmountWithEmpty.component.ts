import { 
    Pipe, 
    PipeTransform 
 } from '@angular/core';  
 
 @Pipe ({ 
    name: 'formattedAmount' 
 }) 
 
 export class FormattedAmountWithEmptyPipe implements PipeTransform { 
   transform(value?: number): string { 
      if (value == undefined) {
         return "";
      }
      return value.toLocaleString('pl-pl', {maximumFractionDigits: 2, minimumFractionDigits:2});
   } 
 } 