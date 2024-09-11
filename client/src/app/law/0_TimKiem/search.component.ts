import { Component, OnInit, OnDestroy } from '@angular/core';
import { AppService } from 'src/app/services/app.service';
import { SelectableSettings, GridDataResult } from '@progress/kendo-angular-grid';
import { State, SortDescriptor } from '@progress/kendo-data-query';
import { IntlService } from '@progress/kendo-angular-intl';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
	selector: 'app-search',
	templateUrl: './search.component.html',
	// styleUrls: ['./search.component.css'],
})
export class SearchComponent implements OnInit, OnDestroy {

	dataDetailItem: any;
	dataDetailItemtemp: any;
	DialogDetail = false;
	searchOption = {
		searchText1: '' , 
		searchText2: ''
	};
	searchDetail: boolean = false;
	DataList: any;
	resultData: any[] = []; // Store the fetched data
	detailAnswer: boolean = false;

	data: any[] = [];
	totalItems = 0;
	pageIndex = 0;
	pageSize = 10;
	totalPages = 0;

	constructor(
		private appService: AppService,
		public intl: IntlService,
	) {

	}

	ngOnDestroy(): void {

	}

	ngOnInit() {
		this.onSearch();
	}
	
	onPageChange(pageIndex: number) {
		if (pageIndex < 0 || pageIndex >= this.totalPages) {
		  return;
		}
		this.pageIndex = pageIndex;
		const startIndex = this.pageIndex * this.pageSize;
		const endIndex = startIndex + this.pageSize;
		this.DataList = this.resultData.slice(startIndex, endIndex); // Use stored data for pagination
	}

	bindTemp(item) {
		this.dataDetailItemtemp = Object.assign({}, item);
	}



	onSearchKeyPress(e: any) {
		if (e.keyCode === 13 && this.searchOption.searchText1) {
			this.onSearch();
		}
	}

	async onSearch() {
		this.searchDetail = true;
		const dataRequest = {
		  text: this.searchOption.searchText1,
		  // Optionally, you can add other search parameters here
		};
	
		const result = await this.appService.doPOST_Python('underthesea', dataRequest);
		if (result) {
		  this.resultData = result.results; // Store the fetched data
		  this.totalPages = Math.ceil(this.resultData.length / this.pageSize);
		  this.onPageChange(0); // Call onPageChange to update DataList for the first page
		}
	}

	syncInputs(value: string) {
		this.searchOption.searchText1 = value;
		this.searchOption.searchText2 = value;
	}

	async onDetail(iD){
		this.detailAnswer = true;
		const dataRequest = {
			iD : iD
		}
		const result = await this.appService.doGET('api/TimKiem/GetByID', dataRequest);

		if (result) {
			this.bindTemp(result.Data);
		}
	}
	onBack(){
		if(this.searchDetail){
			this.detailAnswer = false ;
		}
		else {
			this.searchDetail = false; 
			this.detailAnswer = false
		}
	}
}
