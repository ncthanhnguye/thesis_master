import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AppService } from '../../services/app.service';
import { SelectableSettings, PageChangeEvent, GridDataResult, DataStateChangeEvent } from '@progress/kendo-angular-grid';
import { State, process, SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { AppSwal } from 'src/app/services/app.swal';
import { IntlService } from '@progress/kendo-angular-intl';
import { Notification } from '../../services/app.notification';
import { ExcelExportData } from '@progress/kendo-angular-excel-export';
import { Observable } from 'rxjs/Observable';
import { AppGuid } from 'src/app/services/app.guid';
import { NullInjector } from '@angular/core/src/di/injector';
import { FileRestrictions, SelectEvent, ClearEvent, RemoveEvent, FileInfo } from '@progress/kendo-angular-upload';
import * as XLSX from 'xlsx';
import { AppFile } from 'src/app/services/app.file';
import { nullSafeIsEquivalent } from '@angular/compiler/src/output/output_ast';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AppControls } from 'src/app/services/app.controls';
import { AppUtils } from 'src/app/services/app.utils';
import { AppComponent } from '../../app.component';
import { AppConsts } from 'src/app/services/app.consts';
import 'rxjs/add/observable/interval';
import { isEmpty } from 'underscore';


@Component({
  selector: 'app-control',
  templateUrl: './control.component.html',
  //styleUrls: ['./control.component.css']
})
export class ControlComponent implements OnInit, OnDestroy {

  user: any;
  loading = false;
  dataControls = [];
  dataControlSelectableSettings: SelectableSettings;
  dataControlSort = {
    allowUnsort: true,
    mode: 'multiple'
  };
  public dataControlFocus = {
    Name: true
  };
  dataControlSortByField: SortDescriptor[] = [
    // {
    //   field: 'ParentName',
    //   dir: 'asc'
    // }, {
    //   field: 'OrdinalNumber',
    //   dir: 'asc'
    // }
  ];

  //Config: Constant for paging
  public WKS_NUM_PAGING_SKIP = 0;
  public WKS_NUM_PAGING_TAKE = 10;
  public WKS_NUM_PAGING_BTN = 5;

  dataControlSkip = this.WKS_NUM_PAGING_SKIP;
  dataControlPageSize = this.WKS_NUM_PAGING_TAKE;
  dataControlSelection: number[] = [];
  dataControlItem: any;
  dataControlItemtemp: any;
  myInterval: any;

  public buttonCount = this.WKS_NUM_PAGING_BTN;
  public info = true;
  public type: 'numeric' | 'input' = 'numeric';
  public pageSizes = true;
  public previousNext = true;

  public dataControlState: State = {
    skip: this.dataControlSkip,
    take: this.dataControlSkip + this.dataControlPageSize,
    filter: {
      logic: 'and',
      filters: []
    }
  };
  dataControlGridDataResult: GridDataResult;

  roles: Array<{ Name: string, ID: string }>;
  rolesFilter: Array<{ Name: string, ID: string }>;


  public uploadSaveUrl = 'saveUrl';
  public uploadRemoveUrl = 'removeUrl';
  public enabled = false;
  public enabledID = false;
  isEnabledSaveAll = false;
  control: any;
  controlDefault = true;
  allowMulti = true;

