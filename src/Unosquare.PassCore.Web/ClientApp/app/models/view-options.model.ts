import { Alerts } from './alerts.model';
import { ChangePasswordForm } from './change-password-form.model';
import { Recaptcha } from './recaptcha.model';
import { ErrorsPasswordForm } from './errors-password-form.model';
import { ValidationRegex } from './validation-regex.model';

export class ViewOptions {
    alerts: Alerts;
    applicationTitle: string;
    changePasswordForm: ChangePasswordForm;
    changePasswordTitle: string;
    recaptcha: Recaptcha;
    showPasswordMeter: boolean;
    useEmail: boolean;
    errorsPasswordForm: ErrorsPasswordForm;
    validationRegex: ValidationRegex;
}