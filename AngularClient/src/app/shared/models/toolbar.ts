import { FormGroup } from "@angular/forms";

export enum ToolbarElementAction {
    AddNew,
    SaveChanges,
    Delete
}

export interface ToolbarElement {
    name: string;
    title: string;
    defaultAction?: ToolbarElementAction;
}

export interface ToolbarElementWithData {
    toolbarElement: ToolbarElement;
    data: any;
}