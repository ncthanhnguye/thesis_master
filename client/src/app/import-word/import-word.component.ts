import { ChangeDetectorRef, Component, OnInit, ViewChild, Input } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { GridDataResult, SelectableSettings, PageChangeEvent, DataStateChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { SelectEvent, FileInfo } from '@progress/kendo-angular-upload';
import { State, process, SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { AppComponent } from '../app.component';
import { AppConsts } from '../services/app.consts';
import { AppControls } from '../services/app.controls';
import { AppFile } from '../services/app.file';
import { AppGuid } from '../services/app.guid';
import { AppService } from '../services/app.service';
import { AppSwal } from '../services/app.swal';
import { AppUtils } from '../services/app.utils';
import { AuthenticationService } from '../services/authentication.service';
import { Notification } from '../services/app.notification';
import { AppLanguage } from '../services/app.language';

@Component({
	selector: 'app-import-word',
	templateUrl: './import-word.component.html',
	styleUrls: ['./import-word.component.css']
})
export class ImportWordComponent implements OnInit {
	public ManagePersonalOpened = false;
	public isReloadByViewChild: any;
	public request_AccountID;
	user: any;
	loading = false;
	ManagePersonals: any;
	ManagePersonalSelectableSettings: SelectableSettings;
	ManagePersonalSort = {
		allowUnsort: true,
		mode: 'multiple'
	};

	ManagePersonalSortByField: SortDescriptor[] = [
		// {
		// field: 'Name',
		// dir: 'asc'
		// }
	];


	ManagePersonalGridDataResult: GridDataResult;

	//used for kendo grid
	public WORKING_NUM_PAGING_SKIP = 0;
	public WORKING_NUM_PAGING_TAKE = 10;
	public WORKING_NUM_PAGING_BTN = 5;
	public buttonCount = this.WORKING_NUM_PAGING_BTN;
	// paging in grid
	dataManagePersonalSkip = this.WORKING_NUM_PAGING_SKIP;
	dataManagePersonalPageSize = this.WORKING_NUM_PAGING_TAKE;
	public ManagePersonalChange: State = {}
	public ManagePersonalState: State = {
		skip: this.dataManagePersonalSkip,
		take: this.dataManagePersonalSkip + this.dataManagePersonalPageSize,
		filter: {
			logic: 'and',
			filters: []
		}
	};
	pageName = '';

	unit: Array<{ Name: string, ID: string }> = [];
	unitFilter: Array<{ Name: string, ID: string }> = [];
	ManagePersonalSelection: number[] = [];
	dataManagePersonalItem: any;
	dataManagePersonalItemtemp: any;
	myInterval: any;

	public uploadSaveUrl = 'saveUrl';
	public uploadRemoveUrl = 'removeUrl';

	isEnabledSaveAll = false;
	control: any;
	controlDefault = true;
	public myFiles: Array<FileInfo> = [];
	option: Array<any>;


	Personal: Array<{ ID: string; CurrentName: string }>;
	PersonalFilter: Array<{ ID: string; CurrentName: string }>;
	btnFunctionData: Array<any> = [];
	btnMbFunctionData: Array<any> = [];

	filesUpload: Array<FileInfo>;
	filesUploadName = "";
	allowInsertFile = true;
	urlDownload = this.appService.apiRoot;
	public GUID_EMPTY = "00000000-0000-0000-0000-000000000000";

	searchOption = {
		SearchText: '',
		unitID: ''
	};

	public dataItemTemp = null;

	isSummaryInfoCofig = false;
	enabledImportExcelFlg = false;
	public fileSaveUrl: any;

	public Enum = {
		Post_Reference: 0, // nguồn tin bài
		Position: 1, // chức vụ
		Ethnic: 2, // Dân tộc
		PoliticalTheory: 3, // Lý luận chính trị
		CommunistPartyPosition: 4, // Chức vụ đảng
		UnitType: 5, // Phân loại đơn vị (khối,...)
		Qualification: 6,// trình độ chuyên môn
	};

	genders: Array<{ Name: string, ID: number }>;
	positions: Array<{ ID: string, Name: string }> = [];
	units: Array<{ Name: string, ID: string }> = [];

	ethnic: Array<{ Name: string, ID: string, Type: number, OrderIndex: number }> = [];
	communistPartyPosition: Array<{ Name: string, ID: string, Type: number, OrderIndex: number }> = [];
	politicalTheory: Array<{ Name: string, ID: string, Type: number, OrderIndex: number }> = [];
	qualification: Array<{ Name: string, ID: string, Type: number, OrderIndex: number }> = [];
	selectedLangID = this.appConsts.defaultLangID;
	personalsTemp = [];
	tempData = [{
		UserName: '',
		Name: '',
		GenderName: '',
		BirthDate: '',
		UnitName: '',
	}

	]
	public defaultUnitItem: { Name: string; ID: string } = {
		Name: "Tất cả",
		ID: null
	};

	@ViewChild('excelexport1') excelexport1: any;
	@ViewChild('excelexport2') excelexport2: any;

	ViewHistoryCompetitionRewardDialog = false;

	constructor(
		private translate: TranslateService,
		private appService: AppService,
		private language: AppLanguage,
		private appSwal: AppSwal,
		public intl: IntlService,
		private notification: Notification,
		private guid: AppGuid,
		private file: AppFile,
		private authenticationService: AuthenticationService,
		public appControls: AppControls,
		private appConsts: AppConsts,
		private appComponent: AppComponent,
		private cdRef: ChangeDetectorRef,
		public appUtils: AppUtils,
	) {
		this.authenticationService.getUser();
		this.user = this.authenticationService.user;
		this.setDefault();
		this.setSelectableSettings();
	}

	async ngOnInit() {
		this.getManagePersonals();
	}

	ngOnDestroy(): void {

	}

	async initDisplay() {
		this.getManagePersonals();
	}

	async onReload() {
		this.getManagePersonals();
	}

	setSelectableSettings(): void {
		this.ManagePersonalSelectableSettings = {
			checkboxOnly: false,
			mode: 'multiple'
		};
	}

	async getManagePersonals() {
		this.loading = true;

		const dataRequest = {

		};

		const result = await this.appService.doGET('api/ImportDataCrawl/Search', dataRequest);
		if (result && result.Data) {
			this.ManagePersonals = result.Data;
			this.ManagePersonalState.skip = 0;
			this.dataManagePersonalSkip = 0;
			this.bindManagePersonals();
		}
		this.loading = false;
		this.checkSelectionID();
	}

	AccountHandleFilter(value) {
		this.ManagePersonalState = this.ManagePersonals.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
	}

	checkSelectionID() {
		for (let i = this.ManagePersonalSelection.length - 1; i >= 0; i--) {
			const selectedItem = this.ManagePersonals.find((item) => {
				return item.ID === this.ManagePersonalSelection[i];
			});
			if (!selectedItem) {
				this.ManagePersonalSelection.splice(i, 1);
			}
		}
	}

	setDefault() {
		this.dataManagePersonalItem = {
			IsAdd: true,
			ID: this.GUID_EMPTY,

		};
		this.dataManagePersonalItemtemp = {
			IsAdd: true,
			ID: this.GUID_EMPTY,

		};
		this.filesUpload = [];
		this.filesUploadName = "";
		this.fileSaveUrl = `${this.appService.apiRoot}api/Upload?dirName=${this.user.UserName}&typeData=files&acceptExtensions=[".xls",".xlsx"]`;
	}

	bindtemp(item) {
		this.dataManagePersonalItemtemp.ID = item.ID;
		this.dataManagePersonalItemtemp.AccountID = item.AccountID;
		this.dataManagePersonalItemtemp.Date = item.Date;

	}

	onManagePersonalPageChange(event: PageChangeEvent) {
		this.dataManagePersonalSkip = event.skip;
		this.bindManagePersonals();

	}


	onManagePersonalSelectedKeysChange() {

	}

	bindManagePersonals() {
		this.ManagePersonalGridDataResult = {
			data: orderBy(this.ManagePersonals, this.ManagePersonalSortByField),
			total: this.ManagePersonals.length
		};
		this.ManagePersonalGridDataResult = process(this.ManagePersonals, this.ManagePersonalState);
	}

	onManagePersonalSortChange(sort: SortDescriptor[]): void {
		this.ManagePersonalSortByField = sort;
		this.bindManagePersonals();

	}

	async onChangeYear(e: any) {
		this.onReload();

	}
	public onManagePersonalDataStateChange(state: DataStateChangeEvent): void {
		this.ManagePersonalSelection = [];
		this.ManagePersonalState = state;
		this.ManagePersonalGridDataResult = process(this.ManagePersonals, this.ManagePersonalState);
	}

	onAddNewManagePersonal() {
		this.ManagePersonalOpened = true;
		this.setDefault();
	}

	onClosePersonal(e) {
		this.onReload();
		this.ManagePersonalOpened = false;

	}

	onEditManagePersonal(ID) {
		var selectedItem = this.ManagePersonals.find((item) => {
			return item.ID === ID;
		});
		selectedItem.IsAdd = false;
		this.dataManagePersonalItem = selectedItem;
		this.request_AccountID = this.dataManagePersonalItem.ID;
		this.ManagePersonalOpened = true;

	}
	async onDeleteManagePersonal(ID) {
		const dataRequest = {
			iD: ID
		};

		const option = await this.appSwal.showWarning(this.translate.instant('AreYouSure'), true);
		if (option) {
			const result = await this.appService.doDELETE('api/Account', dataRequest);
			if (result && result.Status === 1) {
				this.notification.showSuccess(result.Msg);
				this.onReload();
			} else {
				this.appSwal.showWarning(result.Msg, false);
			}
		}
		this.appComponent.loading = false;
	}



	async onSearchKeyPress(e: any) {
		if (e.keyCode === 13) {
			this.getManagePersonals();

		}
	}
	async onSearch() {
		this.getManagePersonals();
	}
	async onRemoveSearchText() {
		this.searchOption.SearchText = '';
		this.getManagePersonals();
	}


	onChangeFunction(e, dataItem) {
		if (e.id == 'Edit') {
			this.onEditManagePersonal(dataItem.ID);
		}
		else if (e.id == 'Delete') {
			this.onDeleteManagePersonal(dataItem.ID)
		}
	}
	onFunctionIconClick(dataItem) {
		this.getbtnFunctionData();
	}

	getbtnFunctionData() {

		this.btnFunctionData = [];
		this.btnMbFunctionData = [];


		if (this.controlDefault || this.control.Edit) {
			var func1 = {
				text: this.translate.instant('Edit'),
				id: 'Edit',
				icon: 'pencil',
			};
			this.btnFunctionData.push(func1);
			this.btnMbFunctionData.push(func1);
		}


		if (this.controlDefault || this.control.Delete) {
			var func2 = {
				text: this.translate.instant('Delete'),
				id: 'Delete',
				icon: 'delete',
			};
			this.btnFunctionData.push(func2);
			this.btnMbFunctionData.push(func2);
		}



	}


	async showAdvancedImportExcel() {
		this.enabledImportExcelFlg = !this.enabledImportExcelFlg;
		if (this.enabledImportExcelFlg) {
			this.ManagePersonals = [];
			this.bindManagePersonals();
		} else {
			this.getManagePersonals();
		}
	}

	onSelectEventHandler(e: SelectEvent) {
		this.loadXLSX(e);
	}



	async loadXLSX(e) {
		const fileData = (await this.file.readXLSX(e.files[0].rawFile)) as Array<any>;
		this.ManagePersonals = [];
		this.bindManagePersonals();
		this.personalsTemp = [];

		for (let i = 1; i < fileData.length; i++) {

			this.ManagePersonals.push({
				ID: fileData[i][0],
				TenCauHoi: fileData[i][1],
				LinhVuc: fileData[i][2],
				NoiDungCauHoi: fileData[i][3],
				CauTraLoi: fileData[i][4],
				Luat: fileData[i][5],
				KeyWords: fileData[i][6],

			});

		}
		this.bindManagePersonals();

	}

	async onSavePersonals() {
		const dataRequests = [];
		for (let i = 0; i < this.ManagePersonals.length; i++) {
			dataRequests.push(this.ManagePersonals[i]);
		}
		const result = await this.appService.doPOST('api/ImportDataCrawl/Saves', dataRequests);
		if (result && result.Status === 1) {
			this.notification.showSuccess(result.Msg);
			this.enabledImportExcelFlg = false;
			this.getManagePersonals();
		} else {
			this.appSwal.showWarning(result.Msg, false);
		}
	}

	onExportExcelNotUpdateInfo(excelexport) {
		excelexport.data = excelexport.data.filter(function (e) {
			return e.Phone == null || e.PositionName == null;
		});
		excelexport.save();
	}
	async getUnitName() {
		const dataRequest = null;

		const result = await this.appService.doGET('api/Unit', dataRequest);
		if (result && result.Data) {
			this.unit = result.Data;
			this.unitFilter = this.unit.slice();
		} else {
			this.appSwal.showWarning(result.Msg, false);
		}
	}
	unitHandleFilter(value) {
		this.unitFilter = this.unit.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
	}


}
