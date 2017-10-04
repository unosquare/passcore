
import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';
import { MdSnackBar } from '@angular/material';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { ViewOptions } from './models/view-options.model';
import { Alerts } from './models/alerts.model';
import { Recaptcha } from './models/recaptcha.model';
import { ChangePasswordForm } from './models/change-password-form.model';
import { Result } from './models/error-data.model';
import { Error } from './models/error.model';
import { PasswordValidatior } from './passwordValidator';

import 'rxjs/add/operator/map';

const EMAIL_REGEX = /^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;

@Component({
  selector: 'app-root',
  templateUrl: '../views/change-password.html',
  styleUrls: ['../styles/app.change-password.css']
})
export class ChangePasswordComponent implements OnInit {
  // Form Controls
  FormGroup = new FormGroup({
    username : new FormControl('', [Validators.required, Validators.pattern(EMAIL_REGEX)]),
    currentPassword : new FormControl('', [Validators.required]),
    newPassword : new FormControl('', [Validators.required]),
    newPasswordVerify : new FormControl('', [Validators.required]),
  },  PasswordValidatior.MatchPassword);
  // Variables
  ViewOptions: ViewOptions;
  ErrorData: Result;
  Loading: boolean = false;
  ErrorAlertMessage : string = '';
  ShowSuccessAlert: boolean;
  ShowErrorAlert: boolean;
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
      duration: 5000,
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
    this.Loading = true;
    this.ShowSuccessAlert = false;
    this.ShowErrorAlert = false;
    this.http.post('api/password', this.FormData)
      .subscribe((response) => {
        this.Loading = false;
        this.FormData = Object.assign({}, this.EmptyFormData);
        this.ShowSuccessAlert = true;
      }, (error) => {
        this.Loading = false;
        if (this.ViewOptions.recaptcha.isEnabled == true) {
          grecaptcha.reset();
        }

        this.ErrorData = error.json() as Result;
        this.ErrorData.errors.map((errData, index) => {
          this.ErrorAlertMessage += errData.message + '. ';
        });
        this.openSnackBar(this.ErrorAlertMessage,'OK');
        this.ErrorAlertMessage = '';
      });
  }
}