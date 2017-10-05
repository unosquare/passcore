import { AbstractControl } from '@angular/forms';
export class PasswordValidatior {

    static MatchPassword(AC: AbstractControl) {
        let password = AC.get('newPassword').value; // to get value in input tag
        let confirmPassword = AC.get('newPasswordVerify').value; // to get value in input tag
        if (password != confirmPassword) {
            AC.get('newPasswordVerify').setErrors({ MatchPassword: true });
        } else {
            return null;
        }
    }
}