import Button from '@material-ui/core/Button/Button';
import Paper from '@material-ui/core/Paper/Paper';
import * as React from 'react';
import { ValidatorForm } from 'uno-react';
import { ChangePasswordDialog } from '../Dialogs/ChangePasswordDialog';
import { GlobalContext, SnackbarContext } from '../Provider/GlobalContext';
import { fetchRequest } from '../Utils/FetchRequest';
import { ChangePasswordForm } from './ChangePasswordForm';
import { ReCaptcha } from './ReCaptcha';

export const ChangePassword: React.FunctionComponent<any> = ({ }) => {
    const [disabled, setDisabled] = React.useState(true);
    const [submit, setSubmit] = React.useState(false);
    const [dialogIsOpen, setDialog] = React.useState(false);
    const [token, setToken] = React.useState('');
    const validatorFormRef = React.useRef(null);
    const { alerts, changePasswordForm, recaptcha, validationRegex } = React.useContext(GlobalContext);
    const { changePasswordButtonLabel } = changePasswordForm;
    const { sendMessage } = React.useContext(SnackbarContext);
    const [shouldReset, setReset] = React.useState(false);

    const onSubmitValidatorForm = () => setSubmit(true);

    const toSubmitData = (formData: any) => {
        fetchRequest(
            'api/password',
            'POST',
            JSON.stringify({ ...formData, Recaptcha: token }),
        ).then((response: any) => {
            setSubmit(false);
            if (response.errors && response.errors.length) {
                let errorAlertMessage = '';
                response.errors.forEach((error: any) => {
                    switch (error.errorCode) {
                        case 0:
                            errorAlertMessage += error.message;
                            break;
                        case 1:
                            errorAlertMessage += alerts.errorFieldRequired;
                            break;
                        case 2:
                            errorAlertMessage += alerts.errorFieldMismatch;
                            break;
                        case 3:
                            errorAlertMessage += alerts.errorInvalidUser;
                            break;
                        case 4:
                            errorAlertMessage += alerts.errorInvalidCredentials;
                            break;
                        case 5:
                            errorAlertMessage += alerts.errorCaptcha;
                            break;
                        case 6:
                            errorAlertMessage += alerts.errorPasswordChangeNotAllowed;
                            break;
                        case 7:
                            errorAlertMessage += alerts.errorInvalidDomain;
                            break;
                        case 8:
                            errorAlertMessage += alerts.errorConnectionLdap;
                            break;
                        case 9:
                            errorAlertMessage += alerts.errorComplexPassword;
                            break;
                        case 10:
                            errorAlertMessage += alerts.errorScorePassowrd;
                            break;
                    }
                });

                sendMessage(errorAlertMessage, 'error');
                return;
            }

            setDialog(true);
        });
    };

    const onCloseDialog = () => {
        setDialog(false);
        setReset(true);
    };

    const marginButton = recaptcha.siteKey && recaptcha.siteKey !== '' ? '25px 0 0 180px' : '100px 0 0 180px';

    ValidatorForm.addValidationRule('isUserName',
        (value: string) => new RegExp(validationRegex.usernameRegex).test(value));
    ValidatorForm.addValidationRule('isUserEmail',
        (value: string) => new RegExp(validationRegex.emailRegex).test(value));
    ValidatorForm.addValidationRule('isPasswordMatch',
        (value: string, comparedValue: any) => value === comparedValue);

    return (
        <React.Fragment>
            <Paper
                style={{
                    borderRadius: '10px',
                    height: '550px',
                    marginTop: '75px',
                    width: '650px',
                    zIndex: 1,
                }}
                elevation={6}
            >
                <ValidatorForm
                    ref={validatorFormRef}
                    autoComplete='off'
                    instantValidate={true}
                    onSubmit={onSubmitValidatorForm}
                >
                    <ChangePasswordForm
                        submitData={submit}
                        toSubmitData={toSubmitData}
                        parentRef={validatorFormRef}
                        onValidated={setDisabled}
                        shouldReset={shouldReset}
                        changeResetState={setReset}
                    />
                    {
                        (recaptcha.siteKey && recaptcha.siteKey !== '' &&
                            (
                                <ReCaptcha
                                    setToken={setToken}
                                    shouldReset={false}
                                />

                            )
                        )
                    }
                    <Button
                        type='submit'
                        variant='contained'
                        color='primary'
                        disabled={disabled}
                        style={{
                            margin: marginButton,
                            width: '240px',
                        }}
                    >
                        {changePasswordButtonLabel}
                    </Button>
                </ValidatorForm>
            </Paper>
            <ChangePasswordDialog
                open={dialogIsOpen}
                onClose={onCloseDialog}
            />
        </React.Fragment>
    );
};
