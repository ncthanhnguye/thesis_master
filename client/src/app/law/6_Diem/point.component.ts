import { Component, OnInit, OnDestroy } from '@angular/core';
import { AppService } from 'src/app/services/app.service';
import { SelectableSettings, PageChangeEvent, GridDataResult, DataStateChangeEvent } from '@progress/kendo-angular-grid';
import { State, process, SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { AppSwal } from 'src/app/services/app.swal';
import { IntlService } from '@progress/kendo-angular-intl';
import { Notification } from 'src/app/services/app.notification';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AppUtils } from 'src/app/services/app.utils';
import { AppComponent } from 'src/app/app.component';

@Component({
  selector: 'app-point',
  templateUrl: './point.component.html',
  styleUrls: ['./point.component.css'],
})
export class PointComponent implements OnInit, OnDestroy {
  user: any;
  loading = false;

  dataGridSkip = 0;
  dataGridPageSize = 10;

  dataGrid = [];
  dataGridSelectableSettings: SelectableSettings;
  dataGridGridDataResult: GridDataResult;
  dataGridSort = {
    allowUnsort: true,
    mode: 'multiple'
  };
  dataGridSortByField: SortDescriptor[] = [
  ];
  public dataGridState: State = {
    skip: this.dataGridSkip,
    take: this.dataGridSkip + this.dataGridPageSize,
    filter: {
      logic: 'and',
      filters: [{ field: 'Name', operator: 'contains', value: '' }]
    },
  };
  dataDetailItem: any;
  dataDetailItemtemp: any;
  DialogDetail = false;

  searchOption = {
    LuatUUID: null,
    ChuongUUID: null,
    MucUUID: null,
    DieuUUID : null,
    KhoanUUID: null,
  }

  isDetail: boolean = true;

  Law: Array<{ Content: any; ID: number; Name: string; LuatUUID: string; }> = [];
  LawFilter: Array<{ ID: number; Content: string }> = [];

  Chapter: Array<{ ID: number; Content: string, LuatUUID: number, ChuongUUID: number }> = [];
  ChapterFilter: Array<{ ID: number; Content: string, LuatUUID: number }> = [];

  ChapterItem: Array<{ ID: number; Content: string, LuatUUID: number, ChuongUUID: number, MucUUID: number }> = [];
  ChapterItemFilter: Array<{ ID: number; Content: string, LuatUUID: number, ChuongUUID: number }> = [];

  Artical: Array<{ ID: number; Content: string, LuatUUID: number, ChuongUUID: number , MucUUID : number, DieuUUID: number }> = [];
  ArticalFilter: Array<{ ID: number; Content: string, LuatUUID: number, ChuongUUID: number , MucUUID : number }> = [];

  Claust: Array<{ ID: number; Content: string, LuatUUID: number, ChuongUUID: number , MucUUID : number, DieuUUID: number, KhoanUUID: number }> = [];
  ClaustFilter: Array<{ ID: number; Content: string, LuatUUID: number, ChuongUUID: number , MucUUID : number, DieuUUID: number }> = [];


  constructor(
    private appService: AppService,
    private appComponent: AppComponent,
    private appSwal: AppSwal,
    public intl: IntlService,
    private notification: Notification,
    private authenticationService: AuthenticationService,
    private appUtils: AppUtils,
  ) {
    this.authenticationService.getUser();
    this.user = this.authenticationService.user;

  }

  ngOnDestroy(): void {

  }

  async ngOnInit() {
    this.setDefault();
    this.setSelectableSettings();
    await this.getFilter();
    this.onReload();
  }


  setSelectableSettings(): void {
    this.dataGridSelectableSettings = {
      checkboxOnly: false,
      mode: 'single'
    };
  }


  async getFilter() {
    const result = await this.appService.doGET('api/Law/GetLaw', null);
    if (result) {
      this.Law = result.Data;
      this.LawFilter = this.Law.slice();
    }

    const chapterResult = await this.appService.doGET('api/Chapter/GetChapter', null);
    if (chapterResult) {
      this.Chapter = chapterResult.Data;
      this.ChapterFilter = this.Chapter.slice();
    }
    const chapterItemResult = await this.appService.doGET('api/ChapterItem/GetChapterItem', null);
    if (chapterItemResult) {
      this.ChapterItem = chapterItemResult.Data;
      this.ChapterItemFilter = this.ChapterItem.slice();
    }
    const ArticalResult = await this.appService.doGET('api/Artical/GetArtical', null);
    if (ArticalResult) {
      this.Artical = ArticalResult.Data;
      this.ArticalFilter = this.Artical.slice();
    }
    const ClaustResult = await this.appService.doGET('api/Claust/GetClaust', null);
    if (ClaustResult) {
      this.Claust = ClaustResult.Data;
      this.ClaustFilter = this.Claust.slice();
    }
  }

