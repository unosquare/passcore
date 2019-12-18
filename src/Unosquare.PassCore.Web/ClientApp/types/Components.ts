export interface IChangePasswordFormInitialModel {
    CurrentPassword: string;
    NewPassword: string;
    NewPasswordVerify: string;
    Recaptcha: string;
    Username: string;
}

export interface IChangePasswordFormProps {
    submitData: boolean;
    toSubmitData: any;
    parentRef: any;
    onValidated: any;
    shouldReset: boolean;
    changeResetState: any;
    setReCaptchaToken: any;
    ReCaptchaToken: string;
}

export interface IPasswordGenProps {
    value: string;
    setValue: any;
}
