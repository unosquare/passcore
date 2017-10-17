import { Title } from '@angular/platform-browser';
import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';
import { MatSnackBar, MatDialog } from '@angular/material';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { ViewOptions } from '../models/view-options.model';
import { Alerts } from '../models/alerts.model';
import { Recaptcha } from '../models/recaptcha.model';
import { ChangePasswordForm } from '../models/change-password-form.model';
import { Result } from '../models/result-data.model';
import { Error } from '../models/error.model';
import { PasswordModel } from '../models/password.model';
import { DialogOverview } from '../dialog/app.dialog'


import { PasswordValidatior } from '../helpers/passwordValidator';
import { PasswordStrength } from '../helpers/passwordStrength';

import 'rxjs/add/operator/map';

const EMAIL_REGEX = /^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;

@Component({
  selector: 'app-root',
  templateUrl: './change-password.html',
  styleUrls: ['./app.change-password.css']
})
export class ChangePasswordComponent implements OnInit {
  // Form Controls
  FormGroup = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.pattern(EMAIL_REGEX)]),
    currentPassword: new FormControl('', [Validators.required]),
    newPassword: new FormControl('', [Validators.required]),
    newPasswordVerify: new FormControl('', [Validators.required])
  }, PasswordValidatior.MatchPassword);
  // Variables
  ViewOptions: ViewOptions;
  ResultData: Result;
  Loading: boolean = false;
  ErrorAlertMessage: string = '';
  FormData: PasswordModel;
  color: string = 'warn';
  value: number = 0;

  constructor(private http: Http, private snackBar: MatSnackBar,
    private titleService: Title, public dialog: MatDialog) {
    this.FormData = new PasswordModel;
    this.ViewOptions = new ViewOptions;
    this.ViewOptions.alerts = new Alerts;
    this.ViewOptions.recaptcha = new Recaptcha;
    this.ViewOptions.changePasswordForm = new ChangePasswordForm;
    this.FormGroup.valueChanges.subscribe(data => {
      if (data.newPassword != null)
        this.changeProgressBar(PasswordStrength.measureStrength(data.newPassword));
    });
  }

  ngOnInit(): void {
    this.GetData();
  }

  private changeProgressBar(strength: number) {
    this.value = strength;
    if (strength < 33) {
      this.color = 'warn';
    } else if (strength > 33 && strength < 66) {
      this.color = 'accent';
    } else {
      this.color = 'primary';
    }
  }

  private openSnackBar(message: string, action: string) {
    this.snackBar.open(message, action, {
      duration: 5000
    });
  }

  private openDialog(title: string, message: string) {
    let refDialog = this.dialog.open(DialogOverview, {
      width: '300px',
      data: { Title: title, Message: message }
    });
  }

  private clean(submited: string) {
    this.Loading = false;
    this.ErrorAlertMessage = '';
    this.color = 'warn';
    this.value = 0;

    if (submited == 'success') {
      this.FormGroup.reset();
    } else {
      for (let formControl in this.FormGroup.controls) {
        if (formControl != 'username')
          this.FormGroup.controls[formControl].reset();
      }
    }

    if (this.ViewOptions.recaptcha.isEnabled) {
      grecaptcha.reset();
    }
  }

  private GetData(): void {
    this.http.get('api/password').subscribe(values => {
      this.ViewOptions = values.json();
      this.titleService.setTitle(this.ViewOptions.applicationTitle);
      if (this.ViewOptions.recaptcha.isEnabled) {
        this.FormGroup.addControl('reCaptcha', new FormControl('', [Validators.required]));
        let sp = document.createElement('script'); sp.type = 'text/javascript'; sp.async = true; sp.defer = true;
        sp.src = 'https://www.google.com/recaptcha/api.js?onload=vcRecaptchaApiLoaded&render=explicit&hl=' + this.ViewOptions.recaptcha.languageCode;
      }
    });
  }

  private SetRecaptchaResponse(captchaResponse: string) {
    this.FormData.Recaptcha = captchaResponse;
  }

  Submit() {
    this.Loading = true;
    this.http.post('api/password', this.FormData)
      .subscribe((response) => {
        this.openDialog(this.ViewOptions.alerts.successAlertTitle, this.ViewOptions.alerts.successAlertBody);

        this.clean('success');
      }, (error) => {
        this.ResultData = error.json() as Result;
        this.ResultData.errors.map((errData, index) => {
          this.ErrorAlertMessage += errData.message;
        });
        this.openSnackBar(this.ErrorAlertMessage, 'OK');

        this.clean('error');
      });
  }
}