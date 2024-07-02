import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
  ElementRef,
} from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { AppService } from "../../services/app.service";
import {
  SelectableSettings,
  PageChangeEvent,
  GridDataResult,
  DataStateChangeEvent,
} from "@progress/kendo-angular-grid";
import {
  State,
  process,
  SortDescriptor,
  orderBy,
} from "@progress/kendo-data-query";
import { AppSwal } from "src/app/services/app.swal";
import { IntlService } from "@progress/kendo-angular-intl";
import { Notification } from "../../services/app.notification";
import { ExcelExportData } from "@progress/kendo-angular-excel-export";
import { Observable } from "rxjs/Observable";
import { AppGuid } from "src/app/services/app.guid";
import { NullInjector } from "@angular/core/src/di/injector";
import {
  FileRestrictions,
  SelectEvent,
  ClearEvent,
  RemoveEvent,
  FileInfo,
} from "@progress/kendo-angular-upload";
import * as XLSX from "xlsx";
import { AppFile } from "src/app/services/app.file";
import { nullSafeIsEquivalent } from "@angular/compiler/src/output/output_ast";
import { AuthenticationService } from "src/app/services/authentication.service";
import { AppControls } from "src/app/services/app.controls";
import { AppUtils } from "src/app/services/app.utils";
import { AppComponent } from "../../app.component";
import { DomSanitizer } from "@angular/platform-browser";
import { AppConsts } from "src/app/services/app.consts";
import { delay, map } from "rxjs/operators";
import { isString, values } from "underscore";

@Component({
  selector: "app-role",
  templateUrl: "./role.component.html",
  //styleUrls: ['./role.component.css'],
})
export class RoleComponent implements OnInit, OnDestroy {
  user: any;
  loading = false;
  dataRoles = [];
  dataRolesParent = [];
  dataRolesParentFilter = [];
  dataRoleSelectableSettings: SelectableSettings;
  dataRoleSort = {
    allowUnsort: true,
    mode: "multiple",
  };
  public dataRoleFocus = {
    Name: true,
  };
  dataRoleSortByField: SortDescriptor[] = [
    // {
    //   field: 'ParentName',
    //   dir: 'asc'
    // }, {
    //   field: 'OrdinalNumber',
    //   dir: 'asc'
    // }
  ];

  public WKS_NUM_PAGING_SKIP = 0;
  public WKS_NUM_PAGING_TAKE = 10;
  public WKS_NUM_PAGING_BTN = 5;

  pageUrl: Array<{ Name: string; ID: string }>;
  pageUrlsFilter: Array<{ Name: string; ID: string }>;

  dataRoleSkip = this.WKS_NUM_PAGING_SKIP;
  dataRolePageSize = this.WKS_NUM_PAGING_TAKE;
  dataRoleSelection: number[] = [];
  dataRoleItem: any;
  myInterval: any;

  public buttonCount = this.WKS_NUM_PAGING_BTN;
  public info = true;
  public type: "numeric" | "input" = "numeric";
  public pageSizes = true;
  public previousNext = true;

  public dataRoleState: State = {
    skip: this.dataRoleSkip,
    take: this.dataRoleSkip + this.dataRolePageSize,
    filter: {
      logic: "and",
      filters: [],
    },
  };
  dataRoleGridDataResult: GridDataResult;

  roles: Array<{ Name: string; ID: string }>;
  rolesFilter: Array<{ Name: string; ID: string }>;
  pages: Array<{ Name: string; ID: string }>;
  PagesFilter: Array<{ Name: string; ID: string }>;

  defaultUrls = AppConsts.page;

  public uploadSaveUrl = "saveUrl";
  public uploadRemoveUrl = "removeUrl";
  public addNew = false;
  public editItem = false;
  public saveItem = false;
  public enabledID = false;
  isEnabledSaveAll = false;
  control: any;
  controlDefault = true;
  allowMulti = true;
  pageName: any;

