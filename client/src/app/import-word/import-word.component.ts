import {Component, OnInit} from '@angular/core';
import {TranslateService} from '@ngx-translate/core';
import {IntlService} from '@progress/kendo-angular-intl';
import {FileInfo, SelectEvent} from '@progress/kendo-angular-upload';
import {AppComponent} from '../app.component';
import {AppConsts} from '../services/app.consts';
import {AppControls} from '../services/app.controls';
import {AppFile} from '../services/app.file';
import {AppService} from '../services/app.service';
import {AppSwal} from '../services/app.swal';
import {AppUtils} from '../services/app.utils';
import {AuthenticationService} from '../services/authentication.service';
import {Notification} from '../services/app.notification';
import {AppLanguage} from '../services/app.language';
import {Router} from '@angular/router';
import {v4 as uuidv4} from 'uuid';

@Component({
  selector: 'app-import-word',
  templateUrl: './import-word.component.html',
  styleUrls: ['./import-word.component.css']
})
export class ImportWordComponent implements OnInit {
  lawData: {
    // ID: number,
    Title: string,
    Content: string,
    ContentHTML: string,
    LawNumber: string,
    LawDate: string,
    TotalChapter: number,
    Status: number,
  }
  lawWordData: {
    lawNumber: string,
    lawDate: string,
    chuong: Array<{ content: string, muc: any[], dieu: any[] }>,
    muc: Array<{ content: string, dieu: any[] }>,
    dieu: Array<{ content: string, khoan: any[] }>,
    khoan: Array<{ content: string, diem: any[] }>,
    diem: Array<{ content: string }>
  }

  lastLawID: number;
  lastChapterID: number;
  lastChapterItemID: number;
  lastArticalID: number;
  lastClaustID: number;
  lastPointID: number;

  user: any;
  loading = false;
  pageName = 'Import dữ liệu về luật';
  isFileImport = false;
  fileDataImport: any;
  nameLaw: '';

  Law: Array<{ ID: number; Index: number; Title: string; Content: string; ContentHTML: string; }> = [];

  public uploadSaveUrl = 'saveUrl';
  public uploadRemoveUrl = 'removeUrl';

  filesUpload: Array<FileInfo>;
  filesUploadName = "";

  public fileSaveUrl: any;

  tempData = [
    {
      UserName: '',
      Name: '',
      GenderName: '',
      BirthDate: '',
      UnitName: '',
    }
  ]

  constructor(
    private translate: TranslateService,
    private appService: AppService,
    private language: AppLanguage,
    private appSwal: AppSwal,
    public intl: IntlService,
    private router: Router,
    private notification: Notification,
    private file: AppFile,
    private authenticationService: AuthenticationService,
    public appControls: AppControls,
    private appConsts: AppConsts,
    private appComponent: AppComponent,
    public appUtils: AppUtils,
  ) {
    this.authenticationService.getUser();
    this.user = this.authenticationService.user;
    this.setDefault();
    this.lawData = {
      // ID: number,
      Title: null,
      Content: null,
      ContentHTML: null,
      LawDate: null,
      LawNumber: null,
      TotalChapter: null,
      Status: null,
    }
  }

  async ngOnInit() {
    await this.getLastLawItem();
    await this.getLastChapterItem();
    await this.getLastItemInChapterItem();
    await this.getLastArticalItem();
    await this.getLastClaustItem();
    await this.getLastPointItem();
  }

  setDefault() {
    this.filesUpload = [];
    this.filesUploadName = "";
    this.fileSaveUrl = `${this.appService.apiRoot}api/Upload?dirName=${this.user.UserName}&typeData=files&acceptExtensions=[".doc",".docx"]`;
  }

  onCancelImport() {
    this.isFileImport = !this.isFileImport;
    this.fileDataImport = '';
  }

  onSelectEventHandler(e: SelectEvent) {
    this.loadDoc(e);
  }

