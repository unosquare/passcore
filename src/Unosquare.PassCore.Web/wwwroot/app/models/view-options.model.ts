import { Alerts } from './alerts.model';
import { ChangePasswordForm } from './change-password-form.model';
import { Recaptcha } from "./recaptcha.model";

export class ViewOptions {
    public alerts: Alerts;
    public applicationTitle: string;
    public changePasswordForm: ChangePasswordForm;
    public changePasswordTitle: string;
    public errorMessages: string[];
    public recaptcha: Recaptcha;
    public showPasswordMeter: boolean;
  }