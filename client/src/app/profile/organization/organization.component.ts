import {  Component,  OnInit,  OnDestroy,  ViewChild,
  ElementRef,
  Input,
  Output,
  EventEmitter,
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
import { AppConsts } from "src/app/services/app.consts";
import { AppComponent } from "src/app/app.component";
import { Offset } from "@progress/kendo-angular-popup";
import { fromEvent } from "rxjs";
import { AnimationStyleNormalizer } from "@angular/animations/browser/src/dsl/style_normalization/animation_style_normalizer";
import { isBuffer } from "util";
import { AppUtils } from "src/app/services/app.utils";
import { values } from "underscore";

@Component({
  selector: "app-organization",
  templateUrl: "./organization.component.html",
  //styleUrls: ['./organization.component.css']
})
export class OrganizationComponent implements OnInit {
  mouseDown = false;
  startX: any;
  scrollLeft: any;
  public disabled = false;
  public enabledID = false;
  user: any;
  loading = false;
  control: any;
  controlDefault = true;
  dataUnits = [];
  dataUnitsTreeList = [];
  show = true;
  searchOption = {
    SearchText: "",
  };
  data = {
    type: "OrgChart",
    title: "",
    units: [],
    columnNames: ["Name", "Manager", "Tooltip"],
    options: {
      allowHtml: true,
      nodeClass: "org-chart-style",
      orientation: "vertical",
    },
    options1: {
      allowHtml: true,
      nodeClass: "org-chart-style1",
    },
    // width: 100%,
    height: 900,
  };

  unitDetail = {
    Name: "",
    Description: "",
  };
  dataGridItem: any;
  dataLangs = [];
  currentLang: any;
  phoneEre: any;
  selectedLangID = this.appConsts.defaultLangID;
  listMemberDialog = false;
  addUnitDialog = false;
  unitsFilter: Array<{ Name: string; ID: string; ParentID: string }>;
  unitsChildOfUser: Array<{ Name: string; ID: string; ParentID: string }>;
  units: any;

  typeUnit: Array<{ Name: string; ID: string }>;
  typeUnitFilter: Array<{ Name: string; ID: string }>;

  address: Array<{ Name: string; ID: string }>;
  addressFilter: Array<{ Name: string; ID: string }>;

  languageName = "";
  dataUnitItemtemp: any;
  dataUnitItem: any;
  dataUnitItemEnLanguage: any;
  organizationComponent = false;
  isParentFlag = false;
  isIDFlag = false;
  unitTemp = [];
  LangUnit: any;
  isUpdateData = false;
  unitFilterTemp = [];
  pageName: any;
  dataErr: any;
  importExcelSuccess;
  tempData = [
    {
      ID: "",
      Name: "",
      ParentID: "",
      AddressID: "",
      Type: "",
    },
  ];
  dialogType = 1;
  LongLatPattern = /^[0-9]+\.[0-9]+$/;

  // For Kendo Grid
  DataGrids = [];
  DataGridSelectableSettings: SelectableSettings;

  DataGridGridDataResult: GridDataResult;
  //used for kendo grid
  public WORKING_NUM_PAGING_SKIP = 0;
  public WORKING_NUM_PAGING_TAKE = 10;
  public WORKING_NUM_PAGING_BTN = 5;
  public buttonCount = this.WORKING_NUM_PAGING_BTN;
  // paging in grid
  dataDataGridSkip = this.WORKING_NUM_PAGING_SKIP;
  dataDataGridPageSize = this.WORKING_NUM_PAGING_TAKE;
  public DataGridChange: State = {};
  public DataGridState: State = {
    skip: this.dataDataGridSkip,
    take: this.dataDataGridSkip + this.dataDataGridPageSize,
    filter: {
      logic: "and",
      filters: [],
    },
  };
  DataGridSortByField: SortDescriptor[] = [
    // {
    // field: 'Name',
    // dir: 'asc'
    // }
  ];

  DataGridSelection: number[] = [];

  myRestrictionsImage: FileRestrictions = {
    allowedExtensions: [".jpg", ".jpeg", ".png"],
  };
  filesUploadAvatar: "";
  allowInsertFile = true;
  public uploadSaveAvatar = "";
  filesUploadName = "";
  filesUpload: Array<FileInfo>;
  public fileSaveUrl: any;
  isOpenDetailDialog = false;
  organizationID = "";
  btnFunctionData: Array<any> = [];
  btnMbFunctionData: Array<any> = [];
  defaultImg = null;
  // End for Kendo Grid
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
    private appUtils: AppUtils
  ) {
    this.authenticationService.getUser();
    this.user = this.authenticationService.user;
    this.getControl();
    this.setDefault();
    this.onReload();
    this.getPageName();
  }

  async getPageName() {
    this.pageName = await this.appControls.getPageName();
  }

  option: Array<any> = [];
  async getControl() {
    this.control = await this.appControls.getControls(
      this.user.RoleID,
      AppConsts.page.pro5Organization
    );
    this.controlDefault = false;
    this.option = [];
    if (this.controlDefault || this.control.Edit) {
      this.option.push({
        id: 0,
        text: "Chỉnh sửa",
        click: (dataItem) => {
          this.onEditNewUnit(this.dataUnitItemtemp);
          // this.onEditLangUnit(this.dataUnitItem.ID)
        },
      });
    }
    if (this.controlDefault || this.control.Delete) {
      {
        this.option.push({
          id: 1,
          text: "Xóa",
          click: (dataItem) => {
            this.onDeleteUnit(this.dataUnitItem);
          },
        });
      }
    }
  }

  setDefault() {
    this.defaultImg = "assets/images/image.png";
    this.dataUnitItem = {
      IsAdd: true,
      ID: "",
      Name: "",
      ParentID: "",
      ParentName: "",
      CreateAt: "",
      Type: null,
      TypeName: "",
      TypeNameParent: "",
      DelFlg: false,
      Level: null,
      Email: "",
      Phone: "",
      ImageUrl: this.defaultImg,
      // Longitude: null,
      // Latitude: null,
      // Address: '',
      //AddressID : null
    };
    this.dataUnitItemtemp = {
      IsAdd: true,
      ID: "",
      Name: "",
      ParentID: "",
      ParentName: "",
      CreateAt: "",
      Type: null,
      TypeName: "",
      TypeNameParent: "",
      DelFlg: false,
      Level: null,
      Email: "",
      Phone: "",
      ImageUrl: this.defaultImg,
      // Longitude: null,
      // Latitude: null,
      // Address: '',
      //AddressID : null
    };
    var errTemp = Object.assign({}, this.dataUnitItemtemp);
    errTemp.Type = null;
    this.dataErr = [errTemp];
    this.isUpdateData = false;
    this.filesUploadAvatar = "";
    this.fileSaveUrl = `${this.appService.apiRoot}api/Upload?dirName=${this.user.UserName}&typeData=files&acceptExtensions=[".xls",".xlsx", ".png", ".jpg", ".jpeg"]`;
  }

  // async initDisplay() {
  //   const resultAddress = await this.appService.doGET('api/Address/GetVW', null);
  //   if (resultAddress && resultAddress.Status === 1) {
  //     this.address = resultAddress.Data;
  //     this.addressFilter = this.address.slice();
  //   }
  // }

  onReload() {
    this.getUnits();
  }

  ngOnInit() {}

  public onTabSelect(e) {}

  onEditLangUnit(UnitID) {
    var selectedItem = this.LangUnit.find((item) => {
      return item.UnitID === UnitID;
    });
    selectedItem.IsAdd = false;
  }

  async getUnits() {
    this.appComponent.loading = true;
    const resultTypeUnit = await this.appService.doGET(
      "api/CommonMenu/GetByUnitType",
      null
    );
    if (resultTypeUnit && resultTypeUnit.Status === 1) {
      this.typeUnit = resultTypeUnit.Data;
      this.typeUnitFilter = this.typeUnit.slice();
    }

    const dataRequest = {

    };
    const result = await this.appService.doGET("api/Unit", dataRequest);
    var resultTemp = [];
    if (result && result.Status) {
      this.units = result.Data.slice();
      this.unitsFilter = result.Data.slice();
      // tslint:disable-next-line:prefer-for-of

      this.data.units = [];
      this.unitsChildOfUser = [];
      this.units = result.Data;
      for (let i = 0; i < result.Data.length; i++) {
        resultTemp.push({
          ActiveDate: null,
          CreateAt: null,
          DelFlg: false,
          ID: "ADD",
          Leader: null,
          Type: 3,
          Name: "+",
          ParentName: result.Data[i].Name,
          ParentID: result.Data[i].ID,
          UpdateAt: null,
        });
        // }
      }
      this.dataUnits = result.Data.concat(resultTemp);
      var listUnit = [];
      var listTop = this.dataUnits.filter(
        (x) => x.ParentID === "" || x.ParentID === null
      );
      listTop[0].ParentID = null;
      for (let i = 0; i < listTop.length; i++) {
        // if (this.selectedLangID == 'vi-VN'){
        listUnit.push({
          id: listTop[i].ID,
          name: listTop[i].Name,
          parentID: listTop[i].ParentID,
          level: listTop[i].Type,
          subordinates: this.getSubordinates(listTop[i].ID, this.dataUnits),
          //cssClass: listTop[i].Type == 1 ? 'firstSub' : listTop[i].Type == 2 ? 'secondSub' : listTop[i].Type == 3 ? 'addSub' : null
          cssClass: "firstSub",
        });
        var k = 0;
      }

      this.data.units = listUnit[0];
      // var j  = this.topEmployee;
    }
    this.appComponent.loading = false;
    //this.loading = false;
  }

  getSubordinates(id, unitList) {
    var result = [];
    var childList = unitList.filter((x) => x.ParentID == id);
    if (childList != null && childList != undefined && childList.length > 0) {
      for (let i = 0; i < childList.length; i++) {
        result.push({
          id: childList[i].ID,
          name: childList[i].Name,
          parentName: childList[i].ParentName,
          parentID: childList[i].ParentID,
          level: childList[i].Type,
          subordinates: this.getSubordinates(childList[i].ID, unitList),
          cssClass: childList[i].ID == "ADD" ? "addSub" : "firstSub",
        });
      }

      return result;
    } else {
      return null;
    }
  }

  async onSelectUnit(event) {

  if (event.level == 3 && (this.controlDefault || this.control.AddNew)) {
      this.setDefault();
      this.dataUnitItem.IsAdd = true;
      this.isParentFlag = false;
      this.isIDFlag = true;
      this.dataUnitItemtemp.ParentID = event.parentID;
      this.addUnitDialog = true;
      this.unitsFilter = this.units.slice();
    } else {
      // if (!this.unitsChildOfUser.find(r => r.ID === event.id)){
      var item = {
        IsAdd: true,
        ID: "",
        Name: "",
        ParentID: "",
        // ParentName: '',
        Type: 0,
      };
      // const dataRequest
      const result = await this.appService.doGET("api/Unit", { iD: event.id });
      if (result && result.Status === 1) {
        item.ID = event.id;
        item.Name = result.Data.Name;
        // item.ParentName = event.parentName;
        item.ParentID = result.Data.parentID;
        item.Type = result.Data.Type;
        this.onEditNewUnit(item);
      }
    }
  }

  async onSelectUnitTreeList(dataItem) {
    if (this.controlDefault || this.control.AddNew) {
      this.setDefault();
      this.dataUnitItem.IsAdd = true;
      this.isParentFlag = false;
      this.isIDFlag = true;
      this.dataUnitItemtemp.ParentID = dataItem.ID;
      this.addUnitDialog = true;
      this.unitsFilter = this.units.slice();
      this.tempData[0].ParentID = dataItem.ID;
    }
  }
  async onSelectUnitTreeListEdit(dataItem) {
    if (this.controlDefault || this.control.Edit) {
      // if (!this.unitsChildOfUser.find(r => r.ID === event.id)){
      var item = {
        IsAdd: true,
        ID: "",
        Name: "",
        ParentID: "",
        // ParentName: '',
        Type: 0,
      };
      // const dataRequest
      const result = await this.appService.doGET("api/Unit", {
        iD: dataItem.ID,
      });
      if (result && result.Status === 1) {
        item.ID = dataItem.ID;
        item.Name = result.Data.Name;
        // item.ParentName = event.parentName;
        item.ParentID = result.Data.parentID;
        item.Type = result.Data.Type;
        this.onEditNewUnit(item);
      }
    }
  }
  onCloseMemberDialog() {
    this.listMemberDialog = false;
    this.organizationComponent = false;
    this.appComponent.closeDialog();
    this.selectedLangID = this.appConsts.defaultLangID;
    // this.setDefault();
  }

  onSaveUnit() {
    if (!this.validateEmail(this.dataUnitItemtemp.Email)) {
      if (this.dataUnitItem.IsAdd) {
        this.addUnit();
      } else {
        this.updateUnit();
      }
    }
  }
  validatePhone() {
    if (this.dataUnitItemtemp.Phone) {
      if (
        this.dataUnitItemtemp.Phone.length < 10 ||
        this.dataUnitItemtemp.Phone.length > 13
      ) {
        return "Vui lòng nhập đúng số điện thoại";
      } else return "";
    }
  }

  // textOnly(event): boolean {
  //   const charCode = (event.which) ? event.which : event.keyCode;
  //   if (charCode > 47 && charCode < 58  ) {
  //     return false;
  //   }
  //   return true;
  // }

//cho nhập chữ và số. Không cho nhập ký tự đạt biệt
  filterInput(event) {
    var format = /[`!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/;
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && !format.test(event.key)) {
      return true;
    }

    return ["Backspace", "Delete", "ArrowLeft", "ArrowRight"].includes(
      event.code
    )
      ? true
      : (String(event.key) || event.code == "Space") &&
          format.test(event.key) == false;
  }

  validateEmail = (Email) => {
    if (Email == null || Email == "") {
      return false;
    }
    var a =
      /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

    if (Email.toLowerCase().match(a) || Email == "") {
      return (this.dataErr[0]["Email"] = "");
    }
    return (this.dataErr[0]["Email"] = "Vui lòng nhập đúng định dạng mail");
  };

  onAddNewUnit() {
    this.addUnitDialog = true;

    this.setDefault();
    this.isParentFlag = false;
    this.isIDFlag = true;

  }

  async onEditNewUnit(dataItem: any) {
    // this.setDefault();
    if (this.controlDefault || this.control.Edit) {
      console.log(dataItem);

      await this.getDataItemByID(dataItem.ID);
    }
  }




  async addUnit() {

    this.phoneEre = this.validatePhone();
    if(this.phoneEre === '' || this.phoneEre == null) {
      this.appComponent.loading = true;
      let dataRequest = this.createDataRequest(null);
      if (dataRequest.ImageUrl == this.defaultImg) {
        dataRequest.ImageUrl = null;
      }
      const result = await this.appService.doPOST("api/Unit", dataRequest);
      if (result && result.Status === 1) {
        this.notification.showSuccess(result.Msg);
        this.onReload();
        this.setDefault();
        this.appComponent.closeDialog();
        this.addUnitDialog = false;
      } else {
        if (!result.Msg) {
          this.dataErr = result.Data;
        } else {
          this.appSwal.showWarning(result.Msg, false);
        }
      }

      this.appComponent.loading = false;
    }
  }

  async updateUnit() {
    this.phoneEre = this.validatePhone();
    if(this.phoneEre === '' || this.phoneEre == null) {
      this.appComponent.loading = true;
      const iD = this.dataUnitItemtemp.ID;
      let dataRequest = this.createDataRequest(null);
      if (dataRequest.ImageUrl == this.defaultImg) {
        dataRequest.ImageUrl = null;
      }
      const result = await this.appService.doPUT("api/Unit", dataRequest, {
        iD,
      });
      if (result && result.Status === 1) {
        this.notification.showSuccess(result.Msg);
        this.onReload();
        this.onCloseUnitDialog();
      } else {
        if (!result.Msg) {
          this.dataErr = result.Data;
        } else {
          this.appSwal.showWarning(result.Msg, false);
        }
      }

      this.appComponent.loading = false;
    }
  }

  async onDeleteUnit(dataItem: any) {
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
      const result = await this.appService.doDELETE("api/Unit", dataRequest);
      if (result && result.Status === 1) {
        this.notification.showSuccess(result.Msg);
        this.onReload();
        this.setDefault();
        this.listMemberDialog = false;
        this.addUnitDialog = false;
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
    this.appComponent.closeDialog();
    this.appComponent.loading = false;
  }

  createDataRequest(data) {
    const temp = data ? data : this.dataUnitItemtemp;
    return {
      ID: temp.ID,
      Name: temp.Name,
      ParentID: temp.ParentID == null ? "" : temp.ParentID,
      //ParentName: temp.ParentName,
      //CreateAt: temp.CreateAt,
      DelFlg: temp.DelFlg,
      //Type: temp.Type == 0 ? null : temp.Type,
      //Level: temp.Level,
      //LangID: this.selectedLangID,
      Longitude: temp.Longitude,
      Latitude: temp.Latitude,
      // Type : temp.Type ,
      Email: temp.Email,
      Phone: temp.Phone,
      ImageUrl: temp.ImageUrl,
      //Address: temp.Address,
      //AddressID : temp.AddressID
    };
  }

  checkIsNum(value: any) {
    const x = +value;
    if (x.toString() == "NaN") {
      return false;
    }
    return true;
  }
  createLangDataRequest(data) {
    const temp = data ? data : this.dataUnitItemEnLanguage;
    return {
      ID: temp.ID,
      Name: temp.Name,
      UnitID: this.dataUnitItem.ID,
      CreateAt: temp.CreateAt,
      DelFlg: temp.DelFlg,
      UpdateAt: temp.UpdateAt,
    };
  }

  onCloseUnitDialog() {
    this.addUnitDialog = false;
    this.disabled = false;
    this.selectedLangID = this.appConsts.defaultLangID;
    // this.language.set(this.currentLang);
    // this.selectedLangID = this.appConsts.defaultLangID;
    // if(this.dataUnitItem.IsAdd || this.dataUnitItemtemp.IsAdd){this.setDefault();}
    if (!this.dataUnitItem.IsAdd) {
      if (!this.isUpdateData) {
        // this.setDefault();
        this.unitTemp = [];
        this.units.forEach((element) => {
          this.unitTemp.push(Object.assign({}, element));
        });

        var temp = this.unitTemp.find((item) => {
          return item.ID === this.dataUnitItemtemp.ID;
        });

        this.dataUnitItemtemp = Object.assign({}, temp);
        //this.dataUnitItemtemp.TypeName = this.levelUnitFilter.find(r => r.ID == this.dataUnitItemtemp.Type).Name;
        if (this.dataUnitItemtemp.ParentID) {
          //this.dataUnitItemtemp.TypeNameParent = this.levelUnitFilter.find(r => r.ID == this.dataUnitItemtemp.TypeParent).Name;
          this.dataUnitItemtemp.ParentName = this.unitsFilter.find(
            (r) => r.ID == this.dataUnitItemtemp.ParentID
          ).Name;
        }
      } else {
        //this.dataUnitItemtemp.TypeName = this.levelUnitFilter.find(r => r.ID == this.dataUnitItemtemp.Type).Name;
        if (this.dataUnitItemtemp.ParentID) {
          //this.dataUnitItemtemp.TypeNameParent = this.levelUnitFilter.find(r => r.ID == this.dataUnitItemtemp.TypeParent).Name;
          this.dataUnitItemtemp.ParentName = this.unitsFilter.find(
            (r) => r.ID == this.dataUnitItemtemp.ParentID
          ).Name;
        }
      }
    }
    this.setDefault();
    this.isUpdateData = false;
    this.appComponent.closeDialog();
    this.disabled = true;
    // this.dataErr = [];
  }

  unitsHandleFilter(value) {
    this.unitsFilter = this.unitFilterTemp.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }
  typeUnitHandleFilter(value) {
    this.typeUnitFilter = this.typeUnit.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }
  addressHandleFilter(value) {
    this.addressFilter = this.address.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  takeListUnitRecursive(listCleared, listRemain, id) {
    var listTemp = [];
    listRemain.forEach((element) => {
      if (listCleared.find((r) => r.ID === element.ParentID)) {
        listCleared.push(element);
        listRemain.forEach((e) => {
          listTemp.push(Object.assign({}, e));
        });
        listRemain.splice(listRemain.indexOf(element), 1);
      }
    });
    if (listTemp.length > 0) {
      this.takeListUnitRecursive(listCleared, listRemain, id);
    }
    if (listRemain.find((r) => r.ID === id)) {
      listRemain.splice(
        listRemain.indexOf(listRemain.find((r) => r.ID === id)),
        1
      );
    }
    return listRemain;
  }

  async GetAllLang() {
    //this.loading = true;

    const result = await this.appService.doGET("api/Lang/GetAllLang", null);
    if (result) {
      this.dataLangs = result.Data;
      var viLang = this.dataLangs.findIndex((x) => x.ID == "vi-VN");
      if (viLang != 0 && viLang != -1) {
        this.dataLangs.splice(viLang, 1);
        this.dataLangs.unshift({
          ID: "vi-VN",
          Name: "Tiếng Việt",
        });
      }
    }
    //this.loading = false;
  }

  onChangeLang(langID) {
    this.selectedLangID = langID;
    if (!this.dataUnitItem.IsAdd || !this.dataUnitItemtemp.IsAdd) {
      this.getDataItemByID(this.dataUnitItem.ID);
    }
    // this.language.set(this.selectedLangID);
  }
  async getDataItemByID(id: any) {
    const dataRequest = {
      iD: id,
      langID: this.selectedLangID,
    };

    const result = await this.appService.doGET("api/Unit", dataRequest);
    if (result && result.Status === 1) {
      this.dataUnitItem = result.Data;
      this.dataUnitItem.IsAdd = false;
      this.dataUnitItem.UnitID = result.Data.ID;
      this.dataUnitItemtemp = result.Data;
      this.dataUnitItemtemp.UnitID = result.Data.ID;
      this.dataUnitItemtemp.IsAdd = false;
      //this.uploadSaveAvatar = this.dataUnitItemtemp.ImageUrl;
      this.dataUnitItemtemp.ImageUrl = result.Data.ImageUrl
        ? this.appService.apiRoot + result.Data.ImageUrl
        : this.defaultImg;
      this.disabled = false;
      this.enabledID = false;
      this.addUnitDialog = true;
    } else {
      this.appSwal.showWarning(result.Msg, false);
    }
  }

  startDragging(e, el) {
    this.mouseDown = true;
    this.startX = e.pageX - el.offsetLeft;
    this.scrollLeft = el.scrollLeft;
  }
  stopDragging() {
    this.mouseDown = false;
  }
  // onSearchKeyPress(e: any) {
  //   // this.units = this.dataTemp;
  //   // let temp2 ;
  //   if (e.keyCode === 13) {
  //     if(this.searchOption.SearchText != ''){

  //       temp2 = this.units.filter((x) => x.Name.toLowerCase().includes(this.searchOption.SearchText.toLowerCase()));
  //       temp2.forEach(e => {
  //         this.findParentUnit(e);
  //       });
  //     }
  //   }
  // }
  // findParentUnit(unit: any){
  //   let a = this.units.filter((x) => x.ParentID.toLowerCase().includes(unit.toLowerCase()));

  //  this.units.push(a);
  // return this.findParentUnit(a);
  // }
  distinctTitles = (data) =>
    data
      .map((item) => ({
        value: item.title,
        text: item.title,
      }))
      .filter(
        (item, idx, arr) => arr.findIndex((i) => i.value === item.value) === idx
      );
  onSearchKeyPress(e: any) {
    if (e.keyCode === 13) {
    }
  }
  onRemoveSearchText() {
    this.searchOption.SearchText = "";
    this.getUnits();
  }
  moveEvent(e, el) {
    e.preventDefault();
    if (!this.mouseDown) {
      return;
    }
    const x = e.pageX - el.offsetLeft;
    const scroll = x - this.startX;
    el.scrollLeft = this.scrollLeft - scroll;
  }
  onExportExcelTemp(excelexport) {
    // const rows = excelexport.workbook.sheets[0].rows;

    // // align multi header
    // rows[1].cells[0].hAlign = 'center';
    excelexport.save();
  }
  onExportExcel(excelexport) {
    excelexport.save();
  }

  onSelectEventHandler(e: SelectEvent) {
    this.loadXLSX(e);
  }
  async loadXLSX(e) {
    const fileData = (await this.file.readXLSX(
      e.files[0].rawFile
    )) as Array<any>;
    var name = "";
    var parent = "";
    var id = "";
    var type = "";
    var errMsg = "";

    for (let i = 1; i < fileData.length; i++) {
      if (fileData[i].length > 0) {
        id = fileData[i][0].toString().trim();
        if (!id || (id && id.length <= 0)) {
          errMsg = "Mã đơn vị không được để trống";
          break;
        } else {
          id.toString().trim();
        }

        name = fileData[i][1];
        if (!name || (name && name.length <= 0)) {
          errMsg = "Tên đơn vị không được để trống";
          break;
        } else {
          name.toString().trim();
        }

        parent = fileData[i][2].toString().trim();
        if (!parent || (parent && parent.length <= 0)) {
          errMsg = "Đơn vị cha không được để trống";
          break;
        } else {
          parent.toString().trim();
        }

        // type = fileData[i][3];
        // if (!type || (type &&  type.length <= 0)) {
        //   errMsg = 'Phân loại không được để trống';
        //   break;
        // }
        // else{
        //   type.toString().trim();
        // }

        this.DataGrids.push({
          IsAdd: false,
          ID: id,
          Name: name,
          ParentID: parent,
          // Type: type
        });
      }
    }
    this.bindDataGrids();
    if (errMsg) {
      this.appSwal.showError(errMsg);
      this.importExcelSuccess = false;
      this.DataGrids = [];
    } else {
      this.importExcelSuccess = true;
    }
  }

  openMuiOrgTree() {
    this.dialogType = 1;
  }
  openMuiOrgChart() {
    this.dialogType = 2;
  }
  openImportExcelDialog() {
    this.DataGrids = [];
    this.dialogType = 3;
    this.importExcelSuccess = false;
  }
  onDataGridPageChange(event: PageChangeEvent) {
    this.dataDataGridSkip = event.skip;
    // this.bindDataGrids();
  }

  onDataGridSelectedKeysChange() {}

  bindDataGrids() {
    this.DataGridGridDataResult = {
      data: orderBy(this.DataGrids, this.DataGridSortByField),
      total: this.DataGrids.length,
    };
    this.DataGridGridDataResult = process(this.DataGrids, this.DataGridState);
  }

  public onDataGridDataStateChange(state: DataStateChangeEvent): void {
    this.DataGridSelection = [];
    this.DataGridState = state;
    this.DataGridGridDataResult = process(this.DataGrids, this.DataGridState);
  }

  async onSavePersonals() {
    const dataRequests = [];
    let dataGridsTemp = JSON.parse(JSON.stringify(this.DataGrids));
    for (let i = 0; i < dataGridsTemp.length; i++) {
      for (let j = 0; j < this.typeUnit.length; j++) {
        if (this.typeUnit[j].Name === dataGridsTemp[i].Type) {
          dataGridsTemp[i].Type = this.typeUnit[j].ID;
        }
      }

      dataRequests.push(dataGridsTemp[i]);
    }
    const result = await this.appService.doPOST("api/Unit/Saves", dataRequests);
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onReload();
      this.importExcelSuccess = false;
      this.DataGrids = [];
      this.dialogType = 1;
    } else {
      this.appSwal.showWarning(result.Msg, false);
    }
  }
  checkInput(event: any, value): void {
    if (
      (event.which != 46 ||
        (event.which == 46 && value == "") ||
        value.indexOf(".") != -1) &&
      (event.which < 48 || event.which > 57)
    ) {
      event.preventDefault();
    }
  }

  onRemoveAvatarUpload() {
    this.dataUnitItemtemp.ImageUrl = this.defaultImg;
    this.filesUploadName = "";
    this.filesUploadAvatar = "";
  }

  async onSelectAvatarUpload(e: SelectEvent) {
    if (!e.files || e.files.length <= 0) {
      return;
    }
    const extension = e.files[0].extension.toLowerCase();
    this.allowInsertFile = true;
    try {
      const fileData = e.files[0]; // await this.file.readFile(e.files[0].rawFile);
      const maxMB = 30;
      const maxSizeKB = 1024 * 1024 * maxMB;
      if (fileData.size > maxSizeKB) {
        this.allowInsertFile = false;
        this.appSwal.showWarning(
          this.translate.instant("Error_Size_Cannot_Be_Exceeded") +
            ` ${maxMB} MB`,
          false
        );
        return;
      }
    } catch {
      this.appSwal.showError(e);
    }
    // tslint:disable-next-line: max-line-length
    if (
      !extension ||
      (extension.toLowerCase() !== ".png" &&
        extension.toLowerCase() !== ".jpg" &&
        extension.toLowerCase() !== ".jpeg")
    ) {
      this.allowInsertFile = false;
      this.appSwal.showWarning(
        this.translate.instant("Error_Image_Extension"),
        false
      );
      return false;
    }
  }

  onSuccessAvatarUpload(e: any) {
    if (!this.allowInsertFile) {
      return;
    }
    try {
      if (this.dataUnitItemtemp.ImageUrl == undefined) {
        this.dataUnitItemtemp.ImageUrl = this.defaultImg;
      }
      this.uploadSaveAvatar = `${e.response.body.Data.DirMedia}${e.response.body.Data.MediaNm[0]}`;

      this.dataUnitItemtemp.ImageUrl =
        this.appService.apiRoot +
        `${e.response.body.Data.DirMedia}${e.response.body.Data.MediaNm[0]}`;
    } catch {
      this.appSwal.showError(e);
    }
  }

  onOpenEmpDetailDialog(id) {
    this.isOpenDetailDialog = true;
    this.organizationID = id;
  }
  onCloseDetailDialog() {
    this.isOpenDetailDialog = false;
  }
  numberOnly(event): boolean {
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;
  }
  onChangeFunction(e, dataItem) {
    if (e.id == "Add") {
      this.onSelectUnitTreeList(dataItem);
    } else if (e.id == "Edit") {
      this.onSelectUnitTreeListEdit(dataItem);
    }
  }
  onFunctionIconClick() {
    this.getbtnFunctionData();
  }

  getbtnFunctionData() {
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
    if (this.controlDefault || this.control.AddChild) {
      var func2 = {
        text: this.translate.instant("Unit_Add_Child_Org"),
        id: "Add",
        icon: "inherited",
      };
      this.btnFunctionData.push(func2);
      this.btnMbFunctionData.push(func2);
    }
  }
  getFileType(fileUrls) {
    var nameFile = "";
    if (fileUrls != "" && fileUrls != null) {
      var urlArr = fileUrls.split("/");
      if (urlArr.length > 0) {
        nameFile = urlArr[urlArr.length - 1];
        if (nameFile != "" && nameFile != null) {
          var indexOfLast = nameFile.lastIndexOf(".");
          nameFile = nameFile.substring(indexOfLast + 1);
        }
      }
    }
    return nameFile;
  }
}
