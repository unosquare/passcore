import { Component, AfterViewInit } from '@angular/core';
import { Http, Response } from '@angular/http';

import 'rxjs/add/operator/map';

@Component({
  selector: 'app-root',
  templateUrl: '../views/change-password.html'
})
export class ChangePasswordComponent implements AfterViewInit {
  ViewOptions: any;
  constructor(private http: Http) { }

  ngAfterViewInit(): void {
    this.http.get('api/password').subscribe(values => {     
      this.ViewOptions = values.json() as string;
      console.log(this.ViewOptions);
      if (this.ViewOptions.recaptcha.isEnabled == true) {
        let sp = document.createElement('script'); sp.type = 'text/javascript'; sp.async = true; sp.defer = true;
        sp.src = 'https://www.google.com/recaptcha/api.js?onload=vcRecaptchaApiLoaded&render=explicit&hl=' + this.ViewOptions.Recaptcha.LanguageCode;
      }
    });
  }
}