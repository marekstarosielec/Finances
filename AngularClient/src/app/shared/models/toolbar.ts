import { FormGroup } from "@angular/forms";

export enum ToolbarElementAction {
    AddNew,
    SaveChanges,
    Delete
}

export interface ToolbarElement {
    name: string;
    title?: string;
    defaultAction?: ToolbarElementAction;
    align?: string;
    image?: string;
}

export interface ToolbarElementWithData {
    toolbarElement: ToolbarElement;
    data: any;
}