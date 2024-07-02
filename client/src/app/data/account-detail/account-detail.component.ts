import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
} from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { AppService } from "../../services/app.service";
import { AppSwal } from "src/app/services/app.swal";
import { IntlService } from "@progress/kendo-angular-intl";
import { Notification } from "../../services/app.notification";
import "rxjs/add/observable/interval";
import { AppGuid } from "src/app/services/app.guid";
import {
  FileRestrictions,
  SelectEvent,
  FileInfo,
} from "@progress/kendo-angular-upload";
import { AppFile } from "src/app/services/app.file";
import { AuthenticationService } from "src/app/services/authentication.service";
import { AppControls } from "src/app/services/app.controls";
import { AppUtils } from "src/app/services/app.utils";
import { AppComponent } from "../../app.component";
import { DomSanitizer } from "@angular/platform-browser";
import { AppConsts } from "src/app/services/app.consts";
import { ActivatedRoute } from "@angular/router";
//import CKFinder from '@ckeditor/ckeditor5-ckfinder/src/ckfinder';
@Component({
  selector: "app-account-detail",
  templateUrl: "./account-detail.component.html",
})
export class AccountDetailComponent implements OnInit {
  @Input() request_AccountID;
  @Output() isReloadByViewChild = new EventEmitter<any>();
  @Output() newAccount = new EventEmitter();
  user: any;
  popupClass = "popup-width";
  btnFunctionData: Array<any> = [];
  btnMbFunctionData: Array<any> = [];
  loading = false;

  dataLangs = [];

  myRestrictionsImage: FileRestrictions = {
    allowedExtensions: [".jpg", ".jpeg", ".png"],
  };

  myRestrictionsFile: FileRestrictions = {
    allowedExtensions: [".doc", ".docx", ".pdf"],
  };

