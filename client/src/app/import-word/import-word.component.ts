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
    lawDate: string;
    lawNumber: string;
    chapters: any[];
    chapterItems: any[];
    articals: any[];
    clausts: any[];
    points: any[];
  }

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
    // Tạo UUID mới cho các mục
    let nextChuongID = uuidv4();
    let nextMucID = uuidv4();
    let nextDieuID = uuidv4();
    let nextKhoanID = uuidv4();
    let nextDiemID = uuidv4();

    this.lawWordData = {
      lawDate: '',
      lawNumber: '',
      chapters: [],
      chapterItems: [],
      articals: [],
      clausts: [],
      points: []
    };

    const lines = text.split('\n');
    let currentChuong = { ID: '', content: '', luatID: null };
    let currentMuc = { ID: '', content: '', luatID: null, chuongID: '' };
    let currentDieu = { ID: '', content: '', luatID: null, chuongID: '', mucID: '' };
    let currentKhoan = { ID: '', content: '', luatID: null, chuongID: '', mucID: '', dieuID: '' };

    let currentContent = '', currentType = '';
    let isDateCollected = false;

    const saveContent = () => {
      if (currentContent) {
        if (currentType === 'chuong') {
          const chuongID = uuidv4();
          currentChuong = {
            ID: chuongID,
            content: currentContent.trim(),
            luatID: null
          };
          this.lawWordData.chapters.push(currentChuong);
          nextChuongID = chuongID;  // Cập nhật UUID cho chương tiếp theo
        } else if (currentType === 'muc') {
          const chapterItemID = uuidv4();
          currentMuc = {
            ID: chapterItemID,
            content: currentContent.trim(),
            luatID: null,
            chuongID: nextChuongID
          };
          this.lawWordData.chapterItems.push(currentMuc);
          nextMucID = chapterItemID;  // Cập nhật UUID cho mục tiếp theo
        } else if (currentType === 'dieu') {
          const articalID = uuidv4();
          currentDieu = {
            ID: articalID,
            content: currentContent.trim(),
            luatID: null,
            chuongID: nextChuongID,
            mucID: currentMuc.ID || ''
          };
          this.lawWordData.articals.push(currentDieu);
          nextDieuID = articalID;  // Cập nhật UUID cho điều tiếp theo
        } else if (currentType === 'khoan') {
          const claustID = uuidv4();
          currentKhoan = {
            ID: claustID,
            content: currentContent.trim(),
            luatID: null,
            chuongID: nextChuongID,
            mucID: currentMuc.ID || '',
            dieuID: currentDieu.ID || ''
          };
          this.lawWordData.clausts.push(currentKhoan);
          nextKhoanID = claustID;  // Cập nhật UUID cho khoản tiếp theo
        } else if (currentType === 'diem') {
          const pointID = uuidv4();
          this.lawWordData.points.push({
            ID: nextDiemID,
            content: currentContent.trim(),
            luatID: null,
            chuongID: nextChuongID,
            mucID: currentMuc.ID || '',
            dieuID: currentDieu.ID || '',
            khoanID: currentKhoan.ID || ''
          });
          nextDiemID = pointID;  // Cập nhật UUID cho điểm tiếp theo
        }
      }
    };

    // Hàm nối nội dung vào các đoạn văn bản sau tiêu đề
    const appendContent = (trimmedLine: string) => {
      if (currentType === 'chuong') {
        currentChuong.content += ' ' + trimmedLine;
      } else if (currentType === 'muc') {
        currentMuc.content += ' ' + trimmedLine;
      } else if (currentType === 'dieu') {
        currentDieu.content += ' ' + trimmedLine;
      } else if (currentType === 'khoan') {
        currentKhoan.content += ' ' + trimmedLine;
      } else if (currentType === 'diem') {
        const lastDiem = this.lawWordData.points[this.lawWordData.points.length - 1];
        lastDiem.content += ' ' + trimmedLine;
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

    saveContent();  // Lưu nội dung cuối cùng

    console.log('Extracted data with UUIDs:', this.lawWordData);
  }

  showSuccessMessage() {
    const successElement = document.querySelector('.success');
    successElement.classList.add('visible');
    setTimeout(() => {
      successElement.classList.remove('visible');
    }, 5000);
  }

  async onSaveLaw() {
    this.lawData.Content = this.nameLaw;
    console.log('lawData', this.lawData);
    const dataRequest = [this.lawData];
    const result = await this.appService.doPOST('api/ImportWordLaw/Saves', this.lawWordData);
    if (result && result.Status === 1) {
      this.notification.showSuccess(result.Msg);
      await this.router.navigate([AppConsts.page.law]);
    } else {
      await this.appSwal.showWarning(result.Msg, false);
    }
  }
}
