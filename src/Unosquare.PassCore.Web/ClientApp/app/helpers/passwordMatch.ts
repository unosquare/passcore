import { ValidatorFn, AbstractControl } from "@angular/forms";

// Password matching validator

export function PasswordMatch(ac: AbstractControl): ValidatorFn {
    const passwordInput = ac.get('newPassword');
    const passwordConfirmationInput = ac.get('newPasswordVerify');

    if (passwordInput.value !== passwordConfirmationInput.value) {
        passwordConfirmationInput.setErrors({ notEquivalent: true });
    } else {
        return null;
    }
}