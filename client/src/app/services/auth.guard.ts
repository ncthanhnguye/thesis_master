import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './auth.service';
import { Location } from '@angular/common';
import { AppService } from './app.service';
import { AuthenticationService } from './authentication.service';
import { AppSwal } from './app.swal';
import { TranslateService } from '@ngx-translate/core';
import { AppConsts } from './app.consts';
/**
 * Decides if a route can be activated.
 */
@Injectable() export class AuthGuard implements CanActivate {

    constructor(
        public authService: AuthService,
        public router: Router,
        public appService: AppService,
        public authenticationService: AuthenticationService,
        public appSwal: AppSwal,
        public location: Location
    ) { }

    public async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {

        await this.getTokenByUrl(route);

        if (this.authService.isAuthenticated()) {
            const result = await this.checkRole(route.routeConfig.path);
            return result;
        }

        this.router.navigate([AppConsts.page.login]);
        return false;
    }

    async checkRole(url: string) {
        this.authenticationService.getUser();
        const user = this.authenticationService.user;
        const roleID = user.RoleID;

        const result = await this.appService.doGET('api/Page/Check', { roleID, url });
        if (result) {
            if (result.Status === 1) {
                return true;
            } else {
                await this.appSwal.showError(result.Msg);
            }
        }
        return false;
    }

    async getTokenByUrl(route: any) {
        var token = route.queryParams['token'];
        var userName =  route.queryParams['userName'];

        if (token && userName) {
          localStorage.setItem('token', token);
          localStorage.setItem('mbWebviewFlg', '1');
          
          var dataRequest = {
            userName
          };
      
          const result = await this.appService.doGET('api/User/GetUserLoginInfo', dataRequest);
          if (result && result.Status === 1) {
            result.Data['access_token']= token;
            this.authenticationService.store(result.Data);

            // thông tin cá nhân, mobile
            var accountID =  route.queryParams['accountID'];
            var params = {};
            if (accountID) {
                params = { queryParams: { accountID } };
            }

            this.router.navigate([route.routeConfig.path], params);
          }
        }
    }

}