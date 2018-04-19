import { ChangePasswordComponent } from './change-password/app.change-password';
import { DialogOverview } from './dialog/app.dialog';
import { FooterComponent } from './footer/app.footer';
import { AppRoutingModule } from './app.routing-module';
// import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';
// import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MaterialModule } from './app.material-module';
import { NgModule } from '@angular/core';
import { RecaptchaFormsModule } from 'ng-recaptcha/forms';
import { RecaptchaModule } from 'ng-recaptcha';
import { RouterModule } from '@angular/router';

@NgModule({
  bootstrap: [ChangePasswordComponent],
  declarations: [
    ChangePasswordComponent,
    DialogOverview,
    FooterComponent
  ],
  entryComponents: [DialogOverview],
  imports: [
    AppRoutingModule,
    // BrowserAnimationsModule,
    BrowserModule,
    FormsModule,
    // FlexLayoutModule,
    HttpClientModule,
    MaterialModule,
    RecaptchaModule.forRoot(),
    RecaptchaFormsModule,
    ReactiveFormsModule,
    RouterModule
  ],
})
export class AppModule { }