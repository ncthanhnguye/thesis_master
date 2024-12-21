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