  searchOption = {
    SearctText: "",
  };
  roleDialogOpened = false;

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
    private domSanitizer: DomSanitizer
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
    this.rolesFilter = this.roles.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  pageUrlsHandleFilter(value) {
    this.pageUrlsFilter = this.pageUrl.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  RolesParentHandleFilter(value) {
    this.dataRolesParentFilter = this.dataRolesParent.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  ngOnDestroy(): void {
    if (this.myInterval) {
      this.myInterval.unsubscribe();
    }
  }

  ngOnInit() {
    this.pageUrl = Object.values(this.defaultUrls).map((item) => {
      var container: { ID: string; Name: string };
      container = {
        ID: item,
        Name: item,
      };

      return container;
    });
    this.pageUrlsFilter = this.pageUrl.slice();
  }

  async getControl() {
    this.control = await this.appControls.getControls(
      this.user.RoleID,
      AppConsts.page.role
    );
    this.controlDefault = false;
  }

  setSelectableSettings(): void {
    this.allowMulti = !this.allowMulti;
    this.dataRoleSelectableSettings = {
      checkboxOnly: false,
      mode: this.allowMulti ? "multiple" : "single",
    };
  }

  onSearchKeyPress(e: any) {
    if (e.keyCode === 13 && this.searchOption.SearctText) {
      this.onSearch();
    }
  }

  async getRoles() {
    this.loading = true;
    const dataRequest = {
      searchText: this.searchOption.SearctText,
    };

    const result = await this.appService.doGET("api/Role/Search", dataRequest);
    if (result) {
      this.dataRoles = result.Data;
      this.dataRolesParent = result.Data;
      for (let i = 0; i < this.dataRoles.length; i++) {
        this.dataRoles[i].ParentName = this.appUtils.getNameByList(
          this.dataRoles,
          this.dataRoles[i].ParentID
        );
      }

      this.dataRolesParentFilter = this.dataRolesParent.slice();
      this.dataRoleState.skip = 0;
      this.dataRoleSkip = 0;
      this.bindRoles();
    }
    this.loading = false;
    this.checkSelectionID();
  }

  checkSelectionID() {
    // tslint:disable-next-line:prefer-for-of
    for (let i = this.dataRoleSelection.length - 1; i >= 0; i--) {
      const selectedItem = this.dataRoles.find((item) => {
        return item.ID === this.dataRoleSelection[i];
      });
      if (!selectedItem) {
        this.dataRoleSelection.splice(i, 1);
      }
    }
  }
  dataRoleItemtemp: any;
  setDefault() {
    this.dataRoleItem = {
      IsAdd: true,
      ID: "",
      PageID: null,
      ParentID: null,
      Name: "",
      DefaultFlg: false,
    };
    this.dataRoleItemtemp = {
      IsAdd: true,
      ID: "",
      Name: "",
      PageID: null,
      ParentID: null,
      DefaultFlg: false,
    };
  }
  bindtemp(item) {
    this.dataRoleItemtemp = Object.assign({}, item);
    this.dataRoleItemtemp.PageID =
      this.pageUrlsFilter != undefined && this.pageUrlsFilter != null
        ? this.pageUrlsFilter.find((x) => x == item.PageID) != undefined
          ? this.pageUrlsFilter.find((x) => x == item.PageID)
          : item.PageID != null
          ? { ID: item.PageID, Name: item.PageID }
          : null
        : null;
  }

  onRolePageChange(event: PageChangeEvent) {
    this.dataRoleSkip = event.skip;
    this.bindRoles();
  }

  onRoleSelectedKeysChange() {
    if (this.dataRoleSelection.length === 0) {
      this.appSwal.showWarning(
        this.translate.instant("NoRecordSelected"),
        false
      );
      return;
    }

    if (this.dataRoleSelection.length > 1) {
      if (this.allowMulti) {
        return;
      }
      this.appSwal.showWarning(this.translate.instant("SelectSingle"), false);
    } else {
      const selectedID = this.dataRoleSelection[0];
      const selectedItem = this.dataRoles.find((item) => {
        return item.ID === selectedID;
      });
      selectedItem.IsAdd = false;
      this.dataRoleItem = selectedItem;
      this.bindtemp(this.dataRoleItem);
    }
  }

  bindRoles() {
    this.dataRoleGridDataResult = {
      data: orderBy(this.dataRoles, this.dataRoleSortByField),
      total: this.dataRoles.length,
    };

    this.dataRoleGridDataResult = process(this.dataRoles, this.dataRoleState);
  }

  onRoleSortChange(sort: SortDescriptor[]): void {
    this.dataRoleSortByField = sort;
    this.bindRoles();
  }

  public onRoleDataStateChange(state: DataStateChangeEvent): void {
    this.dataRoleSelection = [];
    this.dataRoleState = state;
    this.dataRoleGridDataResult = process(this.dataRoles, this.dataRoleState);
  }

  getColumnIndex(name) {
    const columns = ["ID", "Name", "DefaultFlg", "PageID", "ParentID"];

    return columns.indexOf(name);
  }

  async selectEventHandler(e: SelectEvent) {
    this.appComponent.loading = true;
    const fileData = (await this.file.readXLSX(
      e.files[0].rawFile
    )) as Array<any>;
    this.dataRoles = [];
    // tslint:disable-next-line:prefer-for-of
    for (let i = 1; i < fileData.length; i++) {
      this.dataRoles.push({
        IsAdd: false,
        ID: fileData[i][this.getColumnIndex("ID")],
        Name: fileData[i][this.getColumnIndex("Name")],
        DefaultFlg: fileData[i][this.getColumnIndex("DefaultFlg")],
      });
    }
    this.bindRoles();
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
    this.searchOption.SearctText = "";
    this.onReload();
  }

  async initDisplay() {
    const resultRole = await this.appService.doGET("api/Role", null);
    if (resultRole && resultRole.Status === 1) {
      this.roles = resultRole.Data;
      this.rolesFilter = this.roles.slice();
    }
    const resultPage = await this.appService.doGET("api/Role/GetPage", null);
    if (resultPage && resultPage.Status === 1) {
      this.pages = resultPage.Data;
      this.PagesFilter = this.pages.slice();
    }
  }

  onSearch() {
    this.getRoles();
    this.isEnabledSaveAll = false;
  }

  onReload() {
    this.searchOption.SearctText = "";
    this.getRoles();
    this.isEnabledSaveAll = false;
  }

  onClearRole() {
    this.setDefault();
  }

  onAllowSelectMulti() {
    this.setSelectableSettings();
  }

  onAddNewRole() {
    this.addNew = true;
    this.editItem = false;
    this.saveItem = true;
    this.enabledID = true;
    this.roleDialogOpened = true;
    this.setDefault();
  }

  onSaveRole() {
    if (this.dataRoleItem.IsAdd) {
      this.addRole();
    } else {
      this.updateRole();
    }
  }

  async onSaveRoles() {
    this.appComponent.loading = true;
    const dataRequests = [];
    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < this.dataRoles.length; i++) {
      dataRequests.push(this.createDataRequest(this.dataRoles[i]));
    }
    const result = await this.appService.doPOST("api/Role/Saves", dataRequests);
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
    const temp = data ? data : this.dataRoleItemtemp;

    var outID = temp.PageID;
    if (temp.PageID != undefined && temp.PageID != null) {
      if (temp.PageID.ID != undefined && temp.PageID.ID != null) {
        outID = temp.PageID.ID;
      }
    }
    return {
      ID: temp.ID,
      Name: temp.Name,
      PageID: outID,
      ParentID: temp.ParentID,
      DefaultFlg: temp.DefaultFlg,
    };
  }

  onEditRole(selectedID) {
    const selectedItem = this.dataRoles.find((item) => {
      return item.ID === selectedID;
    });
    if (selectedItem) {
      selectedItem.IsAdd = false;
      this.dataRoleItem = selectedItem;
      this.bindtemp(this.dataRoleItem);

      this.saveItem = true;
      this.addNew = false;
      this.editItem = true;
      this.enabledID = false;
      this.roleDialogOpened = true;
    }
  }

  async addRole() {
    this.appComponent.loading = true;
    const dataRequest = this.createDataRequest(null);
    const result = await this.appService.doPOST("api/Role", dataRequest);
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onReload();
      this.onAddNewRole();
      this.roleDialogOpened = false;
    } else {
      this.appSwal.showWarning(result.Msg, false);
    }
    this.appComponent.loading = false;
  }