  async loadDoc(e) {
    this.isFileImport = true;
    this.fileDataImport = (await this.file.readDocx(e.files[0].rawFile, 'html')) as string;
    if (this.fileDataImport != null || this.fileDataImport != undefined) {
      this.lawData.ContentHTML = this.fileDataImport;
    }

    const rawData = (await this.file.readDocx(e.files[0].rawFile, 'text')) as string
    if (this.fileDataImport != null) {
      this.processText(rawData);
      this.showSuccessMessage();
    }
  }

  processText(text: string) {
    // Create new UUID
    let nextLawID = uuidv4();
    let nextChapterID = uuidv4();
    let nextMucID = uuidv4();
    let nextDieuID = uuidv4();
    let nextKhoanID = uuidv4();
    let nextDiemID = uuidv4();

    this.lawWordData = { lawDate: '', lawNumber: '', chuong: [], muc: [], dieu: [], khoan: [], diem: [] };

    const lines = text.split('\n');

    let currentChuong = { ID: '', content: '', muc: [], dieu: [] };
    let currentMuc = { ID: '', content: '', dieu: [], chapterID: '' };
    let currentDieu = { ID: '', content: '', khoan: [], chapterID: '', mucID: '' };
    let currentKhoan = { ID: '', content: '', diem: [], chapterID: '', mucID: '', dieuID: '' };

    let currentContent = '', currentType = '';
    let isDateCollected = false;

    const saveContent = () => {
      if (currentContent) {
        if (currentType === 'chuong') {
          const chapterID = uuidv4();
          currentChuong = {
            ID: chapterID,
            content: currentContent.trim(),
            muc: [],
            dieu: []
          };
          this.lawWordData.chuong.push(currentChuong);
          currentMuc = { ID: '', content: '', dieu: [], chapterID: chapterID }; // Sử dụng nextChapterID đã lưu
          nextChapterID = chapterID;
        } else if (currentType === 'muc') {
          // Kiểm tra nếu mục đã tồn tại trong chương để tránh tạo lại mục trùng
          if (!currentChuong.muc.some(m => m.content === currentContent.trim())) {
            const chapterItemID = uuidv4();
            currentMuc = {
              ID: chapterItemID,
              content: currentContent.trim(),
              dieu: [],
              chapterID: nextChapterID
            };
            currentChuong.muc.push(currentMuc);
            nextMucID = chapterItemID;
          }
        } else if (currentType === 'dieu') {
          const articalID = uuidv4();
          currentDieu = {
            ID: articalID,
            content: currentContent.trim(),
            khoan: [],
            chapterID: nextChapterID,
            mucID: currentMuc.ID ? nextMucID : '',
          };
          if (currentMuc && currentMuc.content.trim() !== '') {
            currentMuc.dieu.push(currentDieu);
          } else {
            currentChuong.dieu.push(currentDieu);
          }
          nextDieuID = articalID;
        } else if (currentType === 'khoan') {
          const claustID = uuidv4();
          currentKhoan = {
            ID: claustID,
            content: currentContent.trim(),
            diem: [],
            chapterID: nextChapterID,
            mucID: currentMuc.ID ? nextMucID : '',
            dieuID: currentDieu.ID ? nextDieuID : ''
          };
          currentDieu.khoan.push(currentKhoan);
          nextKhoanID = claustID;
        } else if (currentType === 'diem') {
          const pointID = uuidv4();
          currentKhoan.diem.push({
            ID: nextDiemID,
            content: currentContent.trim(),
            chapterID: nextChapterID,
            mucID: currentMuc.ID ? nextMucID : '',
            dieuID: currentDieu.ID ? nextDieuID : '',
            khoanID: currentKhoan.ID ? nextKhoanID : ''
          });
          nextDiemID = pointID;
        }
      }
    };

    // Hàm nối nội dung vào các đoạn văn bản sau tiêu đề
    const appendContent = (trimmedLine: string) => {
      // Check if the line contains only a title without full content
      const isTitleOnlyLine = (line: string) => {
        const titleFullRegex = /^(Chương\s+\S+|Mục\s+\S+|Điều\s+\d+|\d+\.\s+|[a-z]\)\s+)/;
        const match = line.match(titleFullRegex);
        if (match) {
          const title = match[0];
          const content = line.replace(title, '').trim();

          if (content === '') {
            return true;
          }
          else {
            return false;
          }
        }
      };

      if (currentType === 'chuong') {
        if (isTitleOnlyLine(currentContent) == true) {
          currentChuong.content += ' ' + trimmedLine;
        }
      } else if (currentType === 'muc') {
        if (isTitleOnlyLine(trimmedLine)) {
          currentMuc.content += ' ' + trimmedLine;
        }
      } else if (currentType === 'dieu') {
        if (isTitleOnlyLine(trimmedLine)) {
          currentDieu.content += ' ' + trimmedLine;
        }
      } else if (currentType === 'khoan') {
        if (isTitleOnlyLine(trimmedLine)) {
          currentKhoan.content += ' ' + trimmedLine;
        }
      } else if (currentType === 'diem') {
        if (currentKhoan.diem.length > 0) {
          // If already has a point, append to the last one
          const lastDiem = currentKhoan.diem[currentKhoan.diem.length - 1];
          if (isTitleOnlyLine(trimmedLine)) {
            lastDiem.content += ' ' + trimmedLine;  // Append content if part of the description
          }
        } else {
          // If no point exists, insert a new point with the current content
          currentKhoan.diem.push({
            content: trimmedLine,
          });
        }
      }
    };

