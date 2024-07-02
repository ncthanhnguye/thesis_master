import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../services/authentication.service';
import { Router, ActivatedRoute, Params } from '@angular/router';

@Component({
  selector: 'app-permision',
  templateUrl: './permision.component.html',
  //styleUrls: ['./permision.component.css']
})
export class PermisionComponent implements OnInit {

  constructor(
    private authenticationService: AuthenticationService,
    private router: Router,
  ) { }

  ngOnInit() {
  }

  onLogOut() {
    if (localStorage.getItem('ThesisLocalStorage')) {
      this.authenticationService.doSignout();
    }
  }

}