  async getDataGrid() {
    this.loading = true;
    const dataRequest = {
      LuatUUID: this.searchOption.LuatUUID ? this.searchOption.LuatUUID : '',
      ChuongUUID: this.searchOption.ChuongUUID ? this.searchOption.ChuongUUID : '',
      MucUUID : this.searchOption.MucUUID ? this.searchOption.MucUUID : '',
      DieuUUID: this.searchOption.DieuUUID ? this.searchOption.DieuUUID : '',
      KhoanUUID : this.searchOption.KhoanUUID ? this.searchOption.KhoanUUID : '',
      PointID : '',
    }
    const result = await this.appService.doGET('api/Point/Search', dataRequest);
    if (result) {
      this.dataGrid = [];

      result.Data.forEach((item: { Luat: any; Chuong: any; Muc: any; Dieu: any; Khoan: any; Diem: any; dataLuat: any; }) => {
        const dataLuat = item.Luat;
        const dataChuong = item.Chuong;
        const dataMuc = item.Muc;
        const dataDieu = item.Dieu;
        const dataKhoan = item.Khoan;
        const dataDiem = item.Diem;

        const combinedData = {
          ...this.appComponent.mapDataWithDefault(dataLuat),
          ...this.appComponent.mapDataWithDefault(dataChuong),
          ...this.appComponent.mapDataWithDefault(dataMuc),
          ...this.appComponent.mapDataWithDefault(dataDieu),
          ...this.appComponent.mapDataWithDefault(dataKhoan),
          ...this.appComponent.mapDataWithDefault(dataDiem),
          LuatContent: dataLuat.Content,
          ChuongContent: dataChuong.Content,
          MucContent: dataMuc.Content,
          DieuContent: dataDieu.Content,
          KhoanContent: dataKhoan.Content,
        };
        this.dataGrid.push(combinedData);
      });
      this.bindDataGrid();
    }
    this.loading = false;
  }

  setDefault() {
    this.dataDetailItem = {
      ID: null,
      Content: null,
      Title: null,
      LuatUUID: null,
      ChuongUUID: null,
      MucUUID : null,
      DieuUUID: null,
      KhoanUUID: null,
    };
    this.searchOption = {
      LuatUUID: '',
      ChuongUUID: '',
      MucUUID: '',
      DieuUUID: '',
      KhoanUUID: '',
    };
    this.ChapterFilter = this.Chapter.filter((obj) => { return obj.LuatUUID == this.searchOption.LuatUUID })
    this.ChapterItemFilter = this.ChapterItem.filter((obj) => { return obj.ChuongUUID == this.searchOption.ChuongUUID })
    this.ArticalFilter = this.Artical.filter((obj) => { return obj.MucUUID == this.searchOption.MucUUID })
    this.ClaustFilter = this.Claust.filter((obj) => { return obj.DieuUUID == this.searchOption.DieuUUID })

    this.dataDetailItemtemp = Object.assign({}, this.dataDetailItem);
  }

  onEdit() {
    this.isDetail = false;
  }

  onCancelEditing() {
    this.isDetail = !this.isDetail;
  }

  bindtemTemp(item) {
    this.dataDetailItemtemp = Object.assign({}, item);
    this.dataDetailItemtemp.LawDate = this.dataDetailItemtemp.LawDate ? new Date(this.dataDetailItemtemp.LawDate) : null;

    const lawItem = this.Law.find(l => l.LuatUUID === this.dataDetailItemtemp.LuatUUID);
    this.dataDetailItemtemp.LawName = lawItem ? lawItem.Content : '(Trống)';

    const chapter = this.Chapter.find(l => l.ChuongUUID === this.dataDetailItemtemp.ChuongUUID);
    this.dataDetailItemtemp.ChuongContent = chapter ? chapter.Content : '(Trống)';

    const chapterItem = this.ChapterItem.find(l => l.MucUUID === this.dataDetailItemtemp.MucUUID);
    this.dataDetailItemtemp.MucContent = chapterItem ? chapterItem.Content : '(Trống)';

    const artical = this.Artical.find(l => l.DieuUUID === this.dataDetailItemtemp.DieuUUID);
    this.dataDetailItemtemp.DieuContent = artical ? artical.Content : '(Trống)';

    const claust = this.Claust.find(l => l.KhoanUUID === this.dataDetailItemtemp.KhoanUUID);
    this.dataDetailItemtemp.KhoanContent = claust ? claust.Content : '(Trống)';
  }

  onUserPageChange(event: PageChangeEvent) {
    this.dataGridSkip = event.skip;
    this.bindDataGrid();
  }


  bindDataGrid() {
    this.dataGridGridDataResult = {
      data: orderBy(this.dataGrid, this.dataGridSortByField),
      total: this.dataGrid.length
    };

    this.dataGridGridDataResult = process(this.dataGrid, this.dataGridState);
  }

