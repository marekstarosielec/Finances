import { 
    Pipe, 
    PipeTransform 
 } from '@angular/core';  
 
 @Pipe ({ 
    name: 'formattedNumber' 
 }) 
 
 export class FormattedNumberPipe implements PipeTransform { 
    transform(value: number): string { 
      if (!value)
         return "";
      return value.toLocaleString('pl-pl', {maximumFractionDigits : 4});
    } 
 } 