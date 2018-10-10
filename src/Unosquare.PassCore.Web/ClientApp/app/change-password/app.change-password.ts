import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { Alerts } from '../models/alerts.model';
import { ChangePasswordForm } from '../models/change-password-form.model';
import { Component, OnInit } from '@angular/core';
import { DialogOverview } from '../dialog/app.dialog';
import { HttpClient } from '@angular/common/http';
import { MatDialog, MatSnackBar } from '@angular/material';
import { PasswordMatch } from '../helpers/passwordMatch';
import { PasswordModel } from '../models/password.model';
import { PasswordStrength } from '../helpers/passwordStrength';
import { Recaptcha } from '../models/recaptcha.model';
import { Title } from '@angular/platform-browser';
import { ViewOptions } from '../models/view-options.model';
import { ErrorsPasswordForm } from '../models/errors-password-form.model';
import { ValidationRegex } from '../models/validation-regex.model';

@Component({
    selector: 'app-root',
    templateUrl: './change-password.html',
    styleUrls: ['./app.change-password.css'],
    providers: [ChangePasswordComponent],
    viewProviders: [ViewOptions]
})
export class ChangePasswordComponent implements OnInit {

    // Constructor: parent "this" doesn't work here
    constructor(
        private http: HttpClient,
        private snackBar: MatSnackBar,
        private titleService: Title,
        private dialog: MatDialog,
        private r: ActivatedRoute
    ) { }

    // Properties
    color: string = 'warn';
    ErrorAlertMessage: string = '';
    FormData: PasswordModel;
    Loading: boolean = false;
    value: number = 0;
    ViewOptions: ViewOptions;

    // Form Controls
    FormGroup = new FormGroup({
        username: new FormControl('', [Validators.required]),
        currentPassword: new FormControl('', [Validators.required]),
        newPassword: new FormControl('', [Validators.required]),
        newPasswordVerify: new FormControl('', [Validators.required])
    }, PasswordMatch);

    // Angular "OnInit": happens only on first page load
    ngOnInit() {
        this.FormData = new PasswordModel;
        this.ViewOptions = new ViewOptions;
        this.ViewOptions.alerts = new Alerts;
        this.ViewOptions.recaptcha = new Recaptcha;
        this.ViewOptions.changePasswordForm = new ChangePasswordForm;
        this.ViewOptions.errorsPasswordForm = new ErrorsPasswordForm;
        this.ViewOptions.validationRegex = new ValidationRegex();
        this.r.queryParams.subscribe((params: Params) => {
            const userId = params['userName'] || '';
            this.GetData(userId);
        });
        this.FormGroup.valueChanges.subscribe(data => {
            if (data.newPassword != null)
                this.changeProgressBar(PasswordStrength.measureStrength(data.newPassword));
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
        this.dialog.open(DialogOverview, {
            width: '300px',
            height: '300px',
            data: { Title: title, Message: message }
        });
    }

    // Reset form
    clean(submitted: string) {
        this.Loading = false;
        this.ErrorAlertMessage = '';
        this.color = 'warn';
        this.value = 0;

        if (submitted === 'success') {
            this.FormGroup.reset();
        } else {
            for (let formControl in this.FormGroup.controls) {
                if (formControl !== 'username')
                    this.FormGroup.controls[formControl].reset();
            }
        }

        if (this.ViewOptions.recaptcha.siteKey !== '') {
            grecaptcha.reset();
        }
    }

    // Get data from the form
    GetData(queryParam: string) {
        this.FormData.Username = queryParam;
        this.ViewOptions = window['config'];
        this.titleService.setTitle(`${this.ViewOptions.changePasswordTitle} - ${this.ViewOptions.applicationTitle}`);

        if (this.ViewOptions.recaptcha.siteKey !== '') {
            this.FormGroup.addControl('reCaptcha', new FormControl('', [Validators.required]));
            const sp = document.createElement('script');
            sp.type = 'text/javascript';
            sp.async = true;
            sp.defer = true;
            sp.src = `https://www.google.com/recaptcha/api.js?onload=vcRecaptchaApiLoaded&render=explicit&hl=${this.ViewOptions.recaptcha.languageCode}`;
        }

        this.FormGroup.get('username')
            .setValidators(Validators.pattern(
                this.ViewOptions.useEmail
                    ? this.ViewOptions.validationRegex.emailRegex
                    : this.ViewOptions.validationRegex.usernameRegex));
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
            errorResponse => {
                this.ErrorAlertMessage = '';

                errorResponse.error.errors.forEach((error: any) => {
                    switch (error.errorCode) {
                        case 0:
                            return this.ErrorAlertMessage += error.message;
                        case 1:
                            return this.ErrorAlertMessage += this.ViewOptions.alerts.errorFieldRequired;
                        case 2:
                            return this.ErrorAlertMessage += this.ViewOptions.alerts.errorFieldMismatch;
                        case 3:
                            return this.ErrorAlertMessage += this.ViewOptions.alerts.errorInvalidUser;
                        case 4:
                            return this.ErrorAlertMessage += this.ViewOptions.alerts.errorInvalidCredentials;
                        case 5:
                            return this.ErrorAlertMessage += this.ViewOptions.alerts.errorCaptcha;
                        case 6:
                            return this.ErrorAlertMessage += this.ViewOptions.alerts.errorPasswordChangeNotAllowed;
                        case 7:
                            return this.ErrorAlertMessage += this.ViewOptions.alerts.errorInvalidDomain;
                        default:
                            return null;
                    }
                });
                this.openSnackBar(this.ErrorAlertMessage, 'OK');
                this.clean('error');
            }
        );
    }
}