  onUserSortChange(sort: SortDescriptor[]): void {
    this.dataGridSortByField = sort;
    this.bindDataGrid();
  }

  public onGridDataStateChange(state: DataStateChangeEvent): void {
    this.dataGridState = state;
    this.dataGridGridDataResult = process(this.dataGrid, this.dataGridState);
  }

  onReload() {
    this.getDataGrid();
  }




  createDataRequest() {
    const temp = this.dataDetailItemtemp;
    return {
      ID: temp.ID,
      Name: temp.Name,
      Title: temp.Title,
      LawID: temp.LawID,
      ChapterID : temp.ChapterID ,
    };
  }


  async onAddItem() {
    const dataRequest = this.createDataRequest();
    const result = await this.appService.doPOST("api/Point/Post", dataRequest);
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onReload();
      this.DialogDetail = false;
    } else {
      if (!result.Msg) {
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
  }

  async onUpdateItem() {
    const iD = this.dataDetailItem.ID;
    const dataRequest = this.createDataRequest();

    const result = await this.appService.doPUT("api/Point/Put", dataRequest, { iD });
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      this.onReload();
      this.DialogDetail = false;
      this.isDetail = true;
    } else {
      if (!result.Msg) {
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
  }

  async openUpdateDialog(ID) {
    this.DialogDetail = true;
    const dataRequest = {
      iD: ID
    };

    const result = await this.appService.doGET("api/Point/GetDetail", dataRequest);
    if (result) {
      this.dataDetailItem = result.Data;
      this.bindtemTemp(this.dataDetailItem);
    }
  }

  async onDeleteItem(ID) {
    const dataRequest = {
      iD: ID
    };
    const option = await this.appSwal.showWarning("Bạn chắc chắn không ?", true);
    if (option) {
      const result = await this.appService.doDELETE('api/Point/Delete', dataRequest);
      if (result && result.Status === 1) {
        this.notification.showSuccess(result.Msg);
        this.onReload();
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
  }

  onCloseDialog() {
    this.isDetail = true;
    this.DialogDetail = false;
  }


  openAddDialog() {
    this.DialogDetail = true;
    this.setDefault();
  }

  LawHandleFilter(value) {
    this.LawFilter = this.Law.filter((s) => s.Content.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  ChapterHandleFilter(value) {
    this.ChapterFilter = this.Chapter.filter((s) => s.Content.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  ChapterItemHandleFilter(value) {
    this.ChapterItemFilter = this.ChapterItem.filter((s) => s.Content.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  ArticalHandleFilter(value) {
    this.ArticalFilter = this.Artical.filter((s) => s.Content.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  ClaustHandleFilter(value) {
    this.ClaustFilter = this.Claust.filter((s) => s.Content.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  onSearchChange() {
    if (this.searchOption.LuatUUID == null) {
      this.searchOption.ChuongUUID = null;
      this.searchOption.MucUUID = null;
      this.searchOption.DieuUUID = null;
      this.searchOption.KhoanUUID = null;
      this.ChapterFilter = null;
      this.ChapterItemFilter = null;
      this.ArticalFilter = null;
      this.ClaustFilter = null;
    }
    else {
      this.ChapterFilter = this.Chapter.filter((obj) => { return obj.LuatUUID == this.searchOption.LuatUUID })
    }

    if (this.searchOption.ChuongUUID == null) {
      this.searchOption.MucUUID = null;
      this.searchOption.DieuUUID = null;
      this.searchOption.KhoanUUID = null;
      this.ChapterItemFilter = null
      this.ArticalFilter = null;
      this.ClaustFilter = null;
    }
    else {
      this.ChapterItemFilter = this.ChapterItem.filter((obj) => { return obj.ChuongUUID == this.searchOption.ChuongUUID })
    }
    if (this.searchOption.MucUUID == null) {
      this.searchOption.DieuUUID = null;
      this.searchOption.KhoanUUID = null;
      this.ArticalFilter = null;
      this.ClaustFilter = null;
    }
    else {
      this.ArticalFilter = this.Artical.filter((obj) => { return obj.MucUUID == this.searchOption.MucUUID })
    }
    if (this.searchOption.DieuUUID == null) {
      this.searchOption.KhoanUUID = null;
      this.ClaustFilter = null;
    }
    else {
      this.ClaustFilter = this.Claust.filter((obj) => { return obj.DieuUUID == this.searchOption.DieuUUID})
    }
    if (this.searchOption.KhoanUUID == null) {
     this.ClaustFilter = null; 
    }


    this.onReload();
  }

  onLawDetailChange(){
    this.ChapterFilter = this.Chapter.filter( (obj) => { return obj.LuatUUID == this.dataDetailItemtemp.LuatUUID })
  }
}
