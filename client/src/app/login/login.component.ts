import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';
import { AppService } from '../services/app.service';
import { URLSearchParams } from '@angular/http';
import { AppComponent } from '../app.component';
import { AppLanguage } from '../services/app.language';
import { AppSwal } from '../services/app.swal';
import { Title } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';
import { AppConsts } from '../services/app.consts';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  // styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy {
  public type = 'password';

  loginInfo = {
    UserName: '',
    Password: '',
    Type: 0
  };
  public GUID_EMPTY = '00000000-0000-0000-0000-000000000000';

  languageName = '';

  public focus = {
    UserName : false ,
    Password : false
  };

  OTPCode = '';
  dataAcc: any;
  OTPFlag = false;

  languages: Array<any> = [{
    id: 0,
    culture: 'vi-VN',
    text: 'Tiếng Việt',
    click: (dataItem) => {
      this.onChangeLanguage(dataItem);
    }
  }, {
    id: 1,
    culture: 'en-US',
    text: 'English',
    click: (dataItem) => {
      this.onChangeLanguage(dataItem);
    }
  }];

  MsgError = '';
  initTranslate: any;
  redirectUrl = '';
  defaultPageWhenLogin = AppConsts.page.homePage;
  titlePostDetail = '';
  public changPassURL = '';

  // mobile
  public isMobile = false;
  // screen switch
  public screenName: string;
  public isShowCleanText = false;
  public isDisplayDecoration = false;
  
  public loginFailed = {
    isHide: true,
    message: null
  }

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService,
    private appService: AppService,
    private appComponent: AppComponent,
    private language: AppLanguage,
    private appSwal: AppSwal,
    private title: Title,
    private translate: TranslateService,
    private activatedRoute: ActivatedRoute
  ) {
    // this.appComponent.menuHidden = true;
    this.initTranslate = this.translate.get('init.translate').subscribe((translated: string) => {
      this.title.setTitle(this.translate.instant('Login'));
    });

    this.getLanguageName();

    this.onInit();

  }

  async onInit() {
    this.activatedRoute.queryParams.subscribe(async (params: any) => {
      if (params && params.redirectUrl) {
        this.redirectUrl = params.redirectUrl;
        if (params.title) {
          this.titlePostDetail = params.title;
        }
        if (localStorage.getItem('ThesisLocalStorage')) {
          window.location.reload();
          this.authenticationService.doSignOutWithoutReload();
        }
        this.changPassURL = this.appService.portalRoot +  'forgot-password' + '?redirectUrl=' + this.redirectUrl;

      } else {
        this.changPassURL = this.appService.portalRoot +  'forgot-password' ;
      }
      if (params && params.userName && params.password) {
        this.loginInfo.UserName = params.userName;
        this.loginInfo.Password = params.password;
        this.loginInfo.Type = params.type;
        await this.onLogin();
      }
    });
  }

  ngOnInit() {
    // Check device
    this.isMobile = this.isMobileDevice();
    // Event tracking device size
    window.addEventListener('resize', this.reportWindowSize);
    window.onresize = this.reportWindowSize;
    // Check length UserName to show Clear button
    this.checkUserNameLength();
    // if (localStorage.getItem('ThesisLocalStorage')) {
    //   this.authenticationService.doSignout();
    // }
    this.authenticationService.postCrossDomainMessage();
  }

  ngOnDestroy() {
    this.initTranslate.unsubscribe();
  }

  async onLogin() {
    const isValid = this.checkValidate()
    if (!isValid) {
      this.loginFailed.isHide = false;
      setTimeout(() => { this.loginFailed.isHide = true }, 1500)
      return
    }
    this.MsgError = '';
    this.appComponent.loading = true;
    const result = await this.authenticationService.doSignIn(this.loginInfo.UserName, this.loginInfo.Password, this.loginInfo.Type);

    if (result.Status === 1) {
      if (result.Data && this.redirectUrl) {
        if (this.titlePostDetail) {
          this.redirectUrl =  this.redirectUrl  + '?title=' + this.titlePostDetail + '&userName=' + result.Data.UserName + '&token=' + result.Data.access_token + '&webFlg=1';
        } else {
          this.redirectUrl =  this.redirectUrl  + '?userName=' + result.Data.UserName 
          // + '&token=' + result.Data.access_token 
          + '&webFlg=1';
        }
      }
      window.location.href = this.redirectUrl;
    } else {
      // this.appSwal.showWarning(result.Msg, false);
      this.loginFailed = {
        isHide: false,
        message: result.Msg
      }
      setTimeout(() => {
        this.loginFailed.isHide = true;
      }, 2000);
    }
    this.appComponent.loading = false;
  }
  checkValidate(): boolean {
    if (!this.loginInfo.UserName || this.loginInfo.UserName == '') {
      this.loginFailed.message = "Vui lòng nhập tài khoản"
      return false
    } else if (!this.loginInfo.Password || this.loginInfo.Password == '') {
      this.loginFailed.message = "Vui lòng nhập mật khẩu"
      return false
    }
    this.loginFailed.message = ""
    return true
  }

  onChangeLanguage(dataItem) {
    // this.appComponent.switchLanguage(dataItem.culture);
    this.getLanguageName();
  }

  getLanguageName() {
    let lang = this.language.get();
    if (!lang) {
      lang = 'vi-VN';
      // this.appComponent.switchLanguage(lang);
    }
    const language = this.languages.find(item => {
      return item.culture === lang;
    });

    if (language) { this.languageName = language.text; }
  }

  onLoginByMail() {
    window.location.href = `${this.appService.apiRoot}api/VNPTLogin/Login`;
  }

  onKeyPress(e: any) {
    this.isShowCleanText = true;
    if (e.keyCode === 13) {
      this.onLogin();
    }
  }
  onKeyUpUserName(event): void {
    this.checkUserNameLength()
  }
  checkUserNameLength(): void {
    if (this.loginInfo.UserName.length === 0) {
      this.isShowCleanText = false;
    } else {
      this.isShowCleanText = true;
    }
  }

  onShowPass() {
    if (this.type == 'password') {
      this.type = 'text';
    } else {
      this.type = 'password';
    }
  }

  onFocus(type) {
    if (type == 0) {
      this.focus.UserName = true;
    }
    if (type == 1) {
      this.focus.Password = true;
    }

  }
  onFocusOut(type) {
    if (type == 0) {
      this.focus.UserName = false;
    }
    if (type == 1) {
      this.focus.Password = false;
    }
  }

  isMobileDevice(): boolean {
    const screenWidth = window.screen.width;
    if (screenWidth <= 812) {
      this.screenName = 'waiting';
      if (window.screen.height > 540) {
        this.isDisplayDecoration = true;
      }
      return true;
    }
    this.screenName = 'login';
    this.isDisplayDecoration = false;
    return  false;
  }
  isWaitingEvent(value) {
    this.screenName = 'intro';
  }
  isIntroEvent(value) {
    this.screenName = 'login';
  }
  backToIntro() {
    this.screenName = 'intro';
  }
  cleanUserNameTxt() {
    this.loginInfo.UserName = '';
    this.isShowCleanText = false;
  }
  reportWindowSize() {
    if (window.innerWidth < 812) {
      this.isMobile = true;
      if (window.innerHeight > 540) {
        this.isDisplayDecoration = true;
      } else  {
        this.isDisplayDecoration = false;
      }
    } else {
      this.isMobile = false;
    }
  }
}
