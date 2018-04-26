import { ValidatorFn, AbstractControl } from "@angular/forms";

// Password matching validator

export function PasswordMatch(): ValidatorFn {
    return (ac: AbstractControl) => {
            let passwordInput = ac.value('newPassword');
            let passwordConfirmationInput = ac.value('newPasswordVerify');
            return (passwordInput.value !== passwordConfirmationInput.value) ?
                passwordConfirmationInput.setErrors({ notEquivalent: true }) :
                passwordConfirmationInput.setErrors({ notEquivalent: false })
        }
}