  searchOption = {
    SearctText: ''
  };
  pageName: any;
  addCommon = false;
  dataErr: any;
  customCss = 0;
  pageUrl: Array<{ Name: string, ID: string }>;
  pageUrlsFilter: Array<{ Name: string, ID: string }>;
  defaultUrls = AppConsts.page;
  methods: Array<String> = ["GET","POST","PUT","DELETE"];


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
    private appUtils: AppUtils,
    private appComponent: AppComponent,
  ) {
    this.authenticationService.getUser();
    this.user = this.authenticationService.user;
    this.getControl();
    this.setDefault();
    this.setSelectableSettings();
    this.onReload();
    this.initDisplay();
    this.getPageName();
  }

  async getPageName() {
    this.pageName = await this.appControls.getPageName();
  }

  rolesHandleFilter(value) {
    this.rolesFilter = this.roles.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  ngOnDestroy(): void {
    if (this.myInterval) { this.myInterval.unsubscribe(); }
  }

  ngOnInit() {
    this.pageUrl = Object.values(this.defaultUrls).map((item) => {
      var container: {ID: string, Name: string};
      container = {
        ID: item,
        Name: item
      }
  
      return container;
  });
    this.pageUrlsFilter = this.pageUrl.filter((s) => !isEmpty(s.Name)).slice();
  }

  pageUrlsHandleFilter(value) {
    this.pageUrlsFilter = this.pageUrl.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  async getControl() {
    this.control = await this.appControls.getControls(this.user.RoleID, AppConsts.page.control);
    this.controlDefault = false;
  }

  setSelectableSettings(): void {

    this.allowMulti = !this.allowMulti;
    this.dataControlSelectableSettings = {
      checkboxOnly: false,
      mode: this.allowMulti ? 'multiple' : 'single'
    };
  }

  onSearchKeyPress(e: any) {
    if (e.keyCode === 13 && this.searchOption.SearctText) {
      this.onSearch();
    }
  }

  async getControls() {
    this.loading = true;
    const dataRequest = {
      searchText: this.searchOption.SearctText
    };

    const result = await this.appService.doGET('api/Control/Search', dataRequest);
    if (result) {
      this.dataControls = result.Data;
      this.bindControls();
    }
    this.loading = false;
    this.checkSelectionID();
  }

  checkSelectionID() {
    // tslint:disable-next-line:prefer-for-of
    for (let i = this.dataControlSelection.length - 1; i >= 0; i--) {
      const selectedItem = this.dataControls.find((item) => {
        return item.ID === this.dataControlSelection[i];
      });
      if (!selectedItem) {
        this.dataControlSelection.splice(i, 1);
      }
    }
  }

  setDefault() {
    this.dataControlItem = {
      IsAdd: true,
      ControlID: '',
      Name: '',
      AbsoluteUrl: '',
      MethodRequest: '',
      PageID: ''
    };
    this.dataControlItemtemp = {
      IsAdd: true,
      ControlID: '',
      Name: '',
      AbsoluteUrl: '',
      MethodRequest: '',
      PageID: ''


    };
    var errTemp = Object.assign({}, this.dataControlItemtemp);
    errTemp.Type = null;
    this.dataErr = [errTemp];
    this.customCss = 0;

  }

  onControlPageChange(event: PageChangeEvent) {
    this.dataControlSkip = event.skip;
    this.bindControls();
  }

  bindtemp(item){
    this.dataControlItemtemp.ControlID = item.ControlID;
    this.dataControlItemtemp.Name = item.Name;
    this.dataControlItemtemp.AbsoluteUrl = item.AbsoluteUrl;
    this.dataControlItemtemp.MethodRequest = item.MethodRequest;
    this.dataControlItemtemp.PageID = item.PageID;


  }

  // onControlSelectedKeysChange() {

  //   if (this.dataControlSelection.length === 0) {
  //     this.appSwal.showWarning(this.translate.instant('NoRecordSelected'), false);
  //     return;
  //   }

  //   if (this.dataControlSelection.length > 1) {
  //     if (this.allowMulti) {
  //       return;
  //     }
  //     this.appSwal.showWarning(this.translate.instant('SelectSingle'), false);
  //   } else {
  //     const selectedID = this.dataControlSelection[0];
  //     const selectedItem = this.dataControls.find((item) => {
  //       return item.ControlID === selectedID;
  //     });
  //     selectedItem.IsAdd = false;
  //     this.dataControlItem = selectedItem;
  //     this.bindtemp(this.dataControlItem);
  //     this.enabled = false;
  //     this.enabledID = false;
  //   }
  // }

  bindControls() {
    this.dataControlGridDataResult = {
      data: orderBy(this.dataControls, this.dataControlSortByField),
      total: this.dataControls.length
    };

    this.dataControlGridDataResult = process(this.dataControls, this.dataControlState);
  }

  onControlSortChange(sort: SortDescriptor[]): void {
    this.dataControlSortByField = sort;
    this.bindControls();
  }

  public onControlDataStateChange(state: DataStateChangeEvent): void {
    this.dataControlSelection = [];
    this.dataControlState = state;
    this.dataControlGridDataResult = process(this.dataControls, this.dataControlState);
  }

  getColumnIndex(name) {
    const columns = [
      'ControlID',
      'Name',
      'AbsoluteUrl',
      'MethodRequest',
      'PageID'
      
    ];

    return columns.indexOf(name);
  }

  async selectEventHandler(e: SelectEvent) {
    this.appComponent.loading = true;
    const fileData = (await this.file.readXLSX(e.files[0].rawFile)) as Array<any>;
    let role: any;
    this.dataControls = [];
    for (let i = 1; i < fileData.length; i++) {
      role = this.roles.find(item => {
        return this.appUtils.compareString(fileData[i][this.getColumnIndex('RoleID')], item.Name, item.ID);
      });

      if (fileData[i].indexOf(fileData[i][this.getColumnIndex('RoleName')]) === -1) {
        this.dataControls.push({
          IsAdd: false,
          ID: fileData[i][this.getColumnIndex('ID')],
          Name: fileData[i][this.getColumnIndex('Name')]
        });
      }
    }
    this.bindControls();
    this.isEnabledSaveAll = true;
    this.appComponent.loading = false;
  }

  removeEventHandler() {
    this.isEnabledSaveAll = false;
    this.onReload();
  }



  onSearchTextChange(e: any) {
    if (!this.searchOption.SearctText) {
      this.onReload();
    }
  }

  onRemoveSearchText() {
    this.searchOption.SearctText = '';
    this.onReload();

  }

  async initDisplay() {
    const resultRole = await this.appService.doGET('api/Role', null);
    if (resultRole && resultRole.Status === 1) {
      this.roles = resultRole.Data;
      this.rolesFilter = this.roles.slice();
    }
  }

  onSearch() {
    this.getControls();
    this.isEnabledSaveAll = false;
  }

  onReload() {
    this.searchOption.SearctText = '';
    this.getControls();
    this.isEnabledSaveAll = false;
  }

  onClearControl() {
    this.setDefault();
  }

  onAllowSelectMulti() {
    this.setSelectableSettings();
  }

  onAddNewControl() {
    this.enabled = true;
    this.enabledID = true;
    this.setDefault();
  }

  onSaveControl() {
    if (this.dataControlItem.IsAdd) { this.addControl(); } else { this.updateControl(); }
  }

  async onSaveControls() {
    this.appComponent.loading = true;
    const dataRequests = [];
    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < this.dataControls.length; i++) {
      dataRequests.push(this.createDataRequest(this.dataControls[i]));
    }
    const result = await this.appService.doPOST('api/Control/Saves', dataRequests);
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onReload();
      this.isEnabledSaveAll = false;
    } else {
      this.appSwal.showWarning(result.Msg, false);
    }
    this.appComponent.loading = false;
  }

  createDataRequest(data) {
    const temp = data ? data : this.dataControlItem;
    return {
      iD: temp.ID,
      ControlID: temp.ControlID,
      Name: temp.Name,
      AbsoluteUrl: temp.AbsoluteUrl,
      MethodRequest: temp.MethodRequest,
      PageID: temp.PageID
    };
  }
  createtempDataRequest(data) {
    const temp = data ? data : this.dataControlItemtemp;
    return {
      iD: temp.ID,
      ControlID: temp.ControlID,
      Name: temp.Name,
      AbsoluteUrl: temp.AbsoluteUrl,
      MethodRequest: temp.MethodRequest,
      PageID: temp.PageID
    };
  }

  onCloseControl(status: any) {
    this.enabled = false;
    this.enabledID = false;
    this.setDefault()
  }

  onEditControl(selectedID) {
    const selectedItem = this.dataControls.find((item) => {
      return item.ID === selectedID;
    });
    if (selectedItem){
      selectedItem.IsAdd = false;
      this.dataControlItem = selectedItem;
      this.bindtemp(this.dataControlItem);
    }
    this.enabled = true;
    this.enabledID = false;
    this.addCommon = true;
  }

  async addControl() {
    this.appComponent.loading = true;
    const dataRequest = this.createtempDataRequest(null);
    const result = await this.appService.doPOST('api/Control', dataRequest);
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onReload();
      this.onAddNewControl();
      this.addCommon = false;
    } else {
      this.dataErr = result.Data;
        var count = 0;
        for (var prop in this.dataErr[0]) {
          count++;
        }
        this.customCss = count;
    }
    this.appComponent.loading = false;
  }

  async updateControl() {
    this.appComponent.loading = true;
    const iD = this.dataControlItem.ID;
    this.dataControlItemtemp.ID = iD;
    const dataRequest = this.createtempDataRequest(null);

    const result = await this.appService.doPUT('api/Control', dataRequest, { iD });
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onReload();
      this.setDefault();
      this.addCommon = false;
    } else {
      this.dataErr = result.Data;
      var count = 0;
      for (var prop in this.dataErr[0]) {
        count++;
      }
      this.customCss = count;
    }
    this.appComponent.loading = false;
  }

  async onDeleteControl(selectedID) {
    
    this.appComponent.loading = true;
    const dataRequest = {
      ID: selectedID,
    };

    const option = await this.appSwal.showWarning(this.translate.instant('AreYouSure'), true);
    if (option) {
      const result = await this.appService.doDELETE('api/Control', dataRequest);
      if (result && result.Status === 1) {
        this.notification.showSuccess(result.Msg);
        this.onReload();
        this.onAddNewControl();
        this.enabled = false;
        this.enabledID = false;
        this.dataControlSelection = [];
        this.allowMulti = false;
        this.addCommon = false;
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
    this.appComponent.loading = false;
  }

  onAddNewCommon() {
    this.setDefault();
    this.addCommon = true;
  }

  onCloseUnitDialog() {
    this.addCommon = false;
    this.setDefault()
    this.onReload();
  }
  onEnterControlID(event): boolean {
    
    const charCode = (event.which) ? event.which : event.keyCode;
  
    // let num : number = parseFloat((<HTMLInputElement>document. getElementById(id)).value);
    if (charCode > 32 && charCode < 225 ) {
  
      return true;
    }
    return false;
  }
}

