
import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';
import { MdSnackBar } from '@angular/material';
import { FormControl, Validators } from '@angular/forms';

import { ViewOptions } from './models/view-options.model';
import { Alerts } from './models/alerts.model';
import { Recaptcha } from './models/recaptcha.model';
import { ChangePasswordForm } from './models/change-password-form.model';
import { Result } from './models/error-data.model';
import { Error } from './models/error.model';
import { PasswordValidatior } from './passwordValidator';

import 'rxjs/add/operator/map';

const EMAIL_REGEX = '/^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$/';

@Component({
  selector: 'app-root',
  templateUrl: '../views/change-password.html',
  styleUrls: ['../styles/app.change-password.css']
})
export class ChangePasswordComponent implements OnInit {

  // Form Controls
  username = new FormControl('', [Validators.required, Validators.pattern(EMAIL_REGEX)]);
  currentPassword = new FormControl('', [Validators.required]);
  newPassword = new FormControl('', [Validators.required]);
  newPasswordVerify = new FormControl('', [Validators.required]);
  // Variables
  ViewOptions: ViewOptions;
  ErrorData: Result;
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

  constructor(private http: Http, private snackBar: MdSnackBar) {
    this.ViewOptions = new ViewOptions;
    this.ViewOptions.alerts = new Alerts;
    this.ViewOptions.recaptcha = new Recaptcha;
    this.ViewOptions.changePasswordForm = new ChangePasswordForm;
  }

  ngOnInit(): void {
    this.GetData();
  }

  private openSnackBar(message: string, action: string) {
    this.snackBar.open(message, action, {
      duration: 30000,
    });
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

  Submit() {
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

        this.ErrorData = error.json() as Result;
        this.ErrorData.errors.map((errData, index) => {          
          switch(errData.errorType){
            case 0: // Success
              this.ErrorAlertMessage = this.ViewOptions.alerts.errorAlertBody + errData.message;
            break;
            case 1: // GeneralFailure
              this.ErrorAlertMessage = this.ViewOptions.alerts.errorAlertBody + errData.message;
            break;
            case 2: // FieldValidation
              switch(errData.errorCode){
                case 0: // Generic
                  this.ErrorAlertMessage = this.ViewOptions.alerts.errorAlertBody + errData.message;
                break;
                case 1: // FieldRequired
                  this.ErrorAlertMessage = this.ViewOptions.alerts.errorAlertBody + errData.message;
                break;
                case 2: // FieldMismatch
                  this.ErrorAlertMessage = this.ViewOptions.alerts.errorAlertBody + errData.message;
                break;
                case 3: // UserNotFound
                  this.ErrorAlertMessage = this.ViewOptions.alerts.errorAlertBody + errData.message;
                break;
                case 4: // InvalidCredentials
                  this.ErrorAlertMessage = this.ViewOptions.alerts.errorAlertBody + errData.message;
                break;
                case 5: // InvalidCaptcha
                  this.ErrorAlertMessage = this.ViewOptions.alerts.errorAlertBody + errData.message;
                break;
              }              
            break;
          }
          this.openSnackBar(this.ErrorAlertMessage,'OK');
        });
      });
  }
}