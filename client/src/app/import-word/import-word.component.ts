import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { SelectEvent, FileInfo } from '@progress/kendo-angular-upload';
import { State, process, SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { AppComponent } from '../app.component';
import { AppConsts } from '../services/app.consts';
import { AppControls } from '../services/app.controls';
import { AppFile } from '../services/app.file';
import { AppService } from '../services/app.service';
import { AppSwal } from '../services/app.swal';
import { AppUtils } from '../services/app.utils';
import { AuthenticationService } from '../services/authentication.service';
import { Notification } from '../services/app.notification';
import { AppLanguage } from '../services/app.language';
import { Router } from '@angular/router';
import { AppRouter } from '../app.router';

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
		LawNumber: string,
		LawDate: string,
		TotalChapter: number,
		Status: number,
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

	Law: Array<{ ID: number; Index: number; Title: string; Content: string; ContentHTML: string; }> = [];

	public uploadSaveUrl = 'saveUrl';
	public uploadRemoveUrl = 'removeUrl';

	filesUpload: Array<FileInfo>;
	filesUploadName = "";

	public fileSaveUrl: any;

	tempData = [
		{
			UserName: '',
			Name: '',
			GenderName: '',
			BirthDate: '',
			UnitName: '',
		}
	]

	constructor(
		private translate: TranslateService,
		private appService: AppService,
		private language: AppLanguage,
		private appSwal: AppSwal,
		public intl: IntlService,
		private router: Router,
		private notification: Notification,
		private file: AppFile,
		private authenticationService: AuthenticationService,
		public appControls: AppControls,
		private appConsts: AppConsts,
		private appComponent: AppComponent,
		public appUtils: AppUtils,
	) {
		this.authenticationService.getUser();
		this.user = this.authenticationService.user;
		this.setDefault();
		this.lawData = {
			// ID: number,
			Title: null,
			Content: null,
			ContentHTML: null,
			LawDate: null,
			LawNumber: null,
			TotalChapter: null,
			Status: null,
		}
	}

	async ngOnInit() {
		this.getLaw();
	}

	setDefault() {
		this.filesUpload = [];
		this.filesUploadName = "";
		this.fileSaveUrl = `${this.appService.apiRoot}api/Upload?dirName=${this.user.UserName}&typeData=files&acceptExtensions=[".doc",".docx"]`;
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
			// if (line.startsWith(', ngày')) {
			// 	this.lawWordData
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
		this.lawData.Content = this.nameLaw;
		const dataRequest = [this.lawData];
		const result = await this.appService.doPOST('api/ImportWordLaw/Saves', dataRequest);
		if (result && result.Status === 1) {
			this.notification.showSuccess(result.Msg);
			this.router.navigate([AppConsts.page.law]);
		} else {
			this.appSwal.showWarning(result.Msg, false);
		}
	}
}
