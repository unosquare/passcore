import ChangePasswordComponent from './change-password/app.change-password';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const appRoutes: Routes = [
    { path: '', component: ChangePasswordComponent },
];

@NgModule({
    imports: [
        RouterModule.forRoot(appRoutes),


    ],
    exports: [
        RouterModule,

    ]
})
export class AppRoutingModule { }