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
import 'rxjs/add/observable/interval';
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
import { DomSanitizer } from '@angular/platform-browser';
import { AppConsts } from 'src/app/services/app.consts';
import { delay, map } from 'rxjs/operators';
import { VariableAst } from '@angular/compiler';
import { isEmpty } from 'underscore';

@Component({
  selector: 'app-page',
  templateUrl: './page.component.html',
  //styleUrls: ['./page.component.css']
})
export class PageComponent implements OnInit, OnDestroy {
  

  user: any;
  popupClass = "popup-width";
  btnFunctionData: Array<any> = [];
  btnMbFunctionData: Array<any> = [];
  loading = false;
  dataGrids = [];
  dataGridSelectableSettings: SelectableSettings;
  dataGridSort = {
    allowUnsort: true,
    mode: 'multiple'
  };
  allowInsertFile = true;
  filesUpload: Array<FileInfo>;
  filesUploadName = "";
  urlDownload = this.appService.apiRoot;
  public fileSaveUrl: any;
  dataPages = [];
  maxMB = 30;
 maxSizeKB = 1024 * 1024 * this.maxMB;

  myRestrictions: FileRestrictions = {
    maxFileSize: this.maxSizeKB,
    minFileSize: 0,
    allowedExtensions: [".jpg", ".png", ,'.jpeg'],
    //  [accept]="['.png','.jpg','.jpeg']"
  };
  defaultUrls = AppConsts.page;
  public dataGridFocus = {
    Name: true
  };
  public dataPageFocus = {
    Name: true,
    Icon: true
  };
  dataGridSortByField: SortDescriptor[] = [];

  dataGridSkip = this.appConsts.pageSkip;
  dataGridPageSize = this.appConsts.pageSize;
  dataGridSelection: number[] = [];
  dataGridItem: any;
  dataLangs = [];
  public GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
  selectedLangID = this.appConsts.defaultLangID;

  customCss = 0;
  public type: 'numeric' | 'input' = 'numeric';
  public pageSizes = true;
  public previousNext = true;

  public dataGridState: State = {
    skip: this.dataGridSkip,
    take: this.dataGridSkip + this.dataGridPageSize,
    filter: {
      logic: 'and',
      filters: []
    },
  group: [{ field: 'ParentName' }]
  };
  dataGridGridDataResult: GridDataResult;

  dataItems: Array<{ Name: string, ID: string }>;
  dataItemsFilter: Array<{ Name: string, ID: string }>;


  public disabled = false;
  public enabledID = false;
  control: any;
  controlDefault = true;
  allowMulti = false;
  infoOpened = false;

  searchOption = {
    SearchText: ''
  };
  dataErr: any;

  pageName = '';

