import { Component, OnInit, OnDestroy, ViewChild, ElementRef, Input, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AppService } from '../../services/app.service';
import { SelectableSettings, PageChangeEvent, GridDataResult, DataStateChangeEvent } from '@progress/kendo-angular-grid';
import { State, process, SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { AppSwal } from 'src/app/services/app.swal';
import { IntlService } from '@progress/kendo-angular-intl';
import { Notification } from '../../services/app.notification';
import { ExcelExportData } from '@progress/kendo-angular-excel-export';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/interval';
import { AppGuid } from 'src/app/services/app.guid';
import { NullInjector } from '@angular/core/src/di/injector';
import { FileRestrictions, SelectEvent, ClearEvent, RemoveEvent, FileInfo } from '@progress/kendo-angular-upload';
import * as XLSX from 'xlsx';
import { AppFile } from 'src/app/services/app.file';
import { nullSafeIsEquivalent } from '@angular/compiler/src/output/output_ast';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AppControls } from 'src/app/services/app.controls';
import { AppConsts } from 'src/app/services/app.consts';
import { AppComponent } from 'src/app/app.component';
import { AppUtils } from 'src/app/services/app.utils';
import { result } from 'underscore';
import { state } from '@angular/animations';

@Component({
  selector: 'app-accounts',
  templateUrl: './accounts.component.html',
  //styleUrls: ['./accounts.component.css']
})
export class AccountsComponent implements OnInit {
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
  ManagePersonalsChage: any;
  ManagePersonalsChange: any;
  ManagePersoManagePersonalsChangeNamenalsChageName: any;
  ManageListChangeName: any;

  @Input() isOriginComponent = true;
  @Input() organizationID: string = '';
  selectedUnitName = '';
  manageOrgPersonals = [];
  @ViewChild('excelexport1') excelexport1: any;
  @ViewChild('excelexport2') excelexport2: any;

  ViewHistoryCompetitionRewardDialog = false;

  constructor(
    private translate: TranslateService,
    private appService: AppService,
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
    this.getControl();
    this.setDefault();
    this.getPageName();
    this.setSelectableSettings();
  }

  async getPageName() {
    this.pageName = await this.appControls.getPageName();
  }

  ngOnDestroy(): void {

  }

  async ngOnInit() {

    await this.getUnitName();
    this.getManagePersonals();
  }

  async getControl() {
    this.control = await this.appControls.getControls(this.user.RoleID, AppConsts.page.accounts);
    this.controlDefault = false;
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
      SearchText: this.searchOption.SearchText,
      UnitID: this.searchOption.unitID ? this.searchOption.unitID : '',
    };

