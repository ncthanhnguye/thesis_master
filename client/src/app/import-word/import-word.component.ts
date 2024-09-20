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
	lawData: {
		// ID: number,
		Title: string,
		Content: string,
		ContentHTML: string,
	}
	lawWordData: {
		chuong: string[],
		muc: string[],
		dieu: string[],
		khoan: string[],
		diem: string[],
	}
	user: any;
	loading = false;
	pageName = 'Import dữ liệu về luật'; 
	fileDataImport: any;
	nameLaw: '';

	unit: Array<{ Name: string, ID: string }> = [];
	unitFilter: Array<{ Name: string, ID: string }> = [];

	Law: Array<{ ID: number; Index: number; Title: string; Content: string; ContentHTML: string; }> = [];

	public uploadSaveUrl = 'saveUrl';
	public uploadRemoveUrl = 'removeUrl';

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

	enabledImportWordFlg = false;
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
		// this.setSelectableSettings();
		this.lawData = {
			// ID: number,
			Title: null,
			Content: null,
			ContentHTML: null,
		}
	}

	async ngOnInit() {
		this.getLaw();
	}

	ngOnDestroy(): void {

	}

	async initDisplay() {
		// this.getManagePersonals();
	}

	async onReload() {
		// this.getManagePersonals();
	}

	setDefault() {
		// this.dataManagePersonalItem = {
		// 	IsAdd: true,
		// 	ID: this.GUID_EMPTY,

		// };
		// this.dataManagePersonalItemtemp = {
		// 	IsAdd: true,
		// 	ID: this.GUID_EMPTY,

		// };
		this.filesUpload = [];
		this.filesUploadName = "";
		this.fileSaveUrl = `${this.appService.apiRoot}api/Upload?dirName=${this.user.UserName}&typeData=files&acceptExtensions=[".doc",".docx"]`;
	}

	async onSearchKeyPress(e: any) {
		if (e.keyCode === 13) {
			// this.getManagePersonals();

		}
	}
	async onSearch() {
		// this.getManagePersonals();
	}
	async onRemoveSearchText() {
		this.searchOption.SearchText = '';
		// this.getManagePersonals();
	}

	onBack() {
		this.enabledImportWordFlg = !this.enabledImportWordFlg;
	}

	onChangeFunction(e, dataItem) {
		if (e.id == 'Edit') {
			// this.onEditManagePersonal(dataItem.ID);
		}
		else if (e.id == 'Delete') {
			// this.onDeleteManagePersonal(dataItem.ID)
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


	async showAdvancedImportWord() {
		this.enabledImportWordFlg = !this.enabledImportWordFlg;
	}

	onSelectEventHandler(e: SelectEvent) {
		this.loadDoc(e);
	}

	async loadDoc(e) {
		this.fileDataImport = (await this.file.readDocx(e.files[0].rawFile, 'html')) as string;
		if (this.fileDataImport != null || this.fileDataImport != undefined) {
			this.lawData.ContentHTML = this.fileDataImport;
		}

		const rawData = (await this.file.readDocx(e.files[0].rawFile, 'text')) as string
		if (this.fileDataImport != null && this.fileDataImport != undefined) {
			this.processText(rawData);
			this.showSuccessMessage();
		}
	}

	processText(text: string) {
		this.lawWordData = {
			chuong: [],
			muc: [],
			dieu: [],
			khoan: [],
			diem: []
		};

		// Split data into lines for extracting
		const lines = text.split('\n');
		
		lines.forEach(line => {
			// if (line.startsWith('Bộ ')) {
			// 	this.lawWordData.luat.push(line);
			// } 
			if (line.startsWith('Chương ')) {
				this.lawWordData.chuong.push(line);
			} else if (line.startsWith('Mục ')) {
				this.lawWordData.muc.push(line);
			} else if (line.startsWith('Điều ')) {
				this.lawWordData.dieu.push(line);
			} else if (/^\d+\./.test(line)) { // Check if the line start with number "1.,2.,..."
				this.lawWordData.khoan.push(line);
			} else if (/^[a-z]\)/.test(line)) { // Check if the line start with an alphabet "a), b),..."
				this.lawWordData.diem.push(line);
			}
		});
	
		// Log hoặc xử lý thêm dữ liệu tách được
		console.log('Dữ liệu tách được:', this.lawWordData);
	}

	showSuccessMessage() {
		const successElement = document.querySelector('.success');
		successElement.classList.add('visible');
		setTimeout(() => {
			successElement.classList.remove('visible');
		}, 5000);
	}

	async getLaw() {
		const result = await this.appService.doGET('api/Law/GetLaw', null);
		if (result) {
			this.Law = result.Data;
			console.log(this.Law);
			
		}
	}

	async onSaveLaw() {
		console.log(this.nameLaw);
		
		this.lawData.Content = this.nameLaw;	
		const dataRequest = [this.lawData];
		
		const result = await this.appService.doPOST('api/ImportWordLaw/Saves', dataRequest);
		if (result && result.Status === 1) {
			this.notification.showSuccess(result.Msg);
			this.enabledImportWordFlg = false;
			// this.getManagePersonals();
		} else {
			this.appSwal.showWarning(result.Msg, false);
		}
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
