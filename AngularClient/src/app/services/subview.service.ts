import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface SubviewDefinition {
    title?: string;
    icon?: string;
    link: string;
}

@Injectable({
  providedIn: 'root'
})
export class SubViewService {
    public subviews: BehaviorSubject<SubviewDefinition[]> = new BehaviorSubject([]);
}
