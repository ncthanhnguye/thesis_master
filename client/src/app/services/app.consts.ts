import { Injectable } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class AppConsts {
  public pageSkip = 0;
  public pageSize = 10;
  public defaultLangID = 'vi-VN';
  public static readonly DEFAULT_IMAGE = '/assets/images/default-image.png';
  public static readonly NOTIFY_TYPE_CONFIRM = 1;
  public static readonly NOTIFY_TYPE_INFO = 2;
  public static readonly NOTIFY_INFO_MODE_WARNING = 1;
  public static readonly NOTIFY_INFO_MODE_SUCCESS = 2;
  public static readonly NOTIFY_INFO_MODE_ERROR = 3;

  public static page = {
    login: "login",
    homePage: "home",
    search: "search",
    permision: "permision",
    changePassword: "change-password",
    user: "ad/user",
    role: "ad/role",
    userRole: "ad/user-role",
    page: "ad/page",
    control: "ad/control",
    config: "ad/config",
    category: "category",
    dataPeriod: " ",
    common: "m/common",
    personalInformation: "ad/personal-infor",
    autoNumber: "auto-number",
    home: "home",
    unit: "unit",
    address: "m/address",
    unitContact: 'm/unit-contact',
    portal: "portal",
    dataMenu: 'data/data-menu',
    file: "file",
    accounts: "data/accounts",
    accountDetail: "data/account-detail",
    selectOption: "select-option",
    forgotPassword: 'forgot-password',
    pro5Organization: "profile/organization",
    law: "law",



  };
}
