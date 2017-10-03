import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';

import { ViewOptions } from './app.view-options';
import { Alerts } from  './app.alerts';
import { Recaptcha } from './app.recaptcha';
import { ChangePasswordForm } from './app.change-password-form';

import 'rxjs/add/operator/map';

@Component({
  selector: 'app-root',
  templateUrl: '../views/change-password.html'
})
export class ChangePasswordComponent implements OnInit {
  ViewOptions: ViewOptions;
  ShowSuccessAlert;
  ShowErrorAlert;
  ErrorAlertMessage;
  FormData = {
    Username: '',
    CurrentPassword:'',
    NewPassword:'',
    NewPasswordVerify:'',
    Recaptcha:''
  };

  constructor(private http: Http) { 
    this.ViewOptions = new ViewOptions;
    this.ViewOptions.alerts = new Alerts;
    this.ViewOptions.recaptcha = new Recaptcha;
    this.ViewOptions.changePasswordForm = new ChangePasswordForm;
  }

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

  EmptyFormData = Object.assign({}, this.FormData);
  
  SetRecaptchaResponse(response: any){
      this.FormData.Recaptcha = response;
  }

  ClearRecaptchaResponse(){
    this.FormData.Recaptcha = '';
  }

  Submit(){
    this.ShowSuccessAlert = false;
    this.ShowErrorAlert = false;
    this.ErrorAlertMessage = '';
    this.http.post('api/password', this.FormData)
      .subscribe((response: any) => {
        this.FormData = Object.assign({}, this.EmptyFormData);
        this.ShowSuccessAlert = true;
      }, (error: any) => {
        if(this.ViewOptions.recaptcha.isEnabled == true){
          grecaptcha.reset();
        }

        error.data.Errors.map((index, errorData) => {
          if(errorData.ErrorType == 1){
            this.ShowErrorAlert = true;
            if(errorData.ErrorCode == 0){
              this.ErrorAlertMessage = this.ViewOptions.alerts.errorAlertBody + errorData.Message;
            }
            else{
              this.ErrorAlertMessage = this.ViewOptions.alerts.errorAlertBody + this.ViewOptions.errorMessages[errorData.ErrorCode];
            }
          }
          else if(errorData.ErrorType == 2){
            //todo erros in view
          }
        });
      });
  }
}