    const result = await this.appService.doGET('api/Account/Search', dataRequest);
    if (result && result.Data) {
      this.ManagePersonals = result.Data;
      for (let i = 0; i < this.ManagePersonals.length; i++) {
        this.ManagePersonals[i].Gender = this.ManagePersonals[i].Gender == 0 ? 'Nam' : 'Nữ'
        this.ManagePersonals[i].BirthDate = this.appUtils.getDateTimeFromformat(this.ManagePersonals[i].BirthDate, 'dd/MM/yyyy');
        this.ManagePersonals[i].YouthGroupDate = this.appUtils.getDateTimeFromformat(this.ManagePersonals[i].YouthGroupDate, 'dd/MM/yyyy');
        this.ManagePersonals[i].CommunistPartyDate = this.appUtils.getDateTimeFromformat(this.ManagePersonals[i].CommunistPartyDate, 'dd/MM/yyyy');
      }
      this.ManagePersonalState.skip = 0;
      this.dataManagePersonalSkip = 0;
      if (this.isOriginComponent == false) {
        this.ManageOrgPersonals();
      } else {
        this.bindManagePersonals();
      }


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
    this.request_AccountID = this.GUID_EMPTY;
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



  async onOpenHistoryCompetitionRewardDialog(ID) {
    var selectedItem = this.ManagePersonals.find((item) => {
      return item.ID === ID;
    });
    this.dataManagePersonalItem = selectedItem;
    this.request_AccountID = this.dataManagePersonalItem.ID;
    this.ViewHistoryCompetitionRewardDialog = true;

  }
  async onCloseHistoryCompetitionRewardDialog() {
    this.ViewHistoryCompetitionRewardDialog = false;

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
    else if (e.id == 'ViewHistory') {
      this.onOpenHistoryCompetitionRewardDialog(dataItem.ID)
    }
  }
  onFunctionIconClick(dataItem) {
    this.getbtnFunctionData(dataItem);
  }

  getbtnFunctionData(dataItem) {

    this.btnFunctionData = [];
    this.btnMbFunctionData = [];

    // if (this.controlDefault || this.control.ViewHistory) {
    //   var func3 = {
    //     text: this.translate.instant('ViewHistory'),
    //     id: 'ViewHistory',
    //     icon: 'preview',
    //   };
    //   this.btnFunctionData.push(func3);
    //   this.btnMbFunctionData.push(func3);
    // }

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
    await this.getFilter();
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


  async getFilter() {

    const dataRequest = {
      langID: localStorage.getItem('currentLanguage') != "en-US" ? this.selectedLangID : localStorage.getItem('currentLanguage')
    };
    const resultCatolories = await this.appService.doGET("api/Unit", dataRequest);
    if (resultCatolories && resultCatolories.Status === 1) {
      this.units = resultCatolories.Data;
    }

    const resultPosition = await this.appService.doGET('api/CommonMenu/GetPositions', dataRequest);
    if (resultPosition && resultPosition.Status === 1) {
      this.positions = resultPosition.Data;
    }

    const resultGender = await this.appService.doGET('api/Enum/GetEGender', null);
    if (resultGender && resultGender.Status === 1) {
      this.genders = resultGender.Data;
    }

    const resultCommon = await this.appService.doGET("api/CommonMenu", null);
    if (resultCommon && resultCommon.Status === 1) {
      this.ethnic = resultCommon.Data;
      this.ethnic = this.ethnic.filter(x => x.Type == this.Enum.Ethnic);

      this.politicalTheory = resultCommon.Data;
      this.politicalTheory = this.politicalTheory.filter(x => x.Type == this.Enum.PoliticalTheory);

      this.communistPartyPosition = resultCommon.Data;
      this.communistPartyPosition = this.communistPartyPosition.filter(x => x.Type == this.Enum.CommunistPartyPosition);

      this.qualification = resultCommon.Data;
      this.qualification = this.qualification.filter(x => x.Type == this.Enum.Qualification);
    }

  }

  async loadXLSX(e) {

  }

  async onSavePersonals() {
    const dataRequests = [];
    for (let i = 0; i < this.personalsTemp.length; i++) {
      dataRequests.push(this.personalsTemp[i]);
    }
    const result = await this.appService.doPOST('api/Account/Saves', dataRequests);
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.enabledImportExcelFlg = false;
      this.getManagePersonals();
    } else {
      this.appSwal.showWarning(result.Msg, false);
    }
  }

  onExportExcelTemp(excelexport) {
    excelexport.save();
  }
  onExportExcel(excelexport) {
    excelexport.save();
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
  ManageOrgPersonals() {
    let unit = this.organizationID;
    this.manageOrgPersonals = this.ManagePersonals.filter(s => s.UnitID == unit);

    this.ManagePersonalChange.skip = 0;
    this.dataManagePersonalSkip = 0;
    this.ManagePersonalState = {
      skip: this.dataManagePersonalSkip,
      take: this.dataManagePersonalSkip + this.dataManagePersonalPageSize,
      filter: {
        logic: 'and',
        filters: []
      }
    };
    this.bindManageOrgPersonals();
  }
  bindManageOrgPersonals() {
    this.ManagePersonalGridDataResult = {
      data: orderBy(this.manageOrgPersonals, this.ManagePersonalSortByField),
      total: this.manageOrgPersonals.length
    };
    this.ManagePersonalGridDataResult = process(this.manageOrgPersonals, this.ManagePersonalState);
  }
}

function value(value: any) {
  throw new Error('Function not implemented.');
}


