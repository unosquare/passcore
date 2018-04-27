import { ValidatorFn, AbstractControl } from "@angular/forms";

// Password matching validator

export function PasswordMatch(ac: AbstractControl): ValidatorFn {
    let passwordInput = ac.get('newPassword');
    let passwordConfirmationInput = ac.get('newPasswordVerify');
    if (passwordInput.value !== passwordConfirmationInput.value) {
        passwordConfirmationInput.setErrors({ notEquivalent: true })
    } else {
        return null;
    }
}