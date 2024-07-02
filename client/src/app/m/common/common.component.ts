import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
  ElementRef,
  EventEmitter,
  Output,
  Input,
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
import "rxjs/add/observable/interval";
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

@Component({
  selector: "app-common",
  templateUrl: "./common.component.html",
  //styleUrls: ['./common.component.css']
})
export class CommonComponent implements OnInit, OnDestroy {
  @Output() newCommon = new EventEmitter();
  @Input() isAddNewOutCommon = false;
  @Input() typeCommonInput = 0;
  user: any;
  popupClass = "popup-width";
  btnFunctionData: Array<any> = [];
  btnMbFunctionData: Array<any> = [];
  loading = false;
  dataGrids = [];
  dataGridSelectableSettings: SelectableSettings;
  dataGridSort = {
    allowUnsort: true,
    mode: "multiple",
  };
  public dataGridFocus = {
    Name: true,
  };
  dataGridSortByField: SortDescriptor[] = [];

  dataGridSkip = this.appConsts.pageSkip;
  dataGridPageSize = this.appConsts.pageSize;
  dataGridSelection: number[] = [];
  dataGridItem: any;
  dataLangs = [];

  selectedLangID = this.appConsts.defaultLangID;

  public type: "numeric" | "input" = "numeric";
  public pageSizes = true;
  public previousNext = true;

  public dataGridState: State = {
    skip: this.dataGridSkip,
    take: this.dataGridSkip + this.dataGridPageSize,
    filter: {
      logic: "and",
      filters: [],
    },
  };
  dataGridGridDataResult: GridDataResult;

  dataItems: Array<{ Name: string; ID: string }>;
  dataItemsFilter: Array<{ Name: string; ID: string }>;

  public disabled = false;
  public enabledID = false;
  control: any;
  controlDefault = true;
  allowMulti = false;
  infoOpened = false;

  searchOption = {
    SearchText: "",
    Type: null,
  };
  dataErr: any;

  pageName = "";

