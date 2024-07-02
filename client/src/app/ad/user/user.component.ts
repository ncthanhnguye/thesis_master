import { Component, OnInit, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AppService } from '../../services/app.service';
import { SelectableSettings, PageChangeEvent, GridDataResult, DataStateChangeEvent } from '@progress/kendo-angular-grid';
import { State, process, SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { AppSwal } from 'src/app/services/app.swal';
import { IntlService } from '@progress/kendo-angular-intl';
import { Notification } from '../../services/app.notification';
import { ExcelExportData } from '@progress/kendo-angular-excel-export';
import { AppGuid } from 'src/app/services/app.guid';
import { SelectEvent } from '@progress/kendo-angular-upload';
import { AppFile } from 'src/app/services/app.file';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AppControls } from 'src/app/services/app.controls';
import { AppUtils } from 'src/app/services/app.utils';
import { AppComponent } from '../../app.component';
import { AppConsts } from 'src/app/services/app.consts';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  //styleUrls: ['./user.component.css'],
})
export class UserComponent implements OnInit, OnDestroy {
  user: any;
  loading = false;
  dataUsers = [];
  dataUserSelectableSettings: SelectableSettings;
  dataUserSort = {
    allowUnsort: true,
    mode: 'multiple'
  };
  public dataUserFocus = {
    Name: true
  };
  dataUserSortByField: SortDescriptor[] = [
  ];

  public WKS_NUM_PAGING_SKIP = 0;
  public WKS_NUM_PAGING_TAKE = 10;
  public WKS_NUM_PAGING_BTN = 5;

  dataUserSkip = this.WKS_NUM_PAGING_SKIP;
  dataUserPageSize = this.WKS_NUM_PAGING_TAKE;
  dataUserSelection: number[] = [];
  dataUserItem: any;
  myInterval: any;

  public buttonCount = this.WKS_NUM_PAGING_BTN;
  public info = true;
  public type: 'numeric' | 'input' = 'numeric';
  public pageSizes = true;
  public previousNext = true;

  public dataUserState: State = {
    skip: this.dataUserSkip,
    take: this.dataUserSkip + this.dataUserPageSize,
    filter: {
      logic: 'and',
      filters: [{ field: 'RoleName', operator: 'contains', value: '' }]
    },
    // group: [{ field: 'RoleName' }]
  };
  dataUserGridDataResult: GridDataResult;
  roles: Array<{ Name: string, ID: string }>;
  rolesFilter: Array<{ Name: string, ID: string }>;
  accountsFilter: Array<{ Name: string, ID: string }>;
  accounts: Array<{ Name: string, ID: string }>;


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

  dataUserItemtemp: any;
  userDialogOpened = false;
  pageName: any;

  public ManagePersonalOpened = false;
  public request_AccountID;
  public unitIDInput ='';
  public GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
  isSummaryInfoCofig = false;
  popupClass = "popup-width";
  dataErr: any;

