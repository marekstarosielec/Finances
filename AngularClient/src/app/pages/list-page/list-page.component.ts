import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GridColumn, RowClickedData, ViewChangedData } from 'app/shared/grid/grid.component';
import { Subject } from 'rxjs';
import * as l from 'lodash';
import { ToolbarElement, ToolbarElementAction } from 'app/shared/models/toolbar';
import { SubViewService } from 'app/services/subview.service';

export interface Summary {
    name: string;
    options: any;
}

export interface SummaryAmountCurrencyOptions {
    amountProperty: string;
    currencyProperty: string;
}

export interface SummaryAmountCategoryOptions {
    amountProperty: string;
    categoryProperty: string;
    showDirectioned?: boolean;
}

export interface SummaryTotalNumberOptions {
    numberProperty: string;
}

interface ViewAnalyticsData {
    totalNumberOfRecords: number;
    maximumVisibleNumberOfRecords: number;
    filteredData: any[];
    displayedData: any[];
    amountCurrency: any;
    amountCategory: any;
    totalNumber: any;
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

    constructor(private router: Router, private route: ActivatedRoute, private subviewService: SubViewService) {
    }

    ngOnInit() {

        if (!this.toolbarElements) {
            this.toolbarElements = [{ name: 'addNew', title: 'Dodaj', defaultAction: ToolbarElementAction.AddNew}];
        }
    }

    ngOnDestroy()
    {
        this.subviewService.subviews.next([]);
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
                    const summaryData = viewChangedData.filteredData.filter(e => e[option.currencyProperty]);
                    result.amountCurrency = {
                        amounts: l(summaryData)
                        .groupBy(option.currencyProperty)
                        .map((objs, key) => ({
                            'currency': key,
                            'amount': l.sumBy(objs, option.amountProperty) ?? 0,
                            'incoming': l.sumBy(objs.filter(o => o[option.amountProperty] > 0), option.amountProperty) ?? 0,
                            'outgoing': l.sumBy(objs.filter(o => o[option.amountProperty] < 0), option.amountProperty) ?? 0}))
                        .value()
                    };
                    result.amountCurrency['options'] = option;
                }
            }
            if (this.summaries?.find(s => s.name === 'amount-category')){
                const option = this.summaries.find(s => s.name === 'amount-category').options as SummaryAmountCategoryOptions;
                if (option) {
                    const summaryData = viewChangedData.filteredData.filter(e => e[option.amountProperty] && e[option.categoryProperty]);
                    result.amountCategory = {
                        amounts: l(summaryData)
                        .groupBy(option.categoryProperty)
                        .map((objs, key) => ({
                            'category': key,
                            'amount': l.sumBy(objs, option.amountProperty) ?? 0,
                            'incoming': l.sumBy(objs.filter(o => o[option.amountProperty] > 0), option.amountProperty) ?? 0,
                            'outgoing': l.sumBy(objs.filter(o => o[option.amountProperty] < 0), option.amountProperty) ?? 0}))
                        .value()
                    };
                    const total = l.sumBy(summaryData, option.amountProperty);
                    const totalIncoming = l.sumBy(summaryData.filter(s => s[option.amountProperty] > 0), option.amountProperty);
                    const totalOutgoing = l.sumBy(summaryData.filter(s => s[option.amountProperty] < 0), option.amountProperty);
                    result.amountCategory['totals'] = { amount: total, incoming: totalIncoming, outgoing: totalOutgoing};
                    result.amountCategory['options'] = option;
                }
            }
            if (this.summaries?.find(s => s.name === 'total-number')){
                const option = this.summaries.find(s => s.name === 'total-number').options as SummaryTotalNumberOptions;
                if (option) {
                    const summaryData = viewChangedData.filteredData.filter(e => e[option.numberProperty]).map(e => e[option.numberProperty]);
                    result.totalNumber = l.sum(summaryData);
                }
            }
            this.currentView$.next(result);
        }, 0);
    }

    rowClickedEvent(rowClickedData: RowClickedData) {
        this.rowClicked.emit(rowClickedData);
    }
}
