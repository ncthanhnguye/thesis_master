import { LoginComponent } from "./login/login.component";
import { HomeComponent } from "./home/home.component";
import { Routes } from "@angular/router";
import { AuthGuard } from "./services/auth.guard";
import { UserComponent } from "./ad/user/user.component";
import { RoleComponent } from "./ad/role/role.component";
import { UserRoleComponent } from "./ad/user-role/user-role.component";
import { PermisionComponent } from "./permision/permision.component";
import { PageComponent } from "./ad/page/page.component";
import { ControlComponent } from "./ad/control/control.component";
import { ConfigComponent } from "./ad/config/config.component";
import { CommonComponent } from "./m/common/common.component";
import { AppConsts } from "./services/app.consts";
import { AccountsComponent } from "./data/accounts/accounts.component";
import { AccountDetailComponent } from "./data/account-detail/account-detail.component";
import { OrganizationComponent } from "./profile/organization/organization.component";
import { LawComponent } from "src/app/law/1_Luat/law.component";
import { ChapterComponent } from "./law/2_Chuong/chapter.component";
import { ChapterItemComponent } from "./law/3_Muc/chapter-item.component";
import { ArticalComponent } from "./law/4_Dieu/artical.component";
import { ClaustComponent } from "./law/5_Khoan/claust.component";
import { PointComponent } from "./law/6_Diem/point.component";
import { SearchComponent } from "./law/0_TimKiem/search.component";
import { DataCrawlComponent } from "./importExcel/data-crawl/data-crawl.component";

export class AppRouter {
  public static routes: Routes = [
    
    { path: AppConsts.page.login, component: LoginComponent },
    { path: AppConsts.page.permision, component: PermisionComponent },

    {
      path: AppConsts.page.user,
      component: UserComponent,
      canActivate: [AuthGuard],
    },
    {
      path: AppConsts.page.role,
      component: RoleComponent,
      canActivate: [AuthGuard],
    },
    {
      path: AppConsts.page.userRole,
      component: UserRoleComponent,
      canActivate: [AuthGuard],
    },
    {
      path: AppConsts.page.page,
      component: PageComponent,
      canActivate: [AuthGuard],
    },
    {
      path: AppConsts.page.control,
      component: ControlComponent,
      canActivate: [AuthGuard],
    },
    {
      path: AppConsts.page.config,
      component: ConfigComponent,
      canActivate: [AuthGuard],
    },
    {
      path: AppConsts.page.common,
      component: CommonComponent,
      canActivate: [AuthGuard],
    },
    {
      path: AppConsts.page.accounts,
      component: AccountsComponent,
      canActivate: [AuthGuard],
    },
    {
      path: AppConsts.page.accountDetail,
      component: AccountDetailComponent,
      canActivate: [AuthGuard],
    },
    {
      path: AppConsts.page.pro5Organization,   component: OrganizationComponent,   canActivate: [AuthGuard],  },

    { path: 'law', component: LawComponent },
    { path: 'chapter', component: ChapterComponent },
    { path: 'chapterItem', component: ChapterItemComponent },
    { path: 'artical', component: ArticalComponent },
    { path: 'claust', component: ClaustComponent },
    { path: 'point', component: PointComponent },
    { path: 'search', component: SearchComponent },
    { path: 'data-crawl', component: DataCrawlComponent },
    {
      path: '**', redirectTo: AppConsts.page.search
    },


  ];
}