  constructor(
    private translate: TranslateService,
    private appService: AppService,
    private appSwal: AppSwal,
    public intl: IntlService,
    private notification: Notification,
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

  // used to export all kendo-grid data(not care for paging) to excel file(s)
  public allData(): ExcelExportData {
    // show loading icon to let user know heavy-export is occurring
    this.appComponent.loading = true;
    const result: ExcelExportData = {
      data: process(this.dataUsers, { sort: this.dataUserSortByField }).data
    };
    // unshow loading icon
    this.appComponent.loading = false;
    return result;
  }

  rolesHandleFilter(value) {
    this.rolesFilter = this.roles.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  accountsHandleFilter(value) {
    this.accountsFilter = this.accounts.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  ngOnDestroy(): void {
    if (this.myInterval) { this.myInterval.unsubscribe(); }
  }

  ngOnInit() {
  }

  async getControl() {
    this.control = await this.appControls.getControls(this.user.RoleID, AppConsts.page.user);
    this.controlDefault = false;
  }

  setSelectableSettings(): void {
    this.allowMulti = !this.allowMulti;
    this.dataUserSelectableSettings = {
      checkboxOnly: false,
      mode: this.allowMulti ? 'multiple' : 'single'
    };
  }

  setSelectableSettingsFile(): void {
    this.dataUserSelectableSettings = {
      enabled: true,
      checkboxOnly: false,
      mode: this.allowMulti ? 'multiple' : 'single'
    };
  }

  onSearchKeyPress(e: any) {
    if (e.keyCode === 13 && this.searchOption.SearctText) {
      this.onSearch();
    }
  }

  async getUsers() {
    this.loading = true;
    const dataRequest = {
      searchText: this.searchOption.SearctText,
      UnitID: this.user.UnitID
    };

    const result = await this.appService.doGET('api/User/Search', dataRequest);
    if (result) {
      this.dataUsers = result.Data;
      this.bindUsers();
    }
    this.loading = false;
    this.checkSelectionID();
  }

  checkSelectionID() {
    // tslint:disable-next-line:prefer-for-of
    for (let i = this.dataUserSelection.length - 1; i >= 0; i--) {
      const selectedItem = this.dataUsers.find((item) => {
        return item.UserName === this.dataUserSelection[i];
      });
      if (!selectedItem) {
        this.dataUserSelection.splice(i, 1);
      }
    }
  }

  setDefault() {
    this.dataUserItem = {
      IsAdd: true,
      UserName: null,
      Password: null,
      AccountID: null,
      RoleID: null,
      LockFlg: false,
    };
    this.dataUserItemtemp = Object.assign({}, this.dataUserItem);
    var errTemp = Object.assign({}, this.dataUserItemtemp);
    errTemp.Type = null;
    this.dataErr = [errTemp];
  }
  bindtemp(item) {
    this.dataUserItemtemp = Object.assign({}, this.dataUserItem);
  }

  onUserPageChange(event: PageChangeEvent) {
    this.dataUserSkip = event.skip;
    this.bindUsers();
  }

  onUserSelectedKeysChange() {

  }

  bindUsers() {
    this.dataUserGridDataResult = {
      data: orderBy(this.dataUsers, this.dataUserSortByField),
      total: this.dataUsers.length
    };

    this.dataUserGridDataResult = process(this.dataUsers, this.dataUserState);
  }

  onUserSortChange(sort: SortDescriptor[]): void {
    this.dataUserSortByField = sort;
    this.bindUsers();
  }

  public onUserDataStateChange(state: DataStateChangeEvent): void {
    this.dataUserSelection = [];
    this.dataUserState = state;
    this.dataUserGridDataResult = process(this.dataUsers, this.dataUserState);
  }

  getColumnIndex(name) {
    const columns = [
      'UserName',
      'Password',
      'AccountID',
      'RoleID'
    ];
    return columns.indexOf(name);
  }

  async selectEventHandler(e: SelectEvent) {
    this.appComponent.loading = true;
    const fileData = (await this.file.readXLSX(e.files[0].rawFile)) as Array<any>;
    this.setSelectableSettings();
    this.dataUsers = [];
    let account;
    let role;
    for (let i = 1; i < fileData.length; i++) {

      account = this.appUtils.getObjectByList(this.accounts, fileData[i][this.getColumnIndex('AccountID')]);
      role = this.appUtils.getObjectByList(this.roles, fileData[i][this.getColumnIndex('RoleID')]);

      this.dataUsers.push({
        IsAdd: false,
        UserName: fileData[i][this.getColumnIndex('UserName')],
        Password: fileData[i][this.getColumnIndex('Password')],
        AccountID: account ? account.ID : null,
        AccountName: account ? account.Name : null,
        RoleID: role ? role.ID : null,
        RoleName: role ? role.Name : null,
        LockFlg: false,
        DelFlg: false
      });
    }
    this.bindUsers();
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

    const result = await this.appService.doGET('api/Account/NoAccountFilter', null );
    if (result) {

      this.accounts = result.Data;
      this.accountsFilter = this.accounts.slice();
    }

    const resultSummaryInfoCofig = await this.appService.doGET('api/Config/GetSummaryInfoUser', null);
    if (resultSummaryInfoCofig && resultSummaryInfoCofig.Status === 1) {
      this.isSummaryInfoCofig = resultSummaryInfoCofig.Data;
    }
  }

  onSearch() {
    this.getUsers();
    this.isEnabledSaveAll = false;
  }

  onReload() {
    this.setSelectableSettingsFile();
    this.searchOption.SearctText = '';
    this.getUsers();
    this.isEnabledSaveAll = false;
  }

  onClearUser() {
    this.setDefault();
  }

  onAllowSelectMulti() {
    this.setSelectableSettings();
  }

  onAddNewUser() {
    this.enabled = true;
    this.enabledID = true;
    this.userDialogOpened = true;
    this.setDefault();
  }

  onSaveUser() {
    if (this.dataUserItem.IsAdd) { this.addUser(); } else { this.updateUser(); }
  }

  async onSaveUsers() {
    this.appComponent.loading = true;
    const dataRequests = [];
    for (let i = 0; i < this.dataUsers.length; i++) {
      dataRequests.push(this.createDataRequest(this.dataUsers[i]));
    }
    if (dataRequests.length < 1) {
      this.onReload();
      this.isEnabledSaveAll = false;
      this.appComponent.loading = false;
      this.appSwal.showWarning(this.translate.instant('Error_NoItem_Save'), false);
      return;
    }
    const result = await this.appService.doPOST('api/User/Saves', dataRequests);
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
    const temp = data ? data : this.dataUserItemtemp;
    var rolesID : any[] ; ;

    if (this.dataUserItemtemp.RoleID) {
      rolesID = [] ;
      for (let i = 0; i < this.dataUserItemtemp.RoleID.length; i++) {
        rolesID.push(this.dataUserItemtemp.RoleID[i]);
      }
      if(rolesID.length === 0 ){
        rolesID = null ;
      }
    }

    return {
      UserName: temp.UserName,
      Password: temp.Password,
      AccountID: temp.AccountID,
      RoleID: rolesID ? JSON.stringify(rolesID) : null ,
      LockFlg: temp.LockFlg,
      UpdateAt: temp.UpdateAt,
    };
  }

  onCloseUser(status: any) {
    this.enabled = false;
    this.enabledID = false;
  }

  onEditUser(selectedID) {
    const selectedItem = this.dataUsers.find((item) => {
      return item.UserName === selectedID;
    });
    if (selectedItem){
    selectedItem.IsAdd = false;
    if(typeof selectedItem.RoleID == 'string') {
      var arr = [];
      try {
        arr = JSON.parse(selectedItem.RoleID);
    } catch(e) {
        arr.push(selectedItem.RoleID);
    }
    selectedItem.RoleID = arr;
    }

    this.dataUserItem = selectedItem;
    this.bindtemp(this.dataUserItem);
    this.enabled = true;
    this.enabledID = false;
    this.userDialogOpened = true;
    }
  }

  async addUser() {
    this.appComponent.loading = true;
    const dataRequest = this.createDataRequest(null);
    const result = await this.appService.doPOST('api/User', dataRequest);
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onReload();
      this.onAddNewUser();
      this.userDialogOpened = false;
      this.appComponent.closeDialog();
    } else {
      if (!result.Msg) {
        this.dataErr = result.Data;
        // console.log(result)
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
    this.appComponent.loading = false;
  }

  async updateUser() {
    this.appComponent.loading = true;
    const userName = this.dataUserItem.UserName;
    const dataRequest = this.createDataRequest(null);

    const result = await this.appService.doPUT('api/User', dataRequest, { userName });
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onReload();
      this.userDialogOpened = false;
      this.appComponent.closeDialog();
    } else {
      if (!result.Msg) {
        this.dataErr = result.Data;
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
    this.appComponent.loading = false;
  }

  async onDeleteUser(selectedID) {
    this.appComponent.loading = true;
    const dataRequest = {
      userName: selectedID
    };
    const option = await this.appSwal.showWarning(this.translate.instant('AreYouSure'), true);
    if (option) {
      const result = await this.appService.doDELETE('api/User', dataRequest);
      if (result && result.Status === 1) {
        this.notification.showSuccess(result.Msg);
        this.onReload();
        this.onAddNewUser();
        this.enabled = false;
        this.enabledID = false;
        this.dataUserSelection = [];
        this.allowMulti = false;
        this.userDialogOpened = false;
        this.appComponent.closeDialog();
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
    this.appComponent.loading = false;
  }

  onCloseUserDialog(status){
    this.userDialogOpened = false;
    this.appComponent.closeDialog();
  }

  onEditAccountInfor(accountID, unitID){
    this.request_AccountID = accountID ;
    this.unitIDInput = unitID;
    this.ManagePersonalOpened = true;
  }

  onAddNewAccount(){
    this.request_AccountID = this.GUID_EMPTY ;
    this.ManagePersonalOpened = true;
  }

  onCloseMember(e) {
    this.onReload();
    this.ManagePersonalOpened = false;
  }

  async setNewAccount(e: any) {

    if (e && e.Name == 'successAdd'){
      if (e.Account){
        const result = await this.appService.doGET('api/Account/Search', { searchText: '' });
        if (result) {

          this.accounts = result.Data;
          this.accountsFilter = this.accounts.slice();
        }
        if (e.Account.SttResultFromUser) {
          this.ManagePersonalOpened = false;
          if (this.accountsFilter.find(r => r.ID === e.Account.ID)){
            this.dataUserItemtemp.AccountID = e.Account.ID;
          }
          this.onReload();
        }
      }
    }
  }
  filterInput(event) {
    var format = /[`!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/;

    return ["Backspace", "Delete", "ArrowLeft", "ArrowRight"].includes(
      event.code
    )
      ? true
      : (isNaN(Number(event.key)) || event.code == "Space") &&
          format.test(event.key) == false;
  }
}
