import { TranslateService } from '@ngx-translate/core';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class AppLanguage {
    constructor(
        public translate: TranslateService
    ) { }

    set(lang: any) {
        this.translate.use(lang);
        localStorage.setItem('culture', lang);
    }

    default() {
        let lang = localStorage.getItem('culture');
        if (!lang) { lang = 'vi-VN'; }
        this.translate.setDefaultLang(lang);
    }

    get() {
        return localStorage.getItem('culture');
    }


    setPortal(lang: any) {
        this.translate.use(lang);
        localStorage.setItem('portal', lang);
    }

    defaultPortal() {
        let lang = localStorage.getItem('portal');
        if (!lang) { lang = 'vi-VN'; }
        this.translate.setDefaultLang(lang);
    }

    getPortal() {
        return localStorage.getItem('portal');
    }
}