  valueNormalizerAdmin = (text: Observable<string>) =>
    text.pipe(
      map((text: string) => {
        if (!text.replace(/\s/g, "").length) {
          return;
        }
        const matchingValue: any = this.pageUrl.find((item: any) => {
          return item.toLowerCase() === text.toLowerCase();
        });
        if (matchingValue) {
          return matchingValue; //return the already selected matching value and the component will remove it
        }
        return { ID: text, Name: text };
      })
    );

  async updateRole() {
    this.appComponent.loading = true;
    const id = this.dataRoleItem.ID;
    const dataRequest = this.createDataRequest(null);

    const result = await this.appService.doPUT("api/Role", dataRequest, { id });
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onReload();
      this.roleDialogOpened = false;
    } else {
      this.appSwal.showWarning(result.Msg, false);
    }
    this.appComponent.loading = false;
  }

  async onDeleteRole(selectedID) {
    const dataRequest = {
      IDList: selectedID,
    };

    const option = await this.appSwal.showWarning(
      this.translate.instant("AreYouSure"),
      true
    );
    if (option) {
      const result = await this.appService.doPOST(
        "api/Role/Deletes",
        dataRequest
      );
      if (result && result.Status === 1) {
        this.notification.showSuccess(result.Msg);
        this.onReload();
        this.setDefault();
        this.saveItem = false;
        this.editItem = false;
        this.addNew = false;
        this.enabledID = false;
        this.dataRoleSelection = [];
        this.allowMulti = false;
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
  }

  onCloseRoleDialog(status) {
    this.roleDialogOpened = false;
  }
  filterInput(event) {
    var format = /[`!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/;
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }

    return ["Backspace", "Delete", "ArrowLeft", "ArrowRight"].includes(
      event.code
    )
      ? true
      : (String(event.key) || event.code == "Space") &&
          format.test(event.key) == false;
  }
}
