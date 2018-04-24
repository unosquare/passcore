import { Alerts } from '../models/alerts.model';
import { ChangePasswordForm } from '../models/change-password-form.model';
import { DialogOverview } from '../dialog/app.dialog';
import { PasswordModel } from '../models/password.model';
import { PasswordStrength } from '../helpers/passwordStrength';
import { Recaptcha } from '../models/recaptcha.model';
import { ViewOptions } from '../models/view-options.model';
import { ActivatedRoute, Params } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { HttpClient, HttpClientModule, HttpErrorResponse } from '@angular/common/http';
import { MatDialog, MatSnackBar } from '@angular/material';
import { Subscription } from 'rxjs/Rx';
import { Title } from '@angular/platform-browser';
import { map } from 'rxjs/operators';

const emailRegex = /^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;

@Component({
  selector: 'app-root',
  templateUrl: './change-password.html',
  styleUrls: ['./app.change-password.css'],
  providers: [ChangePasswordComponent]
})
export class ChangePasswordComponent implements OnInit {

  // Properties
  color: string = 'warn';
  ErrorAlertMessage: string = '';
  FormData: PasswordModel;
  Loading: boolean = false;
  subscription: Subscription;
  value: number = 0;
  ViewOptions: ViewOptions;

  // Form Controls  
  FormGroup = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.pattern(emailRegex)]),
    currentPassword: new FormControl('', [Validators.required]),
    newPassword: new FormControl('', [Validators.required]),
    newPasswordVerify: new FormControl('', [Validators.required])
  }, Validators.compose([Validators.required, this.matchingPasswords]));

  // Constructor
  constructor(public http: HttpClient, public snackBar: MatSnackBar,
    public titleService: Title, public dialog: MatDialog, public r: ActivatedRoute) {
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

  // Password matching validator
  matchingPasswords(): ValidatorFn {
    let passwordInput = this.FormGroup.value('newPassword');
    let passwordConfirmationInput = this.FormGroup.value('newPasswordVerify');
    return (passwordInput.value !== passwordConfirmationInput.value) ?
      passwordConfirmationInput.setErrors({ notEquivalent: true }) :
      passwordConfirmationInput.setErrors({ notEquivalent: false })
  }

  // Angular init
  ngOnInit(): void {
    this.subscription = this.r.queryParams.subscribe((params: Params) => {
      let userId = params['userName'] || "";
      this.GetData(userId);
    });
  }

  // Progress bar for password strength
  changeProgressBar(strength: number) {
    this.value = strength;
    if (strength < 33) {
      this.color = 'warn';
    } else if (strength > 33 && strength < 66) {
      this.color = 'accent';
    } else {
      this.color = 'primary';
    }
  }

  // Uses MatSnackBar
  openSnackBar(message: string, action: string) {
    this.snackBar.open(message, action, {
      duration: 5000
    });
  }

  // Uses MatDialogRef
  openDialog(title: string, message: string) {
    let refDialog = this.dialog.open(DialogOverview, {
      width: '300px',
      data: { Title: title, Message: message }
    });
  }

  // Reset form
  clean(submited: string) {
    this.Loading = false;
    this.ErrorAlertMessage = '';
    this.color = 'warn';
    this.value = 0;

    if (submited === 'success') {
      this.FormGroup.reset();
    } else {
      for (let formControl in this.FormGroup.controls) {
        if (formControl !== 'username')
          this.FormGroup.controls[formControl].reset();
      }
    }

    if (this.ViewOptions.recaptcha.isEnabled) {
      grecaptcha.reset();
    }
  }

  // Get data from the form
  GetData(queryParam: string): void {
    this.FormData.Username = queryParam;
    this.http.get('api/password').subscribe(values => {
      this.ViewOptions = <ViewOptions>values;
      this.titleService.setTitle(this.ViewOptions.changePasswordTitle + " - " + this.ViewOptions.applicationTitle);
      if (this.ViewOptions.recaptcha.isEnabled) {
        this.FormGroup.addControl('reCaptcha', new FormControl('', [Validators.required]));
        const sp = document.createElement('script');
        sp.type = 'text/javascript';
        sp.async = true;
        sp.defer = true;
        sp.src = 'https://www.google.com/recaptcha/api.js?onload=vcRecaptchaApiLoaded&render=explicit&hl=' + this.ViewOptions.recaptcha.languageCode;
      }
    });
  }

  // Uses RecaptchaModule / RecaptchaFormsModule
  SetRecaptchaResponse(captchaResponse: string) {
    this.FormData.Recaptcha = captchaResponse;
  }

  // Form submission
  Submit() {
    this.Loading = true;
    this.http.post('api/password', this.FormData).subscribe(
      response => {
        this.openDialog(this.ViewOptions.alerts.successAlertTitle, this.ViewOptions.alerts.successAlertBody);
        this.clean('success');
      },
      (error: HttpErrorResponse) => {
          this.ErrorAlertMessage = error.message ? error.message : "Password Submission Error";
          this.openSnackBar(this.ErrorAlertMessage, 'OK');
          this.clean('error');
      }
    );
  }
}