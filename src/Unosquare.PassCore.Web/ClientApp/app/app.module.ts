import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { FlexLayoutModule } from "@angular/flex-layout";

import { RecaptchaModule } from 'ng-recaptcha';
import { RecaptchaFormsModule } from 'ng-recaptcha/forms';
import { AppRoutingModule } from './app.routing-module';

import { MaterialModule } from './app.material-module';
import ChangePasswordComponent  from './change-password/app.change-password';
import FooterComponent from './footer/app.footer';
import DialogOverview from './dialog/app.dialog';

@NgModule({
  declarations: [
    ChangePasswordComponent,
    FooterComponent,
    DialogOverview
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    MaterialModule,
    HttpModule,
    FlexLayoutModule,
    RecaptchaModule.forRoot(),
    RecaptchaFormsModule,
    ReactiveFormsModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [ChangePasswordComponent],
  entryComponents: [DialogOverview]
})
export class AppModule { }