    // Extract dữ liệu từ các dòng
    lines.forEach(line => {
      const trimmedLine = line.trim();

      // Lấy thông tin số và ngày của luật
      if (/^(Bộ luật số:|Số:)/.test(trimmedLine)) {
        this.lawWordData.lawNumber = trimmedLine.replace(/(Bộ luật số:|Số:)/, '').trim();
      }
      if (trimmedLine.includes('ngày') && !isDateCollected) {
        const ngayMatch = trimmedLine.match(/ngày (\d{1,2}) tháng (\d{1,2}) năm (\d{4})/);
        if (ngayMatch) {
          const ngay = ngayMatch[1].padStart(2, '0');
          const thang = ngayMatch[2].padStart(2, '0');
          const nam = ngayMatch[3];
          this.lawWordData.lawDate = `${ngay}/${thang}/${nam}`;
          isDateCollected = true;
        }
      }

      // Phân loại các chương, mục, điều, khoản, điểm
      if (trimmedLine.startsWith('Chương ')) {
        saveContent();
        currentContent = trimmedLine;
        currentType = 'chuong';
      } else if (trimmedLine.startsWith('Mục ')) {
        saveContent();
        currentContent = trimmedLine;
        currentType = 'muc';
      } else if (trimmedLine.startsWith('Điều ')) {
        saveContent();
        currentContent = trimmedLine;
        currentType = 'dieu';
      } else if (/^\d+\./.test(trimmedLine)) {  // Khoản
        saveContent();
        currentContent = trimmedLine;
        currentType = 'khoan';
      } else if (/^[a-z]\)/.test(trimmedLine)) {  // Điểm
        saveContent();
        currentContent = trimmedLine;
        currentType = 'diem';
      } else if (currentType && trimmedLine !== '') {
        appendContent(trimmedLine);
      }
    });

    saveContent();  // Lưu nội dung cuối cùng sau khi kết thúc vòng lặp

    console.log('Extracted data with UUIDs:', this.lawWordData);
  }


  // processText(text: string) {
  //   this.lawWordData = { lawDate: '', lawNumber: '', chuong: [], muc: [], dieu: [], khoan: [], diem: [] };
  //   const lines = text.split('\n');
  //
  //   let currentChuong = { content: '', muc: [], dieu: [] };
  //   let currentMuc = { content: '', dieu: [] };
  //   let currentDieu = { content: '', khoan: [] };
  //   let currentKhoan = { content: '', diem: [] };
  //
  //   let currentContent = '', currentType = '';
  //   let isDateCollected = false;
  //
  //   // Hàm lưu nội dung hiện tại vào cấp thích hợp
  //   const saveContent = () => {
  //     if (currentContent) {
  //       if (currentType === 'chuong') {
  //         currentChuong = { content: currentContent.trim(), muc: [], dieu: [] };
  //         this.lawWordData.chuong.push(currentChuong);
  //         currentMuc = { content: '', dieu: [] };
  //       } else if (currentType === 'muc') {
  //         if (!currentChuong.muc.some(m => m.content === currentContent.trim())) {
  //           currentMuc = { content: currentContent.trim(), dieu: [] };
  //           currentChuong.muc.push(currentMuc);
  //         }
  //       } else if (currentType === 'dieu') {
  //         currentDieu = { content: currentContent.trim(), khoan: [] };
  //
  //         if (currentMuc && currentMuc.content.trim() !== '') {
  //           currentMuc.dieu.push(currentDieu);
  //         } else {
  //           currentChuong.dieu.push(currentDieu);
  //         }
  //       } else if (currentType === 'khoan') {
  //         currentKhoan = { content: currentContent.trim(), diem: [] };
  //         currentDieu.khoan.push(currentKhoan);
  //       } else if (currentType === 'diem') {
  //         currentKhoan.diem.push({ content: currentContent.trim() });
  //       }
  //     }
  //   };
  //
  //   // Hàm nối nội dung vào các đoạn văn bản sau tiêu đề
  //   const appendContent = (trimmedLine: string) => {
  //     // Check if the line contains only a title without full content
  //     const isTitleOnlyLine = (line: string) => {
  //       const titleFullRegex = /^(Chương\s+\S+|Mục\s+\S+|Điều\s+\d+|\d+\.\s+|[a-z]\)\s+)/;
  //       const match = line.match(titleFullRegex);
  //       if (match) {
  //         const title = match[0];
  //         const content = line.replace(title, '').trim();
  //
  //         if (content === '') {
  //           return true;
  //         }
  //         else {
  //           return false;
  //         }
  //       }
  //     };
  //
  //     if (currentType === 'chuong') {
  //       if (isTitleOnlyLine(currentContent) == true) {
  //         currentChuong.content += ' ' + trimmedLine;
  //       }
  //     } else if (currentType === 'muc') {
  //       if (isTitleOnlyLine(trimmedLine)) {
  //         currentMuc.content += ' ' + trimmedLine;
  //       }
  //     } else if (currentType === 'dieu') {
  //       if (isTitleOnlyLine(trimmedLine)) {
  //         currentDieu.content += ' ' + trimmedLine;
  //       }
  //     } else if (currentType === 'khoan') {
  //       if (isTitleOnlyLine(trimmedLine)) {
  //         currentKhoan.content += ' ' + trimmedLine;
  //       }
  //     } else if (currentType === 'diem') {
  //       if (currentKhoan.diem.length > 0) {
  //         // If already has a point, append to the last one
  //         const lastDiem = currentKhoan.diem[currentKhoan.diem.length - 1];
  //         if (isTitleOnlyLine(trimmedLine)) {
  //           lastDiem.content += ' ' + trimmedLine;  // Append content if part of the description
  //         }
  //       } else {
  //         // If no point exists, insert a new point with the current content
  //         currentKhoan.diem.push({
  //           content: trimmedLine,
  //         });
  //       }
  //     }
  //   };
  //
  //   // Extract dữ liệu từ các dòng
  //   lines.forEach(line => {
  //     const trimmedLine = line.trim();
  //
  //     // Lấy thông tin số và ngày của luật
  //     if (/^(Bộ luật số:|Số:)/.test(trimmedLine)) {
  //       const boLuatSo = trimmedLine.replace(/(Bộ luật số:|Số:)/, '').trim();
  //       this.lawWordData.lawNumber = boLuatSo;
  //     }
  //     if (trimmedLine.includes('ngày') && !isDateCollected) {
  //       const ngayMatch = trimmedLine.match(/ngày (\d{1,2}) tháng (\d{1,2}) năm (\d{4})/);
  //       if (ngayMatch) {
  //         const ngay = ngayMatch[1].padStart(2, '0');
  //         const thang = ngayMatch[2].padStart(2, '0');
  //         const nam = ngayMatch[3];
  //         const lawDate = `${ngay}/${thang}/${nam}`;
  //         this.lawWordData.lawDate = lawDate;
  //         isDateCollected = true;
  //       }
  //     }
  //
  //     // Phân loại các chương, mục, điều, khoản, điểm
  //     if (trimmedLine.startsWith('Chương ')) {
  //       saveContent();
  //       currentContent = trimmedLine;
  //       currentType = 'chuong';
  //     } else if (trimmedLine.startsWith('Mục ')) {
  //       saveContent();
  //       currentContent = trimmedLine;
  //       currentType = 'muc';
  //     } else if (trimmedLine.startsWith('Điều ')) {
  //       saveContent();
  //       currentContent = trimmedLine;
  //       currentType = 'dieu';
  //     } else if (/^\d+\./.test(trimmedLine)) {  // Khoản
  //       saveContent();
  //       currentContent = trimmedLine;
  //       currentType = 'khoan';
  //     } else if (/^[a-z]\)/.test(trimmedLine)) {  // Điểm
  //       saveContent();
  //       currentContent = trimmedLine;
  //       currentType = 'diem';
  //     } else if (currentType && trimmedLine !== '') {
  //       appendContent(trimmedLine);
  //     }
  //   });
  //
  //   saveContent();  // Lưu nội dung cuối cùng sau khi kết thúc vòng lặp
  //
  //   console.log('Extracted data:', this.lawWordData);
  // }

  showSuccessMessage() {
    const successElement = document.querySelector('.success');
    successElement.classList.add('visible');
    setTimeout(() => {
      successElement.classList.remove('visible');
    }, 5000);
  }

  // Luật
  async getLastLawItem() {
    try {
      const result = await this.appService.doGET('api/Law/GetLastLawID', null);

      if (result) {
        this.Law = result.Data;
        this.lastLawID = result.Data.ID;
      } else {
        console.log('Error data');
      }
    } catch (e) {
      console.error('Error: ', e);
    }
  }

  // Chương
  async getLastChapterItem() {
    try {
      const result = await this.appService.doGET('api/Chapter/GetLastChapterID', null);

      if (result) {
        this.lastChapterID = result.Data.ID;
      } else {
        console.log('Error data');
      }
    } catch (e) {
      console.error('Error: ', e);
    }
  }

  // Mục
  async getLastItemInChapterItem() {
    try {
      const result = await this.appService.doGET('api/ChapterItem/GetLastChapterItemID', null);

      if (result) {
        this.lastChapterItemID = result.Data.ID;
      } else {
        console.log('Error data');
      }
    } catch (e) {
      console.error('Error: ', e);
    }
  }

  // Điều
  async getLastArticalItem() {
    try {
      const result = await this.appService.doGET('api/Artical/GetLastArticalID', null);

      if (result) {
        this.lastArticalID = result.Data.ID;
      } else {
        console.log('Error data');
      }
    } catch (e) {
      console.error('Error: ', e);
    }
  }

  // Khoản
  async getLastClaustItem() {
    try {
      const result = await this.appService.doGET('api/Claust/GetLastClaustID', null);

      if (result) {
        this.lastClaustID = result.Data.ID;
      } else {
        console.log('Error data');
      }
    } catch (e) {
      console.error('Error: ', e);
    }
  }

  // Điểm
  async getLastPointItem() {
    try {
      const result = await this.appService.doGET('api/Point/GetLastPointID', null);

      if (result) {
        this.lastPointID = result.Data.ID;
      } else {
        console.log('Error data');
      }
    } catch (e) {
      console.error('Error: ', e);
    }
  }

  async onSaveLaw() {
    this.lawData.Content = this.nameLaw;
    console.log('lawData', this.lawData);
    const dataRequest = [this.lawData];
    const result = await this.appService.doPOST('api/ImportWordLaw/Saves', dataRequest);
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      await this.router.navigate([AppConsts.page.law]);
    } else {
      await this.appSwal.showWarning(result.Msg, false);
    }
  }
}
