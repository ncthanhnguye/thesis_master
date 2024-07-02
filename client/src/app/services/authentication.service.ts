import { AppService } from '../services/app.service';
import { Injectable, OnInit } from '@angular/core';
import { Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import { AppLanguage } from './app.language';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { AppConsts } from './app.consts';


@Injectable({ providedIn: 'root' })
export class AuthenticationService implements OnInit {

    public redirectUrl: string;
    public user: any = {};
    private headers: Headers;
    private options: RequestOptions;
    private tokenEndpoint: string;
    private clientId: string;    
    public GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
    ngOnInit(): void {       
    }

    constructor(
        private http: Http,
        private appService: AppService,
        private language: AppLanguage,
        private location: Location,
        private router: Router ,
        public appConsts: AppConsts
    ) {
        this.decodeToken();
        this.tokenEndpoint = `${this.appService.apiRoot}/Token`;
        this.clientId = null;
    }


    getUser(): any {
        const result = localStorage.getItem('ThesisLocalStorage');
        if (result) {
            this.user = JSON.parse(result);
        }
        return this.user;
    }

    public async doSignIn(userName: string, password: string, type: number = 0) {

        this.headers = new Headers({ 'Content-Type': 'application/x-www-form-urlencoded' });
        this.headers.set('Accept-Language', this.language.get());
        this.options = new RequestOptions({ headers: this.headers });
        
        const params: any = {
            type: type,
            client_id: this.clientId,
            grant_type: 'password',
            username: userName,
            password
        };

        const body: string = this.encodeParams(params);

        const result = await this.appService.doPOSTOPTION('Token', body, this.options);
        
        if (result.access_token) {
            this.store(result);           
            return {
                Status: 1,
                Data: result                
            };
        } else {
            return {
                Status: 0,
                Msg: result.error_description
            };
        }

    }
    
    async doSignout(): Promise<void> {
        const dataRequest = {};
        this.redirectUrl = null;
        const result = await this.appService.doPOST('api/User/Logout', dataRequest);
        if (result && result.Status == 1){
            localStorage.removeItem('ThesisLocalStorage');
            this.user = {};
            this.revokeToken();
            this.router.navigate([`/${AppConsts.page.search}`])
            .then(() => {
                window.location.reload();
            });
        }

    }

    async doSignOutWithoutReload(): Promise<void> {
        this.redirectUrl = null;
        const dataRequest = {};
        const result = await this.appService.doPOST('api/User/Logout', dataRequest);
        if (result && result.Status == 1){   
            localStorage.removeItem('ThesisLocalStorage');
            this.user = {};
            this.revokeToken();
        }
    }


    public getNewToken(): void {

        const refreshToken: string = localStorage.getItem('refresh_token');

        if (refreshToken) {
            const params: any = {
                client_id: this.clientId,
                grant_type: 'refresh_token',
                refresh_token: refreshToken
            };

            const body: string = this.encodeParams(params);

            this.http.post(this.tokenEndpoint, body, this.options)
                .subscribe(
                    (res: Response) => {

                        const loginInfo: any = res.json();

                        if (typeof loginInfo.access_token !== 'undefined') {
                            this.store(loginInfo);
                        }
                    });
        }
    }

    public revokeToken(): void {

        const token: string = localStorage.getItem('token');
        if (token != null) {
            localStorage.removeItem('token');
        }
    }

    public revokeRefreshToken(): void {

        const refreshToken: string = localStorage.getItem('refresh_token');

        if (refreshToken) {
            const params: any = {
                client_id: this.clientId,
                token_type_hint: 'refresh_token',
                token: refreshToken
            };

            const body: string = this.encodeParams(params);

            this.http.post(this.tokenEndpoint, body, this.options)
                .subscribe(
                    () => {
                        localStorage.removeItem('refresh_token');
                    });
        }
    }

    encodeParams(params: any): string {

        let body = '';
        // tslint:disable-next-line:prefer-const
        // tslint:disable-next-line:forin
        for (const param in params) {
            if (body.length) {
                body += '&';
            }
            body = `${body}${param}=`;
            body = `${body}${encodeURIComponent(params[param])}`;
        }

        return body;
    }

    store(body: any): void {
        // Stores access token in local storage to keep user signed in.
        localStorage.setItem('token', body.access_token);
        // Stores refresh token in local storage.
        localStorage.setItem('refresh_token', body.refresh_token);
        this.setUser(body);
        // Decodes the token.
        this.decodeToken();

    }

    setUser(data: any) {
        if (!data.FilePath ||
            (data.FilePath.indexOf('.png') < 0
            && data.FilePath.indexOf('.jpg') < 0
            && data.FilePath.indexOf('.jpeg') < 0
            && data.FilePath.indexOf('.gif') < 0
            && data.FilePath.indexOf('.bmp') < 0
        )) {
            data.FilePath = 'assets/images/userlogo.png';
        }
        localStorage.setItem('ThesisLocalStorage', JSON.stringify({
            UserName: data.UserName,
            FullName: data.FullName,
            RoleID: data.RoleID,
            RoleName: data.RoleName,
            FilePath: data.FilePath,
            AccountID: data.AccountID,
            UnitID: data.UnitID
        }));
        
        
        localStorage.setItem("currentLanguage",this.appConsts.defaultLangID);
    }

    postCrossDomainMessage() {    
        
        const iframe = document.createElement('IFRAME');
        iframe.id = 'admin-ifr';
        iframe.style.display = "none";
        (<HTMLIFrameElement>iframe).src = this.appService.portalRoot.slice(0, -1);
        document.body.appendChild(iframe);
        let postURL: any;
        let iframeId: any;
        postURL = this.appService.clientRoot.slice(0, -1);
        iframeId = 'admin-ifr';
        const iframe1 = document.getElementById(iframeId);
        if (iframe == null) { return; }
        const iWindow = (iframe as HTMLIFrameElement).contentWindow;
        const storageData = {
            ThesisLocalStorage: null,
            token: null
        }
        storageData.ThesisLocalStorage = localStorage.getItem('ThesisLocalStorage');
        storageData.token = localStorage.getItem('token');
        const domain  = this.appService.portalRoot;
        setTimeout(function () {
            // iWindow.postMessage(storageData, domain); // For Cross storageData to Portal 
        }, 1000);
    }

    decodeToken(): void {

    }
}
