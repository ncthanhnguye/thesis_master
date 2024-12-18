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
  selector: 'app-chapter',
  templateUrl: './chapter.component.html',
  styleUrls: ['./chapter.component.css'],
})
export class ChapterComponent implements OnInit, OnDestroy {
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
    LuatUUID: null
  }

  isDetail: boolean = true;

  Law: Array<{ Content: any; ID: number; Name: string; LuatUUID: string; }> = [];
  LawFilter: Array<{ ID: number; Name: string; LuatUUID: string }> = [];


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

  ngOnInit() {
    this.setDefault();
    this.setSelectableSettings();
    this.getFilter();
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
  }

  async getDataGrid() {
    this.loading = true;

    const dataRequest = {
      LuatUUID: this.searchOption.LuatUUID ? this.searchOption.LuatUUID : '',
      ChapterID: '',
    }
    const result = await this.appService.doGET('api/Chapter/Search', dataRequest);
    if (result) {
      this.dataGrid = [];

      result.Data.forEach(item => {
        const dataLuat = item.DATA_1_Luat;
        const dataChuong = item.DATA_2_Chuong;
        const combinedData = {
          ...dataLuat,
          ...dataChuong,
          LuatContent: dataLuat.Content
        };
        this.dataGrid.push(combinedData);
      });
      // console.log('dataGrid', this.dataGrid);
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

    };
    this.searchOption.LuatUUID = '';
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
    };
  }


  async onAddItem() {
    const dataRequest = this.createDataRequest();
    const result = await this.appService.doPOST("api/Chapter/Post", dataRequest);
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

    const result = await this.appService.doPUT("api/Chapter/Put", dataRequest, { iD });
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

    const result = await this.appService.doGET("api/Chapter/GetDetail", dataRequest);
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
      const result = await this.appService.doDELETE('api/Chapter/Delete', dataRequest);
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
    this.LawFilter = this.Law.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  onSearchChange() {
    this.onReload();
  }
}
