import {  Component,  OnInit,  OnDestroy,  ViewChild,  ElementRef,} from "@angular/core";
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
import { AuthenticationService } from "src/app/services/authentication.service";
import { AppControls } from "src/app/services/app.controls";
import { AppConsts } from "src/app/services/app.consts";

@Component({
  selector: "app-config",
  templateUrl: "./config.component.html",
  //styleUrls: ['./config.component.css']
})
export class ConfigComponent implements OnInit, OnDestroy {
  user: any;
  loading = false;
  configs = [];
  configSelectableSettings: SelectableSettings;
  configSort = {
    allowUnsort: true,
    mode: "multiple",
  };
  public configFocus = {
    Value: true,
  };
  configSortByField: SortDescriptor[] = [
    {
      field: "Value",
      dir: "asc",
    },
  ];

  configGridDataResult: GridDataResult;

  configSkip = 0;
  configPageSize = 8;
  configSelection: number[] = [];
  configItem: any;
  myInterval: any;
  configNames: Array<{ Name: string; ID: string }>;
  configNamesFilter: Array<{ Name: string; ID: string }>;

  public uploadSaveUrl = "saveUrl";
  public uploadRemoveUrl = "removeUrl";
  public configOpened = false;
  isEnabledSaveAll = false;
  data: any;
  control: any;
  phoneEreId: any;
  controlDefault = true;
  pageName: any;
  editDiaolog = false;

  public configState: State = {
    skip: this.configSkip,
    take: this.configSkip + this.configPageSize,
    filter: {
      logic: "and",
      filters: [],
    },
  };

  constructor(
    private translate: TranslateService,
    private appService: AppService,
    private appSwal: AppSwal,
    public intl: IntlService,
    private notification: Notification,
    private file: AppFile,
    private authenticationService: AuthenticationService,
    public appControls: AppControls
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

  handleFilter(value) {
    this.configNamesFilter = this.configNames.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  ngOnDestroy(): void {
    if (this.myInterval) {
      this.myInterval.unsubscribe();
    }
  }

  ngOnInit() { }

  async getControl() {
    this.control = await this.appControls.getControls(
      this.user.RoleID,
      AppConsts.page.config
    );
    this.controlDefault = false;
  }

  setSelectableSettings(): void {
    this.configSelectableSettings = {
      checkboxOnly: false,
      mode: "multiple",
    };
  }

  async getConfigs() {
    this.loading = true;
    const result = await this.appService.doGET("api/Config", null);
    if (result) {
      this.configs = result.Data;
      this.bindConfigs();
    }
    this.loading = false;
    this.checkSelectionID();
  }

  checkSelectionID() {
    // tslint:disable-next-line:prefer-for-of
    for (let i = this.configSelection.length - 1; i >= 0; i--) {
      const selectedItem = this.configs.find((item) => {
        return item.ID === this.configSelection[i];
      });
      if (!selectedItem) {
        this.configSelection.splice(i, 1);
      }
    }
  }

  setDefault() {
    this.configItem = {
      IsAdd: true,
      ID: "",
      Value: "",
      Description: "",
      SercurityValue: "",
    };
    this.configFocus.Value = true;
  }

  onConfigPageChange(event: PageChangeEvent) {
    this.configSkip = event.skip;
    this.bindConfigs();
  }

  onConfigSelectedKeysChange() { }

  bindConfigs() {
    this.configGridDataResult = {
      data: orderBy(this.configs, this.configSortByField),
      total: this.configs.length,
    };
    this.configGridDataResult = process(this.configs, this.configState);
  }

  onConfigSortChange(sort: SortDescriptor[]): void {
    this.configSortByField = sort;
    this.bindConfigs();
  }

  public onConfigDataStateChange(state: DataStateChangeEvent): void {
    this.configState = state;
    this.configGridDataResult = process(this.configs, this.configState);
  }



  async initDisplay() {
    const resultConfig = await this.appService.doGET(
      "api/Enum/GetEConfig",
      null
    );
    if (resultConfig) {
      this.configNames = resultConfig.Data;
      this.configNamesFilter = this.configNames.slice();
    }
  }
  onReload() {
    this.getConfigs();
  }

  onClearConfig() {
    this.setDefault();
  }

  onAddNewConfig() {
    this.editDiaolog = true;
    this.configOpened = true;
    this.setDefault();
  }

  onSaveConfig() {
    if (this.configItem.IsAdd) {
      this.addConfig();
    } else {
      this.updateConfig();
    }
  }

  async onSaveConfigs() {
    const dataRequests = [];
    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < this.configs.length; i++) {
      dataRequests.push(this.createDataRequest(this.configs[i]));
    }
    const result = await this.appService.doPOST(
      "api/Config/SaveAll",
      dataRequests
    );
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onReload();
      this.isEnabledSaveAll = false;
    } else {
      this.appSwal.showWarning(result.Msg, false);
    }
  }

  createDataRequest(data) {
    const temp = data ? data : this.configItem;
    return {
      ID: temp.ID,
      Value: temp.Value,
      Description: temp.Description,
      SercurityValue: temp.SercurityValue,
    };
  }

  onCloseConfig() {
    document.body.classList.remove("disable-scroll");
    this.configOpened = false;
    this.onReload();
  }

  onEditConfig(selectedID) {
    this.editDiaolog = false;
    const selectedItem = this.configs.find((item) => {
      return item.ID === selectedID;
    });

    if (selectedItem) {
      selectedItem.IsAdd = false;
      this.configItem = selectedItem;
      this.configOpened = true;
      this.editDiaolog = true;
    }
  }

  async addConfig() {
    const dataRequest = this.createDataRequest(null);
    const result = await this.appService.doPOST("api/Config", dataRequest);
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onReload();
      this.onAddNewConfig();
      this.configOpened = false;
    } else {
      this.appSwal.showWarning(result.Msg, false);
    }
  }

  async updateConfig() {
    this.phoneEreId = this.validatePhone();
    console.log(this.phoneEreId);

    if (this.phoneEreId === "" || this.phoneEreId == null) {
      const id = this.configItem.ID;
      const dataRequest = this.createDataRequest(null);

      const result = await this.appService.doPUT("api/Config", dataRequest, {
        id,
      });
      if (result && result.Status === 1) {
        this.notification.showSuccess(result.Msg);
        this.onReload();
        this.configOpened = false;
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
  }
  async onDeleteConfig(selectedID) {
    const dataRequest = {
      IDList: selectedID,
    };

    const option = await this.appSwal.showWarning(
      this.translate.instant("AreYouSure"),
      true
    );
    if (option) {
      const result = await this.appService.doPOST(
        "api/Config/Deletes",
        dataRequest
      );
      if (result && result.Status === 1) {
        this.notification.showSuccess(result.Msg);
        this.onReload();
        this.setDefault();
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
  }
  validatePhone() {
    if (!this.configItem.ID) {
      if (this.configItem.ID == "" || this.configItem.ID == null) {
        return "Vui lòng nhập ID";
      } else return "";
    }
  }
}