  pageUrl: Array<{ Name: string, ID: string }>;
  pageUrlsFilter: Array<{ Name: string, ID: string }>;
  parentPages: Array<{ Name: string, ID: string }>;
  parentPagesFilter: Array<{ Name: string, ID: string }>;

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
    this.getPageParent();
  }

  ngOnDestroy(): void {
  }

  ngOnInit() {
    this.getControl();
    this.setDefault();
    this.setSelectableSettings();
    this.onReload();
    this.initDisplay();
    this.allData = this.allData.bind(this);
    this.getPageName();

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

  async getPageName() {
    this.pageName = await this.appControls.getPageName();
  }
  async getControl() {
    this.control = await this.appControls.getControls(this.user.RoleID, AppConsts.page.page);
    this.controlDefault = false;
  }

  setSelectableSettings(): void {

    //this.allowMulti = !this.allowMulti;
    this.dataGridSelectableSettings = {
      checkboxOnly: false,
      mode: this.allowMulti ? 'multiple' : 'single'
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
    this.searchOption.SearchText = '';
    this.onSearch();
  }

  // dataItemsHandleFilter(value) {
  //   this.dataItemsFilter = this.dataItems.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  // }

  // parentHandleFilter(value){
    
  //   this.pagesFilter = this.pages.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  // }
  pageUrlsHandleFilter(value) {
    this.pageUrlsFilter = this.pageUrl.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }
  parentPagesHandleFilter(value) {
    this.parentPagesFilter = this.parentPages.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }


  async getdataItems() {
   
    const dataRequest = {
      searchText: this.searchOption.SearchText,
    };

    const result = await this.appService.doGET('api/Page/Search', dataRequest);
    if (result) {
      
      this.dataGrids = result.Data;
      this.dataGridSkip = this.appConsts.pageSkip;
      this.dataGridState  = {
        skip: this.dataGridSkip,
        take: this.dataGridSkip + this.dataGridPageSize,
        filter: {
          logic: 'and',
          filters: []
        },
      group: [{ field: 'ParentName' }]
      };

     this.loading = true;
     this.binddataItems();

      this.loading = false;
      this.dataItems = [];
      result.Data.forEach(item => {
        
          this.dataItems.push(item);
        
      });
    }

  
  }

  setDefault() {
    this.dataGridItem = {
      IsAdd: true,
      ID: '',
      ParentID: null,
      Name: null,
      MenuFlg: true,
      ButtonFlg:false,
      OrdinalNumber: 0,
      DelFlg: false,
      Icon: '',
      FileUrls: ''
    };
    var errTemp = Object.assign({}, this.dataGridItem);
    errTemp.Type = null;
    this.dataErr = [errTemp];
    this.customCss = 0;
    this.dataGridSelection = [];
    this.disabled = false;
    this.enabledID = true;
    this.dataPageFocus.Name = true;
    this.filesUpload = [];
    this.filesUploadName = "";
    //this.fileSaveUrl = `${this.appService.apiRoot}api/Upload/MediaWeb?personalID=${this.user.UserName}&typeData=files`;
    this.fileSaveUrl = `${this.appService.apiRoot}api/Upload?dirName=${this.user.UserName}&typeData=files&acceptExtensions=[".png",".jpg",".jpeg",".gif"]`;
  }

  onGridPageChange(event: PageChangeEvent) {
    this.dataGridSkip = event.skip;
    this.binddataItems();
  }

  ondataItemselectedKeysChange() {

  }

  binddataItems() {
    this.dataGridGridDataResult = {
      data: orderBy(this.dataGrids, this.dataGridSortByField),
      total: this.dataGrids.length
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
    this.searchOption.SearchText = '';
    this.getdataItems();

  }

  async initDisplay() {



  }



  onSearch() {
    this.getdataItems();
  }

  onReload() {
    //this.searchOption.SearchText = '';
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
    if (this.dataGridItem.IsAdd) { this.addGrid(); } else { this.updateGrid(); }
  }

  createDataRequest(data) {

    const temp = data ? data : this.dataGridItem;
    return {
      ID: temp.ID,
      Name: temp.Name,
      ParentID: temp.ParentID,
      PageID: temp.PageID,
      MenuFlg: temp.MenuFlg,
      // HomeFlg: temp.HomeFlg,
      // HomeOrderIdx: temp.HomeOrderIdx,
      // HomeDisplayType: temp.HomeDisplayType,
      ButtonFlg: temp.ButtonFlg,
      DelFlg: temp.DelFlg,
      OrdinalNumber: temp.OrdinalNumber,
     // OrderIdxInPost: temp.OrderIdxInPost,
      // WidthBanner: temp.WidthBanner,
      // HeightBanner: temp.HeightBanner,
      Icon: temp.Icon,
      FileUrls: temp.FileUrls,
      LangID: this.selectedLangID,
    };
  }

  onCloseGrid(status: any) {
    this.disabled = true;
    this.enabledID = false;
    this.infoOpened = true;
  }

  async onEditGrid(dataItem: any) {
  this.selectedLangID = this.appConsts.defaultLangID;;
    await this.getDataItemByID(dataItem.ID)
  }

  async getDataItemByID(id: any) {

    const dataRequest = {
      iD: id != null ? id : "",
      langID: this.selectedLangID
    };
    const result = await this.appService.doGET('api/Page', dataRequest);
    if (result && result.Status === 1) {
      this.dataGridItem = result.Data;
      this.dataGridItem.MenuID = result.Data.ID;
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
    const result = await this.appService.doPOST('api/Page', dataRequest);
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);

      this.clearErrMsg();

      this.onReload();
      this.onAddNewGrid();
    } else {
      if (!result.Msg) {
        this.dataErr = result.Data;
        var count = 0;
        for (var prop in this.dataErr[0]) {
          count++;
        }
        this.customCss = count;
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
    this.appComponent.loading = false;
  }
  async updateGrid() {
    this.appComponent.loading = true;
    const id = this.dataGridItem.ID;
    const dataRequest = this.createDataRequest(null);

    const result = await this.appService.doPUT('api/Page', dataRequest, { id});
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      // this.dataGridItem ={};
      // var errTemp = Object.assign({}, this.dataGridItem);
      // errTemp.Type = null;
      // this.dataErr = [errTemp];
      this.clearErrMsg();
      this.onReload();
      this.onCloseInfo();
    } else {
      if (!result.Msg) {
        this.dataErr = result.Data;
        var count = -1;
        for (var prop in this.dataErr[0]) {
          count++;
        }
        this.customCss = count;
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
    this.appComponent.loading = false;
  }

  checkErr() {
    if(this.customCss == 1) {
      return "x-dialog-unit-contact-err-one";
    } else if(this.customCss == 2) {
      return "x-dialog-unit-contact-err-two";
    }else if(this.customCss == 3) {
      return "x-dialog-unit-contact-err-three";
    }else if(this.customCss == 4) {
      return "x-dialog-unit-contact-err-four";
    } else {
      return "x-dialog-unit-contact"
    }
  }

  async onDeleteGridChangeLang(dataItem: any) {
    this.appComponent.loading = true;
    const dataRequest = {
      iD: dataItem.ID,
      langID: this.selectedLangID
    };

    const option = await this.appSwal.showWarning(this.translate.instant('AreYouSure'), true);
    if (option) {
      const result = await this.appService.doDELETE('api/Page', dataRequest);
      if (result && result.Status === 1) {
        this.notification.showSuccess(result.Msg);
        this.onReload();
        this.onCloseInfo();
        this.dataGridSelection = [];
        this.allowMulti = false;
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
      langID: this.appConsts.defaultLangID
    };

    const option = await this.appSwal.showWarning(this.translate.instant('AreYouSure'), true);
    if (option) {
      const result = await this.appService.doDELETE('api/Page', dataRequest);
      if (result && result.Status === 1) {
        this.notification.showSuccess(result.Msg);
        this.onReload();
        this.onCloseInfo();
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
      data: process(this.dataGrids, {}).data
    };

    return result;
  }

  onCloseInfo() {
    this.infoOpened = false;
    this.setDefault();
  }

  onChangeFunction(e, dataItem) {
    if (e.id == 'Edit') {
      this.onEditGrid(dataItem);
    }
    else if (e.id == 'Delete') {
      this.onDeleteGrid(dataItem)
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
        text: this.translate.instant('Edit'),
        id: 'Edit',
        icon: 'pencil',
      };
      this.btnFunctionData.push(func1);
      this.btnMbFunctionData.push(func1);

    }



    if (this.controlDefault || this.control.Delete) {
      var func3 = {
        text: this.translate.instant('Delete'),
        id: 'Delete',
        icon: 'delete',
      };
      this.btnFunctionData.push(func3);
      this.btnMbFunctionData.push(func3);

    }
  }

  onChangeLang(langID) {
    this.selectedLangID = langID;
    var tempItem = {
      IsAdd: true,
      ID: null,
      ParentID: null,
      Name: null,
      MenuFlg: true,
      ButtonFlg:false,
      OrdinalNumber: 0,
      DelFlg: false,
      Icon: '',
      FileUrls: '',
      Type: null
      
    };
    var errTemp = Object.assign({}, tempItem);
    errTemp.Type = null;
    this.dataErr = [errTemp];
    this.getDataItemByID(this.dataGridItem.ID);
  }

  Types: Array<{ Name: string, ID: number }>;
  TypesFilter: Array<{ Name: string, ID: number }>;
  

  pages: Array<{ Name: string, ID: string }>;
  pagesFilter: Array<{ Name: string, ID: string }>;

  async getPageParent() {
    this.loading = true;
    const dataRequest = {
      searchText: '',
    };
    const result = await this.appService.doGET('api/Page/Search', dataRequest);
    if (result && result.Status === 1) {
      this.dataPages = result.Data;
      this.parentPages = [];
      this.parentPages.push({
        ID: null,
        Name: 'Root'
      });
      this.dataPages.forEach(item => {
        if (!item.ParentID) {
          this.parentPages.push(item);
        }
      });
      this.parentPagesFilter = this.parentPages.slice();
     // this.bindPages();
    }
    this.loading = false;
   // this.checkSelectionID();
    
  }
  // parentPagesHandleFilter(value) {
  //   this.pagesFilter = this.pages.filter((s) => s.ID.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  // }
  public dataMenuFocus = {
    Name: true
  };
  // menuNameChange(e : any){
  //   if (e && this.selectedLangID == this.appConsts.defaultLangID){
  //     this.dataGridItem.FileUrls = this.cleanAccents(e);
  //   }    
  // } 
  // cleanAccents (str: string){
  //   str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g,"a"); 
  //   str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g,"e"); 
  //   str = str.replace(/ì|í|ị|ỉ|ĩ/g,"i"); 
  //   str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g,"o"); 
  //   str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g,"u"); 
  //   str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g,"y"); 
  //   str = str.replace(/đ/g,"d");
  //   str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
  //   str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
  //   str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
  //   str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
  //   str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
  //   str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
  //   str = str.replace(/Đ/g, "D");
  //   // Some system encode vietnamese combining accent as individual utf-8 characters
  //   // Một vài bộ encode coi các dấu mũ, dấu chữ như một kí tự riêng biệt nên thêm hai dòng này
  //   str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // ̀ ́ ̃ ̉ ̣  huyền, sắc, ngã, hỏi, nặng
  //   str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // ˆ ̆ ̛  Â, Ê, Ă, Ơ, Ư    
  //   str = str.replace(/[!&\/\\#,+()$~%.'":*?<>{} ]/g,'-');     
  //   str = str.trim();
  //   return str;
  // }


   //select file to upload
   async onSelectFileToUpload(e: SelectEvent) {
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
          this.translate.instant('Error_Size_Cannot_Be_Exceeded') +` ${maxMB} MB`,
          false
        );
        return false;
      }
    } catch {
      this.appSwal.showError(e);
    }
    // tslint:disable-next-line: max-line-length
    if (
      !extension ||
      (extension.toLowerCase() !== ".jpeg" &&
        extension.toLowerCase() !== ".jpg" &&
        extension.toLowerCase() !== ".png")
    ) {
      this.allowInsertFile = false;
      this.appSwal.showWarning(
        this.translate.instant('Error_Image_Extension'),
        false
      );
      return false;
    }
  }
  
  onRemoveFileDocToUpload() {
    this.dataGridItem.FileUrls = [];
    this.filesUploadName = "";
    this.filesUpload = [];
  }
  
  getUrlDownload(item) {
    let url = this.urlDownload.replace(/\"/g, "") + item;
    url = url.replace(/\"/g, "");
    return url;
  }
  
  getFileName(fileUrls) {
    var nameFile = "";
    if (fileUrls != "" && fileUrls != null) {
      var urlArr = fileUrls.split("/");
      if (urlArr.length > 0) {
        nameFile = urlArr[urlArr.length - 1];
        if (nameFile != "" && nameFile != null) {
          var indexOfFirst = nameFile.indexOf("_");
          nameFile = nameFile.substring(indexOfFirst + 1);
        }
      }
    }
    return nameFile;
  }
  
  onRemoveFile(file) {
    
      this.dataGridItem.FileUrls = '';
  }
  
  onSuccessFileToUpload(e: any) {
    if (!this.allowInsertFile) {
      return;
    }
    try {
      if (this.dataGridItem.FileUrls == undefined) {
        this.dataGridItem.FileUrls = '';
      }
      this.dataGridItem.FileUrls =
        `${e.response.body.Data.DirMedia}${e.response.body.Data.MediaNm[0]}`;
    } catch {
      this.appSwal.showError(e);
    }
  }
  downloadItem(item) {
    window.open(this.getUrlDownload(item), "_blank");
  }
  
  clearErrMsg(){

    var errTemp = Object.assign({}, this.dataGridItem);
    errTemp.Type = null;
    this.dataErr = [errTemp];
  }

  valueNormalizerAdmin = (text: Observable<string>) => text.pipe(map((text: string) => {
    if (!text.replace(/\s/g, '').length) {
      return
    }
    const matchingValue: any = this.pageUrl.find((item: any) => {
      return item.Name.toLowerCase() === text.toLowerCase();
    });
    if (matchingValue) {
      return matchingValue; //return the already selected matching value and the component will remove it
    }
    return { ID: text, Name: text };
  }));

}
