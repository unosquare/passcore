import { Alerts } from './app.alerts';
import { ChangePasswordForm } from './app.change-password-form';
import { Recaptcha } from "./app.recaptcha";

export class ViewOptions {
    public alerts: Alerts;
    public applicationTitle: string;
    public changePasswordForm: ChangePasswordForm;
    public changePasswordTitle: string;
    public errorMessages: string[];
    public recaptcha: Recaptcha;
    public showPasswordMeter: boolean;
  }