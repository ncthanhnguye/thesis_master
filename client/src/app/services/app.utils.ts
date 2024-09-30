import { Injectable, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { IntlService } from '@progress/kendo-angular-intl';

@Injectable({
    providedIn: 'root'
})
export class AppUtils implements OnInit {

    ngOnInit(): void {
    }

    constructor(
        private translate: TranslateService,
        public intl: IntlService
    ) {
    }

    public getDateByPeriod(period: any) {
        const result = {
            FromDate: new Date(),
            ToDate: new Date()
        };

        if (period === 6) {
            const currentMonth = result.FromDate.getMonth() - 1;
            if (currentMonth < 3) {
                period = 7;
            } else if (currentMonth < 6) {
                period = 8;
            } else if (currentMonth < 9) {
                period = 9;
            } else {
                period = 10;
            }
        }

        let day = result.FromDate.getDay();
        if (day === 0) {
            day = 7;
        }

        switch (period) {
            // last day
            case 0: {
                break;
            }
            // last day
            case 1: {
                result.FromDate.setDate(result.FromDate.getDate() - 1);
                result.ToDate = result.FromDate;
                break;
            }
            // next week
            case 13: {
                result.FromDate = new Date(result.FromDate.setDate(result.FromDate.getDate() + 7));
                const from = new Date(result.FromDate.setDate(result.FromDate.getDate() - day + 1));
                const temp = new Date(from);
                const to = new Date(temp.setDate(temp.getDate() + 6));
                result.FromDate = new Date(from.getFullYear(), from.getMonth(), from.getDate(), 0, 0, 0);
                result.ToDate = new Date(to.getFullYear(), to.getMonth(), to.getDate(), 23, 59, 59);
                break;
            }
            // this week
            // tslint:disable-next-line:no-switch-case-fall-through
            case 2: {
                const from = new Date(result.FromDate.setDate(result.FromDate.getDate() - day + 1));
                const temp = new Date(from);
                const to = new Date(temp.setDate(temp.getDate() + 6));
                result.FromDate = new Date(from.getFullYear(), from.getMonth(), from.getDate(), 0, 0, 0);
                result.ToDate = new Date(to.getFullYear(), to.getMonth(), to.getDate(), 23, 59, 59);
                break;
            }
            // last week
            // tslint:disable-next-line:no-switch-case-fall-through
            case 3: {
                result.FromDate = new Date(result.FromDate.setDate(result.FromDate.getDate() - 7));
                const from = new Date(result.FromDate.setDate(result.FromDate.getDate() - day + 1));
                const temp = new Date(from);
                const to = new Date(temp.setDate(temp.getDate() + 6));
                result.FromDate = new Date(from.getFullYear(), from.getMonth(), from.getDate(), 0, 0, 0);
                result.ToDate = new Date(to.getFullYear(), to.getMonth(), to.getDate(), 23, 59, 59);
                break;
            }
            // This month
            // tslint:disable-next-line:no-switch-case-fall-through
            case 4: {
                const from = new Date(result.FromDate.getFullYear(), result.FromDate.getMonth(), 1);
                const to = new Date(result.FromDate.getFullYear(), result.FromDate.getMonth() + 1, 0);
                result.FromDate = new Date(from.getFullYear(), from.getMonth(), from.getDate(), 0, 0, 0);
                result.ToDate = new Date(to.getFullYear(), to.getMonth(), to.getDate(), 23, 59, 59);
                break;
            }
            // Last month
            // tslint:disable-next-line:no-switch-case-fall-through
            case 5: {
                const from = new Date(result.FromDate.getFullYear(), result.FromDate.getMonth() - 1, 1);
                const to = new Date(result.FromDate.getFullYear(), result.FromDate.getMonth(), 0);
                result.FromDate = new Date(from.getFullYear(), from.getMonth(), from.getDate(), 0, 0, 0);
                result.ToDate = new Date(to.getFullYear(), to.getMonth(), to.getDate(), 23, 59, 59);
                break;
            }
            // 1 Quarter
            // tslint:disable-next-line:no-switch-case-fall-through
            case 7: {
                const from = new Date(result.FromDate.getFullYear(), 0, 1);
                const to = new Date(result.FromDate.getFullYear(), 3, 0);
                result.FromDate = new Date(from.getFullYear(), from.getMonth(), from.getDate(), 0, 0, 0);
                result.ToDate = new Date(to.getFullYear(), to.getMonth(), to.getDate(), 23, 59, 59);
                break;
            }
            // 2 Quarter
            // tslint:disable-next-line:no-switch-case-fall-through
            case 8: {
                const from = new Date(result.FromDate.getFullYear(), 3, 1);
                const to = new Date(result.FromDate.getFullYear(), 6, 0);
                result.FromDate = new Date(from.getFullYear(), from.getMonth(), from.getDate(), 0, 0, 0);
                result.ToDate = new Date(to.getFullYear(), to.getMonth(), to.getDate(), 23, 59, 59);
                break;
            }
            // 2 Quarter
            // tslint:disable-next-line:no-switch-case-fall-through
            case 9: {
                const from = new Date(result.FromDate.getFullYear(), 6, 1);
                const to = new Date(result.FromDate.getFullYear(), 9, 0);
                result.FromDate = new Date(from.getFullYear(), from.getMonth(), from.getDate(), 0, 0, 0);
                result.ToDate = new Date(to.getFullYear(), to.getMonth(), to.getDate(), 23, 59, 59);
                break;
            }
            // 2 Quarter
            // tslint:disable-next-line:no-switch-case-fall-through
            case 10: {
                const from = new Date(result.FromDate.getFullYear(), 9, 1);
                const to = new Date(result.FromDate.getFullYear(), 12, 0);
                result.FromDate = new Date(from.getFullYear(), from.getMonth(), from.getDate(), 0, 0, 0);
                result.ToDate = new Date(to.getFullYear(), to.getMonth(), to.getDate(), 23, 59, 59);
                break;
            }
            // This year
            // tslint:disable-next-line:no-switch-case-fall-through
            case 11: {
                const from = new Date(result.FromDate.getFullYear(), 0, 1);
                const to = new Date(result.FromDate.getFullYear(), 12, 0);
                result.FromDate = new Date(from.getFullYear(), from.getMonth(), from.getDate(), 0, 0, 0);
                result.ToDate = new Date(to.getFullYear(), to.getMonth(), to.getDate(), 23, 59, 59);
                break;
            }
            // Last year
            // tslint:disable-next-line:no-switch-case-fall-through
            case 12: {
                const from = new Date(result.FromDate.getFullYear() - 1, 0, 1);
                const to = new Date(result.FromDate.getFullYear() - 1, 12, 0);
                result.FromDate = new Date(from.getFullYear(), from.getMonth(), from.getDate(), 0, 0, 0);
                result.ToDate = new Date(to.getFullYear(), to.getMonth(), to.getDate(), 23, 59, 59);
                break;
            }
            default: {
                return null;
            }
        }

        return result;
    }

    public getDate(dataStr: any) {
        const result = {
            Value: new Date(),
            Display: ''
        };

        try {
            result.Value = this.intl.parseDate(dataStr, this.translate.instant('FormatDate'));
        } catch (e) {
            console.log(e);
        }

        result.Display = this.intl.formatDate(result.Value, this.translate.instant('FormatDate'));
        if (!result.Display || result.Display.indexOf('NaN') >= 0) {
            result.Display = '';
        }

        return result;
    }

    public getDate2(dataStr: any) {
        const result = {
            Value: new Date(),
            Display: ''
        };

        try {
            result.Value = this.intl.parseDate(dataStr, this.translate.instant('FormatDate2'));
        } catch (e) {
            console.log(e);
        }

        result.Display = this.intl.formatDate(result.Value, this.translate.instant('FormatDate2'));
        if (!result.Display || result.Display.indexOf('NaN') >= 0) {
            result.Display = '';
        }

        return result;
    }

    // input '1970-01-01T00:00:00' or '2019-07-01 13:58:27.087'. Output : 25/06/1994 or 1994-06-25T15:30:13 depend on FormatDate string
    public getDateTimeFromformat (dateTimestr : any, formatDate : any)     {
        // check invalid date
                // differ from cause it read from db
        //if (!(dateTimestr instanceof Date) || isNaN(dateTimestr.getTime()))     {return null;}
        if (dateTimestr == null || typeof dateTimestr == 'undefined' || dateTimestr.toString() == '' || dateTimestr.toString().toLowerCase() == 'null')    {return null;}
        var splitedDateTime = dateTimestr.toString().replace('T', '-');
        splitedDateTime = splitedDateTime.replace(' ', '-');
        var splitedDateTimearr = splitedDateTime.split('-');
        var day : any;  var month : any;    var year : any; var date : any;
        var hour : any;  var minute : any;    var seconds : any;
        year  = splitedDateTimearr[0];
        month = splitedDateTimearr[1];
        date  = splitedDateTimearr[2];
        hour = splitedDateTimearr[3].toLowerCase().split(':')[0];
        minute = splitedDateTimearr[3].toLowerCase().split(':')[1];
        seconds = splitedDateTimearr[3].toLowerCase().split(':')[2];
        if (formatDate == 'dd/MM/yyyy') {
            return `${date}/${month}/${year}`;
        }
        return `${year}-${month}-${date}T${splitedDateTimearr[3]}`;
    }

    public compareString(str1: string, str2: string, str3 = null) {
        if (!str1 || !str2) {
            return false;
        }

        let result = str1.trim().toLowerCase() === str2.trim().toLowerCase();
        if (str3 && !result) {
            try {
                result = str1.trim().toLowerCase() === str3.trim().toLowerCase();
            } catch (e) { }
        }

        return result;
    }

    public getNameByList(list: any, id: any) {        
        let i: any;
        for (i = 0; i < list.length; i++) {
            if (list[i].ID === id) {
                const dataRendered = list[i].Title + '.' + ' ' + list[i].Content;
                return dataRendered;
            }
        }
    }

    public getObjectByList(list: any, value: any) {
        if (list == null || list.length <= 0 || value == null || value == "") {
            return null;
        }

        return list.find(k => k.ID.toLocaleLowerCase() == value.toLocaleLowerCase() || k.Name.toLocaleLowerCase() == value.toLocaleLowerCase());
    }
    public getGenderByList(list: any, value: any) {

      return list.find(k => k.Name.toLocaleLowerCase() === value.toLocaleLowerCase());
  }


    public getWeekday(dateStr) {
        var date = new Date(dateStr);
        var day = date.getDay();

        return day == 0 ? 'CN' : `Thứ ${day + 1}`;
    }
}
