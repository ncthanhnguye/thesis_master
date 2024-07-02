import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { AppService } from '../services/app.service';
import { AuthenticationService } from '../services/authentication.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {
  @Output() closeMenu = new EventEmitter<boolean>();

  MenuFilter: any;
  user: any;
  selectedMenuID: any;

  constructor(
    private router: Router,
    private appService: AppService ,
    private authenticationService: AuthenticationService ) {

   }

  ngOnInit() {
    this.getMenu();

  }

  onReload() {
    this.getMenu();
  }

  async getMenu() {
    
    this.authenticationService.getUser();
    this.user = this.authenticationService.user;
    const roleID = this.user.RoleID;
    const result = await this.appService.doGET('api/Page/GetMenu', {roleID} );
    if (result && result.Status === 1) {
      this.MenuFilter = result.Data;
    }
  }

  onClickMenu(menuItem) {
    if (this.selectedMenuID == menuItem.Url) {
      this.selectedMenuID = null;
      return;
    }


    if (menuItem.Url && (menuItem.IsButton || !menuItem.Childrens|| menuItem.Childrens.length <= 0)) {
      this.router.navigate([menuItem.Url]);
    } else {
      this.selectedMenuID = menuItem.Url;
    }
  }
}
