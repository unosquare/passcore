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
    BrowserAnimationsModule,
    BrowserModule,
    FormsModule,
    FlexLayoutModule,
    HttpClientModule,
    MaterialModule,
    MatDialogModule,
    MatSnackBarModule,
    RecaptchaModule.forRoot(),
    ReactiveFormsModule,
    RouterModule
  ],
  exports: [
  ]
})
export class AppModule { }