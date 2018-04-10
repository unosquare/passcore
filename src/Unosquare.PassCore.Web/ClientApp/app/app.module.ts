import ChangePasswordComponent from './change-password/app.change-password';
import DialogOverview from './dialog/app.dialog';
import FooterComponent from './footer/app.footer';
import { AppRoutingModule } from './app.routing-module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { MaterialModule } from './app.material-module';
import { NgModule } from '@angular/core';
import { RecaptchaFormsModule } from 'ng-recaptcha/forms';
import { RecaptchaModule } from 'ng-recaptcha';

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
