import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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
	openMenuFlg = false;
	user: any;
	menuHidden = false;
	constructor(
		private authenticationService: AuthenticationService,
		private translate: TranslateService,
		private language: AppLanguage,
	) {
		if (this.appMenu) this.appMenu.openMenuFlg = '0';
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
		}
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
}
