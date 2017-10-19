import { AbstractControl } from '@angular/forms';

export default class PasswordValidator {

    static MatchPassword(ac: AbstractControl) {
        const password = ac.get('newPassword').value; // to get value in input tag
        const confirmPassword = ac.get('newPasswordVerify').value; // to get value in input tag

        if (password != confirmPassword) {
            ac.get('newPasswordVerify').setErrors({ MatchPassword: true });
        } else {
            return null;
        }
    }
}