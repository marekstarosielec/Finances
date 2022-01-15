import { 
    Pipe, 
    PipeTransform 
 } from '@angular/core';  
 
 @Pipe ({ 
    name: 'formattedAmount' 
 }) 
 
 export class FormattedAmountPipe implements PipeTransform { 
   transform(value: number): string { 
      if (!value)
         return "";
      return value.toLocaleString('pl-pl', {maximumFractionDigits: 2, minimumFractionDigits:2});
   } 
 } 