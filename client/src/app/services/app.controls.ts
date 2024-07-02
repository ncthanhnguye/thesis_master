import { Injectable } from '@angular/core';
import { Location } from '@angular/common';
import { AppService } from './app.service';
import { AppSwal } from './app.swal';
import { Router } from '@angular/router';
import { AppConsts } from 'src/app/services/app.consts';
@Injectable({
    providedIn: 'root'
})
export class AppControls {

    constructor(
        public location: Location,
        private appService: AppService,
        private appSwal : AppSwal,
        private router: Router,
        public appConsts: AppConsts
    ) {

     }

    async getControls(roleID, pageUrl) {
        if (!pageUrl) {
            return;
        }

        const dataRequest = {
            PageUrl: pageUrl,
            RoleID: roleID,
            // Controls: controlDefault
        };
        const result = await this.appService.doPOST('api/Control/Check', dataRequest);
        const control = {};
        if (result && result.Status == 1 && result.Data && result.Data.length > 0) {
            result.Data.forEach(item => {
                control[item.ID] = item.ActiveFlg;
            });
        }

        return control;
    }

    async getPageName() {
        let pageUrl = this.location.path();
        if (!pageUrl) {
            return null;
        }

        const idx = pageUrl.indexOf('?');
        if (idx >= 0) {
            pageUrl = pageUrl.substring(0, idx);
        }

        pageUrl = pageUrl.substring(1);

        const dataRequest = {
            id: pageUrl,
            langID : localStorage.getItem('currentLanguage') ? localStorage.getItem('currentLanguage') : this.appConsts.defaultLangID
        };
        const result = await this.appService.doGET('api/Page/GetName', dataRequest);
        if (result && result.Status === 1) {
            return result.Data;
        }
        return '';
    }

    async getPageNamePortal() {
        let pageUrl = this.location.path();
        if (!pageUrl) {
            return null;
        }

        const idx = pageUrl.indexOf('?');
        if (idx >= 0) {
            pageUrl = pageUrl.substring(0, idx);
        }

        pageUrl = pageUrl.substring(1);

        const dataRequest = {
            id: pageUrl,
            langID : localStorage.getItem('portal') ? localStorage.getItem('portal') : this.appConsts.defaultLangID
        };
        const result = await this.appService.doGET('api/Page/GetName', dataRequest);
        if (result && result.Status === 1) {
            return result.Data;
        }
        return '';
    }
}