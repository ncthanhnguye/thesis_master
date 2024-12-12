import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';
import { AppService } from '../services/app.service';
import { AppComponent } from '../app.component';
import { AppSwal } from '../services/app.swal';
import { Notification } from '../services/app.notification';
import { AppControls } from 'src/app/services/app.controls';
import { TranslateService } from '@ngx-translate/core';
import { FileInfo } from '@progress/kendo-angular-upload';
import { AppFile } from 'src/app/services/app.file';
import { Observable } from 'rxjs';
import { Message, User } from '@progress/kendo-angular-conversational-ui';
import { AppConsts } from '../services/app.consts';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  //styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit, OnDestroy {
  @Output() searchChange = new EventEmitter<string>();
  @Output() reload = new EventEmitter<boolean>();
  @Output() openMenu = new EventEmitter<boolean>();

  user: any;

  loading = false;
  showToggleUser = false;
  uploadImageOpened = false;
  isShowHeader = true;

  public GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
  opened = false;
  myInterval: any;
  filesUpload: Array<FileInfo>;
  searchText: any;
  feed: Observable<Message[]>;

  //end of chat

  control: any;
  controlDefault = true;
  public ManagePersonalOpened = false;
  public request_AccountID;
  isViewedInfor = false;
  unit: Array<{ UnitName: string, UnitID: string }>;
  unitUsedName = '';
  public userTemp: any = {};


  constructor(
    private translate: TranslateService,
    private router: Router,
    private authenticationService: AuthenticationService,
    public appControls: AppControls,
  ) {
    this.authenticationService.getUser();
    this.user = this.authenticationService.user;
    

  }

  option: Array<any>;


  ngOnInit(): void {
    if (localStorage.getItem('ThesisLocalStorage')) {
      this.authenticationService.getUser();
      this.user = this.authenticationService.user;
      this.option = [
        {
          id: 2,
          text: "Đăng xuất",
          icon: 'logout',
          click: () => {
            this.onLogOut();
          }
        }
      ];
    }
    else {
      this.isShowHeader = false
      // this.option = [
      //   {
      //     id: 1,
      //     text: "Đăng nhập",
      //     icon: 'login',
      //     click: () => {
      //       this.onLogin();
      //     }
      //   }
      // ];
    }
  }


  ngOnDestroy(): void {
    
  }


  onLogin() {
    this.router.navigate([AppConsts.page.login]);
  }
  onLogOut() {
    this.authenticationService.doSignout();
  }

  onUploadImageClose() {
    this.uploadImageOpened = false;
    this.authenticationService.getUser();
    this.user = this.authenticationService.user;
  }

  onUploadImage() {
    this.showToggleUser = false;
    this.uploadImageOpened = true;
  }

  emitReload() {
    this.reload.emit(true);
  }
  setDefault() {
  }

  onSearchKeyPress(e: any) {
    if (e.keyCode === 13) {
      this.onSearch();
    }
  }

  onSearch() {
    if (!this.searchText) {
      return;
    }
  }

  onOpenMenu() {
    // this.openMenu.emit();
  }


  onImgError(event) {
    event.target.src = AppConsts.DEFAULT_IMAGE;
  }
}
