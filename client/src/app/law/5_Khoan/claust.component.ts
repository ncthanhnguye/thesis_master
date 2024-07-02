import { Component, OnInit, OnDestroy } from '@angular/core';
import { AppService } from 'src/app/services/app.service';
import { SelectableSettings, PageChangeEvent, GridDataResult, DataStateChangeEvent } from '@progress/kendo-angular-grid';
import { State, process, SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { AppSwal } from 'src/app/services/app.swal';
import { IntlService } from '@progress/kendo-angular-intl';
import { Notification } from 'src/app/services/app.notification';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AppUtils } from 'src/app/services/app.utils';

@Component({
  selector: 'app-claust',
  templateUrl: './claust.component.html',
  styleUrls: ['./claust.component.css'],
})
export class ClaustComponent implements OnInit, OnDestroy {
  user: any;
  loading = false;

  dataGridSkip = 0;
  dataGridPageSize = 50;

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
    LawID: null,
    ChapterID: null,
    ChapterItemID: null , 
    ArticalID : null , 
    ClaustID : null
  }

  Law: Array<{ ID: number; Name: string; }> = [];
  LawFilter: Array<{ ID: number; Name: string }> = [];

  Chapter: Array<{ ID: number; Name: string, LawID: number }> = [];
  ChapterFilter: Array<{ ID: number; Name: string, LawID: number }> = [];

  ChapterItem: Array<{ ID: number; Name: string, LawID: number, ChapterID: number }> = [];
  ChapterItemFilter: Array<{ ID: number; Name: string, LawID: number, ChapterID: number }> = [];

  Artical: Array<{ ID: number; Name: string, LawID: number, ChapterID: number , ChapterItemID : number }> = [];
  ArticalFilter: Array<{ ID: number; Name: string, LawID: number, ChapterID: number , ChapterItemID : number }> = [];


  constructor(
    private appService: AppService,
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
    this.setSelectableSettings();
    await this.getFilter();
    this.setDefault();
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
  }

  async getDataGrid() {
    this.loading = true;
    const dataRequest = {
      LawID: this.searchOption.LawID ? this.searchOption.LawID : '',
      ChapterID: this.searchOption.ChapterID ? this.searchOption.ChapterID : '',
      ChapterItemID : this.searchOption.ChapterItemID ? this.searchOption.ChapterItemID : '',
      ArticalID: this.searchOption.ArticalID ? this.searchOption.ArticalID : '',
      ClaustID : this.searchOption.ClaustID ? this.searchOption.ClaustID : '' , 
      PointID : ''
    }
    const result = await this.appService.doGET('api/Claust/Search', dataRequest);
    if (result) {
      this.dataGrid = result.Data;
      this.bindDataGrid();
    }
    this.loading = false;
  }

  setDefault() {
    this.dataDetailItem = {
      ID: null,
      Name: null,
      Title: null,
      LawID: null,
      ChapterID: null,
      AritcalID : null
    };
    this.searchOption.LawID = 2;
    this.ChapterFilter = this.Chapter.filter((obj) => { return obj.LawID == this.searchOption.LawID })
    this.ChapterItemFilter = this.ChapterItem.filter((obj) => { return obj.ChapterID == this.searchOption.ChapterID })
    this.ArticalFilter = this.Artical.filter((obj) => { return obj.ChapterID == this.searchOption.ChapterItemID })

    this.dataDetailItemtemp = Object.assign({}, this.dataDetailItem);
  }

  bindtemTemp(item) {
    this.dataDetailItemtemp = Object.assign({}, item);
    this.dataDetailItemtemp.LawDate = this.dataDetailItemtemp.LawDate ? new Date(this.dataDetailItemtemp.LawDate) : null;
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
    const result = await this.appService.doPOST("api/Claust/Post", dataRequest);
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

    const result = await this.appService.doPUT("api/Claust/Put", dataRequest, { iD });
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

  async openUpdateDialog(ID) {
    this.DialogDetail = true;
    const dataRequest = {
      iD: ID
    };

    const result = await this.appService.doGET("api/Claust/GetDetail", dataRequest);
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
      const result = await this.appService.doDELETE('api/Claust/Delete', dataRequest);
      if (result && result.Status === 1) {
        this.notification.showSuccess(result.Msg);
        this.onReload();
      } else {
        this.appSwal.showWarning(result.Msg, false);
      }
    }
  }

  onCloseDialog() {
    this.DialogDetail = false;
  }


  openAddDialog() {
    this.DialogDetail = true;
    this.setDefault();
  }

  LawHandleFilter(value) {
    this.LawFilter = this.Law.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  ChapterHandleFilter(value) {
    this.ChapterFilter = this.Chapter.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  ChapterItemHandleFilter(value) {
    this.ChapterItemFilter = this.ChapterItem.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  onSearchChange() {
    if (this.searchOption.LawID == null) {
      this.searchOption.ChapterID = null;
      this.searchOption.ChapterItemID = null;
      this.searchOption.ArticalID = null;
      this.ChapterFilter = null;
      this.ChapterItemFilter = null;
      this.ArticalFilter = null;
    }
    else {
      this.ChapterFilter = this.Chapter.filter((obj) => { return obj.LawID == this.searchOption.LawID })
    }

    if (this.searchOption.ChapterID == null) {
      this.searchOption.ChapterItemID = null;
      this.searchOption.ArticalID = null;
      this.ChapterItemFilter = null
      this.ArticalFilter = null;
    }
    else {
      this.ChapterItemFilter = this.ChapterItem.filter((obj) => { return obj.ChapterID == this.searchOption.ChapterID })
    }
    if (this.searchOption.ChapterItemID == null) {
      this.searchOption.ArticalID = null;
      this.ArticalFilter = null;
    }
    else {
      this.ArticalFilter = this.Artical.filter((obj) => { return obj.ChapterItemID == this.searchOption.ChapterItemID })
    }

    
    this.onReload();
  }

  onLawDetailChange(){
    this.ChapterFilter = this.Chapter.filter( (obj) => { return obj.LawID == this.dataDetailItemtemp.LawID })
  }
}
