import { AppRoutingModule } from './app.routing-module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';
import { ChangePasswordComponent } from './change-password/app.change-password';
import { DialogOverview } from './dialog/app.dialog';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FooterComponent } from './footer/app.footer';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MatDialogModule } from '@angular/material/dialog';
import { MaterialModule } from './app.material-module';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { NgModule } from '@angular/core';
import { RecaptchaModule, RECAPTCHA_LANGUAGE } from 'ng-recaptcha';
import { RecaptchaFormsModule } from 'ng-recaptcha/forms';
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
    BrowserAnimationsModule,
    BrowserModule,
    FormsModule,
    FlexLayoutModule,
    HttpClientModule,
    MaterialModule,
    MatDialogModule,
    MatSnackBarModule,
    RecaptchaModule.forRoot(),
    RecaptchaFormsModule,
    ReactiveFormsModule,
    RouterModule
  ],
  exports: [ ],
  providers: [
    { provide: RECAPTCHA_LANGUAGE, useFactory: () => window['lang'] },
]
})
export class AppModule { }