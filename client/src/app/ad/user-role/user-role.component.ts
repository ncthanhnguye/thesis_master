import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AppService } from '../../services/app.service';
import { SelectableSettings, PageChangeEvent, GridDataResult, DataStateChangeEvent } from '@progress/kendo-angular-grid';
import { State, process, SortDescriptor, orderBy, CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { AppSwal } from 'src/app/services/app.swal';
import { IntlService } from '@progress/kendo-angular-intl';
import { Notification } from '../../services/app.notification';
import { ExcelExportData } from '@progress/kendo-angular-excel-export';
import { CheckableSettings, CheckedState } from '@progress/kendo-angular-treeview';
import { Observable } from 'rxjs/Observable';
import { AppGuid } from 'src/app/services/app.guid';
import { NullInjector } from '@angular/core/src/di/injector';
import { FileRestrictions, SelectEvent, ClearEvent, RemoveEvent, FileInfo } from '@progress/kendo-angular-upload';
import * as XLSX from 'xlsx';
import { AppFile } from 'src/app/services/app.file';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { of } from 'rxjs/observable/of';
import { AppComponent } from 'src/app/app.component';
import { AppControls } from 'src/app/services/app.controls';
import { AppConsts } from 'src/app/services/app.consts';

@Component({
  selector: 'app-user-role',
  templateUrl: './user-role.component.html',
  //styleUrls: ['./user-role.component.css']
})
export class UserRoleComponent implements OnInit, OnDestroy {

  user: any;
  loading = false;
  userRoles = [];
  userRoleFilter = [];
  unGranteduserRoleFilter = [];
  GranteduserRoleFilter = [];
  unGranteduserRole = [];
  GranteduserRole = [];
  searchUserRoleFilter = [];
  searchUserRoles = [];
  userUnGrantedRoleSelectedKeys = [];
  userGrantedRoleSelectedKeys = [];
  userRoleSelectableSettings: SelectableSettings;
  userRoleSort = {
    allowUnsort: true,
    mode: 'multiple'
  };

  userRoleSortByField: SortDescriptor[] = [];

  searchOption = {
    RoleID: null,
    SearchText1: '',
    SearchText2: ''
  };

  public userRoleState: State = {
    filter: {
      logic: 'and',
      filters: []
    }
  };
  userRoleGridDataResult: GridDataResult;

  userRoleSkip = 0;
  userRolePageSize = 5;
  userRoleSelection: number[] = [];
  userRoleItem: any;
  myInterval: any;

  roles: Array<{ Name: string, ID: string }>;
  roleFilters: Array<{ Name: string, ID: string }>;
  roleSearchs: Array<{ Name: string, ID: string }>;

  public updateFilePathUrl = this.appService.apiRoot + 'File/Upload';
  public uploadRemoveUrl = 'removeUrl';
  public userRoleOpened = false;
  control: any;
  controlDefault = true;
  selectedAll = false;
  pageName: any;

  constructor(
    private translate: TranslateService,
    private appService: AppService,
    private appSwal: AppSwal,
    public intl: IntlService,
    private notification: Notification,
    private guid: AppGuid,
    private file: AppFile,
    private authenticationService: AuthenticationService,
    private location: Location,
    private appComponent: AppComponent,
    public appControls: AppControls
  ) {

    this.authenticationService.getUser();
    this.user = this.authenticationService.user;
    this.getControl();
    this.initDisplay();
    this.getPageName();
  }

  async getPageName() {
    this.pageName = await this.appControls.getPageName();
  }

  ngOnDestroy(): void {
    if (this.myInterval) { this.myInterval.unsubscribe(); }
  }

  ngOnInit() {
  }

  async getControl() {
    this.control = await this.appControls.getControls(this.user.RoleID, AppConsts.page.userRole);
    this.controlDefault = false;
  }

  // setSelectableSettings(): void {
  //   this.userRoleSelectableSettings = {
  //     checkboxOnly: false,
  //     mode: 'multiple'
  //   };
  // }

  async getUserRoles() {
    this.appComponent.loading = true;
    if (this.searchOption.RoleID && this.searchOption.RoleID !== this.guid.empty) {
      const dataRequest = {
        id: this.searchOption.RoleID
      };
      this.loading = true;
      const result = await this.appService.doGET('api/RolePage', dataRequest);
      if (result) {
        this.userRoles = result.Data || [];
      }
      this.loading = false;
    } else {
      this.userRoles = [];
    }

    this.userRoleFilter = this.userRoles.slice();
    this.onSearchTextChange2(this.searchOption.SearchText2);
    this.onSearchTextChange1(this.searchOption.SearchText1);
    this.appComponent.loading = false;
  }


  onUserRolePageChange(event: PageChangeEvent) {
    this.userRoleSkip = event.skip;
  }

  onUserRoleSelectedKeysChange() {
  }

  userRoleHandleSelection({ index }: any) {
    if (typeof index !== "undefined" && index != null) {
      if (index.indexOf('_') < 0) {
        const i = this.intl.parseNumber(index);
        const page = this.userRoleFilter[i];
        if (page.Items && page.Items.length > 0) {
          page.Items.forEach((item) => {
            item.ActiveFlg = !page.ActiveFlg;
          });
        }
      }
    }
  }

  public search(items: any[], term: string): any[] {
    return items.reduce((acc, item) => {
      if (this.contains(item.Name, term)) {
        acc.push(item);
      } else if (item.Items && item.Items.length > 0) {
        const newItems = this.search(item.Items, term);

        if (newItems.length > 0) {
          acc.push({ ID: item.ID, Name: item.Name, ActiveFlg: item.ActiveFlg, Items: newItems });
        }
      }

      return acc;
    }, []);
  }

  public contains(text: string, term: string): boolean {
    return text == null ? false : text.toLowerCase().indexOf(term.toLowerCase()) >= 0;
  }

  onUserRoleSortChange(sort: SortDescriptor[]): void {
    this.userRoleSortByField = sort;
  }

  public onUserRoleDataStateChange(state: DataStateChangeEvent): void {
    this.userRoleSelection = [];
    this.userRoleState = state;
    this.userRoleGridDataResult = process(this.userRoles, this.userRoleState);
  }

  onRoleSearchHandleFilter(value) {
    this.roleSearchs = this.roles.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  async onRoleSearchValueChange(e: any) {
    this.roleSearchs = this.roles.slice();
    this.onReload();
  }

  async onServiceSearchValueChange(e: any) {
    this.onReload();
  }

  async initDisplay() {

    const resultrole = await this.appService.doGET('api/role', null);
    if (resultrole) {
      this.roles = resultrole.Data;
      this.roleFilters = this.roles.slice();
      this.roleSearchs = this.roles.slice();
    }

    if (this.roles.length > 0) {
      this.searchOption.RoleID = this.roles[0].ID;
      this.onReload();
    }
  }

  onReload() {
    this.getUserRoles();
  }

  async onSaveUserRole(id , parentID, activeFlg) { 
    var dataRequest = {
      RoleID: this.searchOption.RoleID,
      PageID: parentID ? parentID : id,
      Value: parentID ? id : "",
      ActiveFlg: activeFlg
    };

    const result = await this.appService.doPOST('api/RolePage', dataRequest);
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onReload();
    } else {
      this.appSwal.showWarning(result.Msg, false);
    }
  }

  onBack() {
    this.location.back();
  }


  searchTitle = {
    searchString: ''
  }

  onRemoveSearchText() {
    this.searchTitle.searchString = '';
  }

  getUrlDownload(item) {
    let url = this.appService.apiRoot.replace(/\"/g, "") + item;
    url = url.replace(/\"/g, "");
    return url;
  }

  onSearchTextChange1(e: any){
    this.unGranteduserRoleFilter = this.userRoleFilter.filter(x => {
      if(x.ActiveFlg == false ) {
        return true
      }
    });

    this.userRoles.forEach(item => {
      if(this.unGranteduserRoleFilter.some(x => x.ParentID == item.ID) ){
        if (item.ActiveFlg == true){
          this.unGranteduserRoleFilter.unshift(item);
        }        
      }    
    })

    if (this.searchOption.SearchText1.length > 0){
      this.unGranteduserRoleFilter = this.unGranteduserRoleFilter.filter(x => {
        if((x.ParentName && x.ParentName.includes(this.searchOption.SearchText1)) 
          || (x.Name && x.Name.includes(this.searchOption.SearchText1))){
            return true;
          }
      })
    }
    this.unGranteduserRole = this.unGranteduserRoleFilter.slice();
  }

  onSearchTextChange2(e: any){
    this.GranteduserRoleFilter = this.userRoleFilter.filter(x => {
      if(x.ActiveFlg == true ){
        return true
      }
    });
    if (this.searchOption.SearchText2.length > 0){
      this.GranteduserRoleFilter = this.GranteduserRoleFilter.filter(x => {
        if((x.ParentName && x.ParentName.includes(this.searchOption.SearchText2)) 
          || (x.Name && x.Name.includes(this.searchOption.SearchText2))){
            return true
          }
      })
    }    
    this.GranteduserRole = this.GranteduserRoleFilter.slice();
  }

  onSearchKeyPress1(e: any) {
    if (e.keyCode === 13 && this.searchOption.SearchText1) {
      this.onSearchTextChange1(e);
    }
  }

  onSearchKeyPress2(e: any) {
    if (e.keyCode === 13 && this.searchOption.SearchText2) {
      this.onSearchTextChange2(e);
    }
  }

}
