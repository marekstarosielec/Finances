import { DatePipe } from '@angular/common';
import { 
    Pipe, 
    PipeTransform 
 } from '@angular/core';  
 
 @Pipe ({ 
    name: 'formattedDate' 
 }) 
 
 export class FormattedDatePipe implements PipeTransform { 
    transform(value: string): string { 
      if (!value)
         return value;
      const pipe = new DatePipe("en-US");
      return pipe.transform(value, "yyyy-MM-dd");
    } 
 } 