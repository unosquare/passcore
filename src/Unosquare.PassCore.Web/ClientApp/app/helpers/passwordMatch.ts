import { Validator, ValidatorFn, FormGroup } from '@angular/forms';

// Password matching validator

export class PasswordMatch implements Validator {

    validator: ValidatorFn;

    constructor() {
        this.validator = this.matchingPasswords();
    }
    validate(g: FormGroup) {
        return this.validator(g);
    }
    matchingPasswords(): ValidatorFn {
        return (g: FormGroup) => {
            let passwordInput = g.value('newPassword');
            let passwordConfirmationInput = g.value('newPasswordVerify');
            return (passwordInput.value !== passwordConfirmationInput.value) ?
                passwordConfirmationInput.setErrors({ notEquivalent: true }) :
                passwordConfirmationInput.setErrors({ notEquivalent: false })
        }
    }
}