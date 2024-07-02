import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { AppRoutingModule } from "./app-routing.module";
import { RouterModule } from "@angular/router";
import { DatePipe } from '@angular/common';

import { HttpClientModule, HTTP_INTERCEPTORS, HttpClient,} from "@angular/common/http";
import { HttpClientInMemoryWebApiModule } from "angular-in-memory-web-api";
import {  HttpModule,} from "@angular/http";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import {  MatPaginatorModule,  MatProgressBarModule,  MatSnackBarModule,  MatSortModule,  MatTableModule,  MatDatepickerModule,} from "@angular/material";
import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { AngularFontAwesomeModule } from "angular-font-awesome";
import { BsDropdownModule } from "ngx-bootstrap/dropdown";
import { FocusModule } from "angular2-focus";
import { ModalModule } from "ngx-bootstrap/modal";
import { AppRouter } from "./app.router";
import { TokenInterceptor } from "./services/token.interceptor";
import { AuthGuard } from "./services/auth.guard";
import { AuthService } from "./services/auth.service";
import { UserComponent } from "./ad/user/user.component";
import { RoleComponent } from "./ad/role/role.component";
import { AppComponent } from "./app.component";
import { LoginComponent } from "./login/login.component";
import { HomeComponent } from "./home/home.component";

import { DropDownsModule } from "@progress/kendo-angular-dropdowns";
import { DateInputModule,  DatePickerModule,} from "@progress/kendo-angular-dateinputs";
import { InputsModule } from "@progress/kendo-angular-inputs";
import { ButtonsModule } from "@progress/kendo-angular-buttons";
import { GridModule, ExcelModule, PDFModule } from "@progress/kendo-angular-grid";
import { LayoutModule } from "@progress/kendo-angular-layout";
import { ToolBarModule } from "@progress/kendo-angular-toolbar";
import { TooltipModule } from "@progress/kendo-angular-tooltip";
import { ExcelExportModule } from "@progress/kendo-angular-excel-export";
import { IntlModule } from "@progress/kendo-angular-intl";
import { NotificationModule } from "@progress/kendo-angular-notification";
import { UploadModule } from "@progress/kendo-angular-upload";
import { DialogModule, DialogsModule } from "@progress/kendo-angular-dialog";
import { PopupModule } from "@progress/kendo-angular-popup";
import { TreeViewModule } from "@progress/kendo-angular-treeview";
import { TreeListModule } from "@progress/kendo-angular-treelist";
import { ScrollViewModule } from '@progress/kendo-angular-scrollview';
import { ChartsModule } from "@progress/kendo-angular-charts";
import { UserRoleComponent } from "./ad/user-role/user-role.component";
import { PageComponent } from "./ad/page/page.component";
import { ControlComponent } from "./ad/control/control.component";
import { ConfigComponent } from "./ad/config/config.component";
import { QRCodeModule } from "angular2-qrcode";
import { OrganizationComponent } from "./profile/organization/organization.component";

import { LOCALE_ID } from "@angular/core";
import { DateInputsModule } from "@progress/kendo-angular-dateinputs";
import {  registerLocaleData,} from "@angular/common";
import localeVn from "@angular/common/locales/vi";

import "@progress/kendo-angular-intl/locales/vi/all";
import "hammerjs";
import { PermisionComponent } from "./permision/permision.component";
import { CommonComponent } from "./m/common/common.component";
import { MenuComponent } from "./menu/menu.component";

import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { PagerModule } from '@progress/kendo-angular-pager';
import { AccountsComponent } from './data/accounts/accounts.component';
import { AccountDetailComponent } from './data/account-detail/account-detail.component';
import { LawComponent } from "src/app/law/1_Luat/law.component";
import { ChapterComponent } from "./law/2_Chuong/chapter.component";
import { ChapterItemComponent } from "./law/3_Muc/chapter-item.component";
import { ArticalComponent } from "./law/4_Dieu/artical.component";
import { ClaustComponent } from "./law/5_Khoan/claust.component";
import { PointComponent } from "./law/6_Diem/point.component";
import { SearchComponent } from "./law/0_TimKiem/search.component";
import { DataCrawlComponent } from "./importExcel/data-crawl/data-crawl.component";
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HomeComponent,
    UserComponent,
    RoleComponent,
    UserRoleComponent,
    PageComponent,
    ControlComponent,
    ConfigComponent,
    PermisionComponent,
    CommonComponent,
    MenuComponent,
    AccountsComponent,
    AccountDetailComponent,
    OrganizationComponent,
    LawComponent , 
    ChapterComponent , 
    ChapterItemComponent , 
    ArticalComponent , 
    ClaustComponent ,
    PointComponent , 
    SearchComponent , 
    DataCrawlComponent

  ],
  imports: [
    BrowserModule,
    ButtonsModule,
    DropDownsModule,
    DateInputModule,
    DatePickerModule,
    InfiniteScrollModule,
    GridModule,
    ExcelModule,
    ExcelExportModule,
    LayoutModule,
    ToolBarModule,
    TooltipModule,
    QRCodeModule,
    UploadModule,
    DialogModule,
    TreeListModule,
    DialogsModule,
    PopupModule,
    TreeViewModule,
    TreeListModule,
    ScrollViewModule,
    ChartsModule,
    MatDatepickerModule,
    IntlModule,
    InputsModule,
    NotificationModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    HttpClientInMemoryWebApiModule,
    HttpModule,
    FormsModule,
    ReactiveFormsModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatSnackBarModule,
    MatSortModule,
    MatTableModule,
    AngularFontAwesomeModule,
    RouterModule.forRoot(AppRouter.routes, { useHash: true }),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
    }),
    BsDropdownModule.forRoot(),
    FocusModule.forRoot(),
    ModalModule.forRoot(),
    BrowserAnimationsModule,
    IntlModule,
    DateInputsModule,
    PagerModule,
    PDFModule
  ],
  exports: [BsDropdownModule, TooltipModule, ModalModule],
  providers: [
    AuthGuard,
    AuthService,
    DatePipe,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true,
    },
    { provide: LOCALE_ID, useValue: "vi-VN" },
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}