  public type: "numeric" | "input" = "numeric";
  public pageSizes = true;
  public previousNext = true;

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
  };
  dataErr: any;
  phoneEre: any;
  pageName = "";
  filesUploadAvatar: "";
  filesUploadName = "";
  filesUpload: Array<FileInfo>;
  urlDownload = this.appService.apiRoot;
  public fileSaveUrl: any;
  dataPersonalItem: any;
  dataPersonalItemtemp: any;
  public uploadSaveAvatar = "";
  IsVietnameseTab = true;

  public Enum = {
    Post_Reference: 0, // nguồn tin bài
    Position: 1, // chức vụ
    Ethnic: 2, // Dân tộc
    PoliticalTheory: 3, // Lý luận chính trị
    CommunistPartyPosition: 4, // Chức vụ đảng
    UnitType: 5, // Phân loại đơn vị (khối,...)
    Qualification: 6, // trình độ chuyên môn
  };

  gender: Array<{ Name: string; ID: number }> = [];
  genderFilter: Array<{ Name: string; ID: number }> = [];

  positions: Array<{ ID: string; Name: string }> = [];
  positionsFilter: Array<{ ID: string; Name: string }> = [];

  unit: Array<{ Name: string; ID: string }> = [];
  unitFilter: Array<{ Name: string; ID: string }> = [];

  ethnic: Array<{
    Name: string;
    ID: string;
    Type: number;
    OrderIndex: number;
  }> = [];
  ethnicFilter: Array<{
    Name: string;
    ID: string;
    Type: number;
    OrderIndex: number;
  }> = [];

  politicalTheory: Array<{
    Name: string;
    ID: string;
    Type: number;
    OrderIndex: number;
  }> = [];
  politicalTheoryFilter: Array<{
    Name: string;
    ID: string;
    Type: number;
    OrderIndex: number;
  }> = [];

  communistPartyPosition: Array<{
    Name: string;
    ID: string;
    Type: number;
    OrderIndex: number;
  }> = [];
  communistPartyPositionFilter: Array<{
    Name: string;
    ID: string;
    Type: number;
    OrderIndex: number;
  }> = [];

  qualification: Array<{
    Name: string;
    ID: string;
    Type: number;
    OrderIndex: number;
  }> = [];
  qualificationFilter: Array<{
    Name: string;
    ID: string;
    Type: number;
    OrderIndex: number;
  }> = [];

  Personalelection: number[] = [];
  Personal: any;
  allowInsertFile = true;
  @Input() HiddenTab;
  selectedLangID = this.appConsts.defaultLangID;
  dataResultOutput = {
    ID: "",
    SttResultFromUser: true,
  };

  constructor(
    private translate: TranslateService,
    private appService: AppService,
    private appSwal: AppSwal,
    public intl: IntlService,
    private notification: Notification,
    private guid: AppGuid,
    private authenticationService: AuthenticationService,
    public appControls: AppControls,
    private appComponent: AppComponent,
    public appConsts: AppConsts,
    private activatedRoute: ActivatedRoute
  ) {
    this.authenticationService.getUser();
    this.user = this.authenticationService.user;
  }

  ngOnDestroy(): void { }

  ngOnInit() {
    this.getControl();
    this.setDefault();
    this.getAccoundID();
    this.getPageName();
  }

  getAccoundID() {
    this.activatedRoute.queryParams.subscribe(async (params: any) => {
      if (params && params.accountID) {
        this.request_AccountID = params.accountID;
      }

      this.getPersonal();
    });
  }

  async getPageName() {
    this.pageName = await this.appControls.getPageName();
  }
  async getControl() {
    this.control = await this.appControls.getControls(
      this.user.RoleID,
      AppConsts.page.accountDetail
    );
    this.controlDefault = false;
  }

  setDefault() {
    this.dataPersonalItem = {
      ID: this.guid.empty,
      Name: null,
      BirthDate: null,
      Gender: 0,
      Email: null,
      Phone: null,
      AvatarUrl: this.user.FilePath,
      UnitID: null,
      PositionID: null,
      UpdateAt: null,
      HomeTown: null,
      Ethnic: null,
      Qualification: null,
      Specialized: null,
      PoliticalTheory: null,
      YouthGroupDate: null,
      CommunistPartyDate: null,
      CommunistPartyPosition: null,
      Note: null,
    };
    this.dataPersonalItemtemp = Object.assign({}, this.dataPersonalItem);

    var errTemp = Object.assign({}, this.dataPersonalItemtemp);
    this.dataErr = [errTemp];

    this.disabled = false;
    this.enabledID = true;
    this.filesUploadAvatar = "";
    this.filesUploadName = "";
    this.fileSaveUrl = `${this.appService.apiRoot}api/Upload?dirName=${this.user.UserName}&typeData=files&acceptExtensions=[".png",".jpg",".jpeg",".gif"]`;
  }

  ondataItemselectedKeysChange() { }

  onSearchTextChange() {
    if (!this.searchOption.SearchText) {
      this.onReload();
    }
  }

  onRemoveSearchText() {
    this.searchOption.SearchText = "";
  }

  async initDisplay() {
    this.getPersonal();
  }

  async getFilter() {
    const resultCatolories = await this.appService.doGET("api/Unit", null);
    if (resultCatolories && resultCatolories.Status === 1) {
      this.unit = resultCatolories.Data;
      this.unitFilter = this.unit.slice();
    }

    const resultCommon = await this.appService.doGET("api/CommonMenu", null);
    if (resultCommon && resultCommon.Status === 1) {
      this.ethnic = resultCommon.Data;
      this.ethnic = this.ethnic.filter((x) => x.Type == this.Enum.Ethnic);
      this.ethnicFilter = this.ethnic.slice();
      this.ethnicFilter.sort(function (a, b) {
        return a.OrderIndex - b.OrderIndex;
      });

      this.politicalTheory = resultCommon.Data;
      this.politicalTheory = this.politicalTheory.filter(
        (x) => x.Type == this.Enum.PoliticalTheory
      );
      this.politicalTheoryFilter = this.politicalTheory.slice();
      this.politicalTheoryFilter.sort(function (a, b) {
        return a.OrderIndex - b.OrderIndex;
      });

      this.communistPartyPosition = resultCommon.Data;
      this.communistPartyPosition = this.communistPartyPosition.filter(
        (x) => x.Type == this.Enum.CommunistPartyPosition
      );
      this.communistPartyPositionFilter = this.communistPartyPosition.slice();
      this.communistPartyPositionFilter.sort(function (a, b) {
        return a.OrderIndex - b.OrderIndex;
      });

      this.qualification = resultCommon.Data;
      this.qualification = this.qualification.filter(
        (x) => x.Type == this.Enum.Qualification
      );
      this.qualificationFilter = this.qualification.slice();
      this.qualificationFilter.sort(function (a, b) {
        return a.OrderIndex - b.OrderIndex;
      });
    }

    const resultPosition = await this.appService.doGET(
      "api/CommonMenu/GetPositions",
      null
    );
    if (resultPosition && resultPosition.Status === 1) {
      this.positions = resultPosition.Data;
      this.positionsFilter = this.positions.slice();
    }

    const resultGender = await this.appService.doGET(
      "api/Enum/GetEGender",
      null
    );
    if (resultGender && resultGender.Status === 1) {
      this.gender = resultGender.Data;
      this.genderFilter = this.gender.slice();
    }
  }

  async getPersonal() {
    this.loading = true;

    this.getFilter();

    const dataRequest = {
      iD: this.request_AccountID,
    };

    const resultPersonal = await this.appService.doGET("api/Account", dataRequest);
    if (
      resultPersonal &&
      resultPersonal.Status === 1 &&
      resultPersonal.Data != null
    ) {
      this.dataPersonalItem = resultPersonal.Data;
      this.dataPersonalItem.BirthDate = this.dataPersonalItem.BirthDate
        ? new Date(this.dataPersonalItem.BirthDate)
        : null;
      this.dataPersonalItem.YouthGroupDate = this.dataPersonalItem
        .YouthGroupDate
        ? new Date(this.dataPersonalItem.YouthGroupDate)
        : null;
      this.dataPersonalItem.CommunistPartyDate = this.dataPersonalItem
        .CommunistPartyDate
        ? new Date(this.dataPersonalItem.CommunistPartyDate)
        : null;
      this.bindtemp(this.dataPersonalItem);

      this.uploadSaveAvatar = this.dataPersonalItemtemp.AvatarUrl;
      this.dataPersonalItemtemp.AvatarUrl = this.dataPersonalItemtemp.AvatarUrl
        ? this.appService.apiRoot + this.dataPersonalItemtemp.AvatarUrl
        : this.user.FilePath;
    }
    this.loading = false;
    this.checkSelectionID();
  }

  bindtemp(item) {
    this.dataPersonalItemtemp = Object.assign({}, item);
  }

  checkSelectionID() {
    for (let i = this.Personalelection.length - 1; i >= 0; i--) {
      const selectedItem = this.Personal.find((item) => {
        return item.ID === this.Personalelection[i];
      });
      if (!selectedItem) {
        this.Personalelection.splice(i, 1);
      }
    }
  }

  onReload() {
    this.getPersonal();
  }

  // async onSavePersonal() {

  //       if (this.dataPersonalItemtemp.ID == this.guid.empty) {
  //         await this.addPersonal();
  //         var addData = {
  //           Name: "successAdd",
  //           Account: this.dataResultOutput,
  //         };
  //         this.newAccount.emit(addData);
  //       } else {
  //         await this.updatePersonal();
  //         var addData = {
  //           Name: "successAdd",
  //           Account: this.dataResultOutput,
  //         };
  //         this.newAccount.emit(addData);
  //       }

  //   }

  // async onSavePersonal() {
  //   this.phoneEre = this.validatePhone();
  //   console.log(this.phoneEre);
  //   if (this.dataPersonalItemtemp.ID == this.guid.empty) {
  //     if (this.phoneEre) {
  //       await this.addPersonal();
  //       var addData = {
  //         Name: "successAdd",
  //         Account: this.dataResultOutput,
  //       };
  //       this.newAccount.emit(addData);
  //     } else {
  //       await this.updatePersonal();
  //       var addData = {
  //         Name: "successAdd",
  //         Account: this.dataResultOutput,
  //       };
  //       this.newAccount.emit(addData);
  //     }
  //   }
  // }

  async onSavePersonal() {
    if (this.dataPersonalItemtemp.ID == this.guid.empty) {
      await this.addPersonal();
      var addData = {
        Name: "successAdd",
        Account: this.dataResultOutput
      }
      this.newAccount.emit(addData);
    }
    else {
      await this.updatePersonal();
      var addData = {
        Name: "successAdd",
        Account: this.dataResultOutput
      }
      this.newAccount.emit(addData);
    }
  }
  // filterInput(event) {
  //   var format = /[`!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/;

  //   return ["Backspace", "Delete", "ArrowLeft", "ArrowRight"].includes(
  //     event.code
  //   )
  //     ? true
  //     : (isNaN(Number(event.key)) || event.code == "Space") &&
  //         format.test(event.key) == false;
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
  validatePhone() {
    if (this.dataPersonalItemtemp.Phone) {
      if (
        this.dataPersonalItemtemp.Phone.length < 9 ||
        this.dataPersonalItemtemp.Phone.length > 13
      ) {
        return "Vui lòng nhập đúng số điện thoại";
      } else return "";
    }
  }

  createDataRequest(data) {
    const temp = data ? data : this.dataPersonalItemtemp;

    return {
      ID: temp.ID ? temp.ID : this.guid.empty,
      Name: temp.Name,
      BirthDate: temp.BirthDate
        ? this.intl.formatDate(new Date(temp.BirthDate), "yyyy-MM-ddT00:00:00")
        : null,
      Gender: temp.Gender,
      Email: temp.Email,
      Phone: temp.Phone,
      AvatarUrl:
        this.dataPersonalItemtemp.AvatarUrl !== this.user.FilePath
          ? this.uploadSaveAvatar
          : null,
      UnitID: temp.UnitID,
      PositionID: temp.PositionID,
      UpdateAt: temp.UpdateAt,
      HomeTown: temp.HomeTown,
      Ethnic: temp.Ethnic,
      Qualification: temp.Qualification,
      Specialized: temp.Specialized,
      PoliticalTheory: temp.PoliticalTheory,
      YouthGroupDate: temp.YouthGroupDate
        ? this.intl.formatDate(
          new Date(temp.YouthGroupDate),
          "yyyy-MM-ddT00:00:00"
        )
        : null,
      CommunistPartyDate: temp.CommunistPartyDate
        ? this.intl.formatDate(
          new Date(temp.CommunistPartyDate),
          "yyyy-MM-ddT00:00:00"
        )
        : null,
      CommunistPartyPosition: temp.CommunistPartyPosition,
      Note: temp.Note,
    };
  }

  async addPersonal() {
    this.phoneEre = this.validatePhone();
    if (this.phoneEre === '' || this.phoneEre == null) {
      this.appComponent.loading = true;
      const dataRequest = this.createDataRequest(null);
      const result = await this.appService.doPOST("api/Account", dataRequest);
      if (result && result.Status === 1) {
        this.dataResultOutput.ID = result.Data;
        this.dataResultOutput.SttResultFromUser = true;
        this.notification.showSuccess(result.Msg);
        this.onReload();
        this.setDefault();
        this.isReloadByViewChild.emit(this.dataResultOutput.ID);
      } else {
        if (!result.Msg) {
          this.dataErr = result.Data;
        } else {
          this.appSwal.showWarning(result.Msg, false);
          this.dataResultOutput.SttResultFromUser = false;
        }
      }
      this.appComponent.loading = false;
    }
  }

  async updatePersonal() {
    this.phoneEre = this.validatePhone();
    if (this.phoneEre === '' || this.phoneEre == null) {
      this.appComponent.loading = true;

      const iD = this.request_AccountID;
      const dataRequest = this.createDataRequest(null);

      const result = await this.appService.doPUT("api/Account", dataRequest, {
        iD,
      });
      if (result && result.Status === 1 && result != null) {
        this.dataResultOutput.ID = result.Data;
        this.dataResultOutput.SttResultFromUser = true;
        this.notification.showSuccess(result.Msg);
        this.onReload();
        this.isReloadByViewChild.emit(this.dataResultOutput.ID);
      } else {
        if (!result.Msg) {
          this.dataErr = result.Data;
        } else {
          this.appSwal.showWarning(result.Msg, false);
          this.dataResultOutput.SttResultFromUser = false;
        }
      }
      this.appComponent.loading = false;
    }
  }
  onChangeLang(langID) {
    var tempItem = {
      Phone: null,
      BirthDate: null,
      Email: null,
      Gender: null,
      Address: null,
      Name: null,
      HomeTown: null,
      CMND: null,
      Nationality: null,
    };
    var errTemp = Object.assign({}, tempItem);
    this.dataErr = [errTemp];
    this.IsVietnameseTab = langID == "vi-VN" ? true : false;
    this.selectedLangID = langID;
    this.getPersonal();
  }

  onRemoveAvatarUpload() {
    this.dataPersonalItemtemp.AvatarUrl = "";
    this.filesUploadName = "";
    this.filesUploadAvatar = "";
  }
  //select file to upload
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
        this.translate.instant("Error_Image_File_Extension"),
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
      if (this.dataPersonalItemtemp.AvatarUrl == undefined) {
        this.dataPersonalItemtemp.AvatarUrl = this.user.FilePath;
      }
      this.uploadSaveAvatar = `${e.response.body.Data.DirMedia}${e.response.body.Data.MediaNm[0]}`;

      this.dataPersonalItemtemp.AvatarUrl =
        this.appService.apiRoot +
        `${e.response.body.Data.DirMedia}${e.response.body.Data.MediaNm[0]}`;
    } catch {
      this.appSwal.showError(e);
    }
  }
  ////////////////////////////////////

  onRemoveFileDocToUpload() {
    this.dataPersonalItemtemp.FileUrls = [];
    this.filesUploadName = "";
    this.filesUpload = [];
  }
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
      (extension.toLowerCase() !== ".doc" &&
        extension.toLowerCase() !== ".docx" &&
        extension.toLowerCase() !== ".pdf")
    ) {
      this.allowInsertFile = false;
      this.appSwal.showWarning(
        this.translate.instant("Error_Document_File_Extension"),
        false
      );
      return false;
    }
  }

  onSuccessFileToUpload(e: any) {
    if (!this.allowInsertFile) {
      return;
    }
    try {
      if (this.dataPersonalItemtemp.FileUrls == undefined) {
        this.dataPersonalItemtemp.FileUrls = [];
      }
      this.dataPersonalItemtemp.FileUrls.push(
        `${e.response.body.Data.DirMedia}${e.response.body.Data.MediaNm[0]}`
      );
    } catch {
      this.appSwal.showError(e);
    }
  }

  getUrlDownload(item) {
    let url = this.urlDownload.replace(/\"/g, "") + item;
    url = url.replace(/\"/g, "");
    return url;
  }

  onRemoveFile(file) {
    var isExistedInFile = this.dataPersonalItemtemp.FileUrls.findIndex(
      (x) => x == file
    );
    if (isExistedInFile != -1) {
      this.dataPersonalItemtemp.FileUrls.splice(isExistedInFile, 1);
    }
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

  numberOnly(event): boolean {
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;
  }
  genderHandleFilter(value) {
    this.genderFilter = this.gender.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }
  unitHandleFilter(value) {
    this.unitFilter = this.unit.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }
  positionsHandleFilter(value) {
    this.positionsFilter = this.positions.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  ethnicHandleFilter(value) {
    this.ethnicFilter = this.ethnic.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }
  politicalTheoryHandleFilter(value) {
    this.politicalTheoryFilter = this.politicalTheory.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }
  communistPartyPositionHandleFilter(value) {
    this.communistPartyPositionFilter = this.communistPartyPosition.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }
  qualificationHandleFilter(value) {
    this.qualificationFilter = this.qualification.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }
}
