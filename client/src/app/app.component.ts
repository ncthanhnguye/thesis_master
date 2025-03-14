import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { AppService } from './services/app.service';
import { AuthenticationService } from './services/authentication.service';
import { TranslateService } from '@ngx-translate/core';
import { AppLanguage } from './services/app.language';
@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	//styleUrls: ['./app.component.css']
})
export class AppComponent {
	@ViewChild('appMenu') appMenu;

	title = 'Law';
	loading = false;
	openMenuFlg = true;
	user: any;
	menuHidden = false;
	isLoginPage = false;

	constructor(
		private authenticationService: AuthenticationService,
		private translate: TranslateService,
		private language: AppLanguage,
	) {
		this.authenticationService.getUser();
		this.user = this.authenticationService.user;
		this.language.setDefaultLanguage();
	}

	public value: Date = new Date();

	ngOnInit(): void {
		if (localStorage.getItem('ThesisLocalStorage')) {
			this.menuHidden = false;
		}
		else {
			this.menuHidden = true;
			this.isLoginPage = true;
		}		
	}

	toggleMenu() {
		this.openMenuFlg = !this.openMenuFlg;
	  }

	closeDialog = () => {
		const body = document.body;
		const scrollY = body.style.top;
		body.style.position = '';
		body.style.top = '';
		body.style.overflowY = 'auto';
		body.style.height = 'auto';
		window.scrollTo(0, parseInt(scrollY || '0') * -1);
		document.getElementById("body-block").classList.remove('show');
	}

	switchLanguage(language: string) {
		this.language.set(language);
	}

	/// Gán giá trị là (Trống) nếu không có data trả về
	mapDataWithDefault(data) {
		const newData = { ...data };
		// Duyệt qua các thuộc tính của đối tượng, thay thế nếu null hoặc undefined hoặc '
		for (const key in newData) {
			if (newData[key] === null || newData[key] === undefined || newData[key] == '') {
				newData[key] = '(Trống)';
			}
		}
		return newData;
	}
}
