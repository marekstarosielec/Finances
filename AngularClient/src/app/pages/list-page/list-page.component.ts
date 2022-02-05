import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GridColumn, RowClickedData, ViewChangedData } from 'app/shared/grid/grid.component';
import { Subject } from 'rxjs';
import * as l from 'lodash';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';

export interface Summary {
    name: string;
    options: any;
}

export interface SummaryAmountCurrencyOptions {
    amountProperty: string;
    currencyProperty: string;
}

interface ViewAnalyticsData {
    totalNumberOfRecords: number;
    maximumVisibleNumberOfRecords: number;
    filteredData: any[];
    displayedData: any[];
    amountCurrency: any;
}

@Component({
    selector: 'list-page',
    styleUrls: ['./list-page.component.scss'],
    templateUrl: 'list-page.component.html'
})

export class ListPageComponent implements OnInit, OnDestroy {
    @Input() public name: string;
    @Input() public columns: GridColumn[];
    @Input() public data: any[];
    @Input() public initialSortColumn: string;
    @Input() public initialSortOrder: number;
    @Input() public toolbarElements: ToolbarElement[];
    @Input() public summaries: Summary[];
    @Output() public toolbarElementClick = new EventEmitter<ToolbarElement>();
    @Output() public rowClicked = new EventEmitter<RowClickedData>();
    
    maximumNumberOfRecordsToShow: Number = 100;
    public currentView$: Subject<ViewAnalyticsData> = new Subject<ViewAnalyticsData>();

    constructor(private router: Router, private route: ActivatedRoute) {
    }

    ngOnInit() {
        if (!this.toolbarElements) {
            this.toolbarElements = [{ name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew}];
        }
    }

    ngOnDestroy()
    {

    }

    toolbarElementClicked(toolbarElement: ToolbarElement) {
        if (toolbarElement.defaultAction === ToolbarElementAction.AddNew) {
            this.router.navigate(["new"], { relativeTo: this.route});
        } else {
            this.toolbarElementClick.emit(toolbarElement);
        }
    }
    
    viewChanged(viewChangedData: ViewChangedData) {
        setTimeout(() => {
            let result = {
                totalNumberOfRecords: viewChangedData.totalNumberOfRecords,
                maximumVisibleNumberOfRecords: viewChangedData.maximumVisibleNumberOfRecords,
                filteredData: viewChangedData.filteredData,
                displayedData: viewChangedData.displayedData
            } as ViewAnalyticsData;
            if (this.summaries?.find(s => s.name === 'amount-currency')){
                const option = this.summaries.find(s => s.name === 'amount-currency').options as SummaryAmountCurrencyOptions;
                if (option) {
                    result.amountCurrency = {
                        amounts: l(viewChangedData.filteredData)
                        .groupBy(option.currencyProperty)
                        .map((objs, key) => ({
                            'currency': key,
                            'amount': l.sumBy(objs, option.amountProperty) ?? 0,
                            'incoming': l.sumBy(objs.filter(o => o[option.amountProperty] > 0), option.amountProperty) ?? 0,
                            'outgoing': l.sumBy(objs.filter(o => o[option.amountProperty] < 0), option.amountProperty) ?? 0}))
                        .value()
                    };
                }
            }
            this.currentView$.next(result);
        }, 0);
    }

    rowClickedEvent(rowClickedData: RowClickedData) {
        this.rowClicked.emit(rowClickedData);
    }
}
