import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import ChangePasswordComponent from './change-password/app.change-password';


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