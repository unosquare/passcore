import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';

import { ViewOptions } from './models/view-options.model';
import { Alerts } from './models/alerts.model';
import { Recaptcha } from './models/recaptcha.model';
import { ChangePasswordForm } from './models/change-password-form.model';
import { Errors } from './models/error-data.model';
import { error } from './models/error.model';

import 'rxjs/add/operator/map';

@Component({
  selector: 'app-root',
  templateUrl: '../views/change-password.html'
})
export class ChangePasswordComponent implements OnInit {
  ViewOptions: ViewOptions;
  ErrorData: Errors;
  Form;
  ShowSuccessAlert;
  ShowErrorAlert;
  ErrorAlertMessage;
  FormData = {
    Username: '',
    CurrentPassword: '',
    NewPassword: '',
    NewPasswordVerify: '',
    Recaptcha: ''
  };

  constructor(private http: Http) {
    this.ViewOptions = new ViewOptions;
    this.ViewOptions.alerts = new Alerts;
    this.ViewOptions.recaptcha = new Recaptcha;
    this.ViewOptions.changePasswordForm = new ChangePasswordForm;
    this.ErrorData = new Errors;
    this.ErrorData.errors = new Array<error>();
  }

  ngOnInit(): void {
    this.GetData();
  }

  private GetData(): void {
    this.http.get('api/password').subscribe(values => {
      this.ViewOptions = values.json();
      if (this.ViewOptions.recaptcha.isEnabled == true) {
        let sp = document.createElement('script'); sp.type = 'text/javascript'; sp.async = true; sp.defer = true;
        sp.src = 'https://www.google.com/recaptcha/api.js?onload=vcRecaptchaApiLoaded&render=explicit&hl=' + this.ViewOptions.recaptcha.languageCode;
      }
    });
  }

  EmptyFormData = Object.assign({}, this.FormData);

  SetRecaptchaResponse(response) {
    this.FormData.Recaptcha = response;
  }

  ClearRecaptchaResponse() {
    this.FormData.Recaptcha = '';
  }

  resolved(captchaResponse: string) {
    console.log(`Resolved captcha with response ${captchaResponse}:`);
  }

  Submit(){
    this.ShowSuccessAlert = false;
    this.ShowErrorAlert = false;
    this.ErrorAlertMessage = '';
    this.http.post('api/password', this.FormData)
      .subscribe((response) => {
        this.FormData = Object.assign({}, this.EmptyFormData);
        this.ShowSuccessAlert = true;
      }, (error) => {
        if (this.ViewOptions.recaptcha.isEnabled == true) {
          grecaptcha.reset();
        }

        this.ErrorData = error.json() as Errors;
        this.ErrorData.errors.map((errData, index) => {
          if (errData.errorType == 1) {
            this.ShowErrorAlert = true;
            if (errData.errorCode == 0) {
              this.ErrorAlertMessage = this.ViewOptions.alerts.errorAlertBody + errData.message;
            }
            else {
              this.ErrorAlertMessage = this.ViewOptions.alerts.errorAlertBody + this.ViewOptions.errorMessages[errData.errorCode];
            }
          }
          else if (errData.errorType == 2) {
           this.Form[errData.fieldName].Validation.HasError = true;
           this.Form[errData.fieldName].Validation.ErrorMessage = this.ViewOptions.errorMessages[errData.errorCode];
          }
        });
      });
  }
}