  cmType: Array<{ Name: string; ID: string }>;
  cmTypeFilter: Array<{ Name: string; ID: string }>;

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
    private domSanitizer: DomSanitizer,
    public appConsts: AppConsts
  ) {
    this.authenticationService.getUser();
    this.user = this.authenticationService.user;
  }

  ngOnDestroy(): void {}

  ngOnInit() {
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
  async getControl() {
    this.control = await this.appControls.getControls(
      this.user.RoleID,
      AppConsts.page.common
    );
    this.controlDefault = false;
  }

  async getTypeCM() {
    var result = await this.appService.doGET("api/Enum/GetECommon", null);
    this.cmType = result.Data;
    this.cmTypeFilter = this.cmType;
  }

  cmHandleFilter(value) {
    this.cmTypeFilter = this.cmType.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  getTypeName(typeID) {
    var typeItem = this.cmType.find((item) => {
      return item.ID == typeID;
    });

    return typeItem.Name;
  }

  setSelectableSettings(): void {
    //this.allowMulti = !this.allowMulti;
    this.dataGridSelectableSettings = {
      checkboxOnly: false,
      mode: this.allowMulti ? "multiple" : "single",
    };
  }

  onSearchKeyPress(e: any) {
    if (e.keyCode === 13 && this.searchOption.SearchText) {
      this.onSearch();
    }
  }

  onSearchTextKeyPress(e: any) {
    if (e.keyCode === 13) {
      this.onSearch();
    }
  }

  onSearchTextRemove(e: any) {
    this.searchOption.SearchText = "";
    this.onSearch();
  }

  dataItemsHandleFilter(value) {
    this.dataItemsFilter = this.dataItems.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  async getdataItems() {
    this.loading = true;
    const dataRequest = {
      searchText: this.searchOption.SearchText,
      type: this.searchOption.Type,
      langID: localStorage.getItem("currentLanguage"),
    };
    const result = await this.appService.doPOST(
      "api/CommonMenu/Search",
      dataRequest
    );
    if (result) {
      this.dataGrids = result.Data;
      this.dataGridState.skip = 0;
      this.dataGridSkip = 0;
      this.binddataItems();
    }
    this.loading = false;
  }

  setDefault() {
    this.dataGridItem = {
      IsAdd: true,
      ID: 0,
      Name: "",
      Type: null,
      CreateAt: "",
      DelFlg: false,
      OrderIndex: 0,
    };
    var errTemp = Object.assign({}, this.dataGridItem);
    errTemp.Type = null;
    this.dataErr = [errTemp];

    this.dataGridSelection = [];
    this.disabled = false;
    this.enabledID = true;
  }

  onGridPageChange(event: PageChangeEvent) {
    this.dataGridSkip = event.skip;
    this.binddataItems();
  }

  ondataItemselectedKeysChange() {}

  binddataItems() {
    this.dataGridGridDataResult = {
      data: orderBy(this.dataGrids, this.dataGridSortByField),
      total: this.dataGrids.length,
    };

    this.dataGridGridDataResult = process(this.dataGrids, this.dataGridState);
  }

  ondataItemsortChange(sort: SortDescriptor[]): void {
    this.dataGridSortByField = sort;
    this.binddataItems();
  }

  public onGridDataStateChange(state: DataStateChangeEvent): void {
    this.dataGridSelection = [];
    this.dataGridState = state;
    this.dataGridGridDataResult = process(this.dataGrids, this.dataGridState);
  }

  onSearchTextChange(e: any) {
    if (!this.searchOption.SearchText) {
      this.onReload();
    }
  }

  onRemoveSearchText() {
    this.searchOption.SearchText = "";
    this.onReload();
  }
  async onChangeYUnit(e: any) {
    this.onReload();
  }
  async initDisplay() {}

  onSearch() {
    this.getdataItems();
  }

  async onReload() {
    //this.searchOption.SearchText = '';
    await this.getTypeCM();
    this.getdataItems();
  }

  onClearGrid() {
    this.setDefault();
  }

  onAllowSelectMulti() {
    this.setSelectableSettings();
  }

  onAddNewGrid() {
    this.setDefault();
    this.infoOpened = true;
  }

  onSaveGrid() {
    if (this.dataGridItem.IsAdd) {
      this.addGrid();
    } else {
      this.updateGrid();
    }
  }

  createDataRequest(data) {
    const temp = data ? data : this.dataGridItem;
    return {
      ID: temp.ID,
      Name: temp.Name,
      Type: temp.Type,
      CreateAt: temp.CreateAt,
      DelFlg: temp.DelFlg,
      OrderIndex: temp.OrderIndex == null ? 0 : temp.OrderIndex,
      LangID: this.selectedLangID,
    };
  }

  onCloseGrid(status: any) {
    this.disabled = true;
    this.enabledID = false;
    this.infoOpened = true;
    this.setDefault();
  }

  async onEditGrid(dataItem: any) {
    await this.getDataItemByID(dataItem.ID);
  }

  async getDataItemByID(id: any) {
    const dataRequest = {
      iD: id,
      langID: this.selectedLangID,
    };

    const result = await this.appService.doGET("api/CommonMenu", dataRequest);
    if (result && result.Status === 1) {
      this.dataGridItem = result.Data;
      this.dataGridItem.UnitContactID = result.Data.ID;
      this.disabled = false;
      this.enabledID = false;
      this.infoOpened = true;
    } else {
      this.appSwal.showWarning(result.Msg, false);
    }
  }

  async addGrid() {
    this.appComponent.loading = true;
    const dataRequest = this.createDataRequest(null);
    const result = await this.appService.doPOST("api/CommonMenu", dataRequest);
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);

      this.onReload();
      this.onAddNewGrid();
      this.onCloseInfo();
    } else {
      if (!result.Msg) {
        this.dataErr = result.Data;
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
    this.appComponent.loading = false;
  }

  async updateGrid() {
    this.appComponent.loading = true;
    const iD = this.dataGridItem.ID;
    const dataRequest = this.createDataRequest(null);

    const result = await this.appService.doPUT("api/CommonMenu", dataRequest, {
      iD,
    });
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onCloseInfo();
      this.onReload();
    } else {
      if (!result.Msg) {
        this.dataErr = result.Data;
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
    this.appComponent.loading = false;
  }

  async onDeleteGrid(dataItem: any) {
    this.appComponent.loading = true;
    const dataRequest = {
      iD: dataItem.ID,
      langID: this.selectedLangID,
    };

    const option = await this.appSwal.showWarning(
      this.translate.instant("AreYouSure"),
      true
    );
    if (option) {
      const result = await this.appService.doDELETE(
        "api/CommonMenu",
        dataRequest
      );
      if (result && result.Status === 1) {
        this.notification.showSuccess(result.Msg);
        this.onCloseInfo();
        this.onReload();
        this.dataGridSelection = [];
        this.allowMulti = false;
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
    this.appComponent.loading = false;
  }

  public allData(): ExcelExportData {
    const result: ExcelExportData = {
      data: process(this.dataGrids, {}).data,
    };

    return result;
  }

  onCloseInfo() {
    // this.infoOpened = false;

    this.infoOpened = false;
    this.setDefault();
    this.getTypeCM();
  }

  onChangeFunction(e, dataItem) {
    if (e.id == "Edit") {
      this.onEditGrid(dataItem);
    } else if (e.id == "Delete") {
      this.onDeleteGrid(dataItem);
    }
  }
  onFunctionIconClick(dataItem) {
    this.getbtnFunctionData(dataItem);
  }

  getbtnFunctionData(dataItem) {
    this.btnFunctionData = [];
    this.btnMbFunctionData = [];
    if (this.controlDefault || this.control.Edit) {
      var func1 = {
        text: this.translate.instant("Edit"),
        id: "Edit",
        icon: "pencil",
      };
      this.btnFunctionData.push(func1);
      this.btnMbFunctionData.push(func1);
    }

    if (this.controlDefault || this.control.Delete) {
      var func3 = {
        text: this.translate.instant("Delete"),
        id: "Delete",
        icon: "delete",
      };
      this.btnFunctionData.push(func3);
      this.btnMbFunctionData.push(func3);
    }
  }

}
