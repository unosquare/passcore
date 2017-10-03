import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';

import 'rxjs/add/operator/map';

@Component({
  selector: 'app-root',
  templateUrl: '../views/change-password.html'
})
export class ChangePasswordComponent implements OnInit {
  ViewOptions: ViewOptions;
  constructor(private http: Http) { this.ViewOptions = new ViewOptions; }

  ngOnInit(): void {
    this.GetData();
  }

  private GetData(): void{
    this.http.get('api/password').subscribe(values => {
      this.ViewOptions = values.json();
      if (this.ViewOptions.recaptcha.isEnabled == true) {
        let sp = document.createElement('script'); sp.type = 'text/javascript'; sp.async = true; sp.defer = true;
        sp.src = 'https://www.google.com/recaptcha/api.js?onload=vcRecaptchaApiLoaded&render=explicit&hl=' + this.ViewOptions.recaptcha.languageCode;
      }
    });
  }
}

export class ViewOptions {
  public alerts: Alerts;
  public applicationTitle: string;
  public changePasswordForm: ChangePasswordForm;
  public changePasswordTitle: string;
  public errorMessages: string[];
  public recaptcha: Recaptcha;
  public showPasswordMeter: boolean;
}

export class Recaptcha{
  public isEnabled: boolean;
  public languageCode: string;
  public siteKey: string;
}

export class ChangePasswordForm{
  public changePasswordButtonLabel:string;
  public currentPasswordHelpblock:string;
  public currentPasswordLabel:string;
  public currentPasswordPlaceholder:string;
  public helpText:string;
  public helpTitle:string;
  public newPasswordHelpblock:string;
  public newPasswordLabel:string;
  public newPasswordPlaceholder:string;
  public newPasswordVerifyHelpblock:string;
  public newPasswordVerifyLabel:string;
  public newPasswordVerifyPlaceholder:string;
  public usernameHelpblock:string;
  public usernameLabel:string;
  public usernamePlaceholder:string;
}
export class Alerts {
  public errorAlertBody: string;
  public errorAlertTitle: string;
  public errorInvalidCredentials: string;
  public errorPasswordChangeNotAllowed :string;
  public successAlertBody: string;
  public successAlertTitle: string;
}