import Button from '@material-ui/core/Button/Button';
import Paper from '@material-ui/core/Paper/Paper';
import * as React from 'react';
import { ValidatorForm } from 'uno-react';
import { ChangePasswordDialog } from '../Dialogs/ChangePasswordDialog';
import { GlobalActionsContext, GlobalContext } from '../Provider/GlobalContext';
import { ChangePasswordForm } from './ChangePasswordForm';
import { ReCaptchaComponent } from './ReCaptcha';

export const ChangePassword: React.FunctionComponent<any> = ({ }) => {
    const [disabled, setDisabled] = React.useState(true);
    const [submit, setSubmit] = React.useState(false);
    const [dialogIsOpen, setDialog] = React.useState(false);
    const [token, setToken] = React.useState('');
    const validatorFormRef = React.useRef(null);
    const { changePassword } = React.useContext(GlobalActionsContext);
    const { changePasswordForm, recaptcha } = React.useContext(GlobalContext);
    const { changePasswordButtonLabel } = changePasswordForm;
    const { siteKey } = recaptcha;

    const onSubmitValidatorForm = () => setSubmit(true);

    const toSubmitData = (formData: any) =>
        changePassword({ ...formData, Recaptcha: token }).then((response: any) => {
            //ToDo: use the response to open the dialog if the change password was successful.
            setSubmit(false);
        });

    const onCloseDialog = () => setDialog(false);

    return (
        <React.Fragment>
            < Paper
                style={{
                    borderRadius: '10px',
                    height: '600px',
                    marginTop: '3.5%',
                    width: '34%',
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
                    />
                    <Button
                        type='submit'
                        variant='contained'
                        color='primary'
                        disabled={disabled}
                        style={{
                            marginLeft: '35.5%',
                            marginTop: '10%',
                        }}
                    >
                        {changePasswordButtonLabel}
                    </Button>
                </ValidatorForm>
                {
                    (siteKey && siteKey !== '') &&
                    <ReCaptchaComponent
                        siteKey={siteKey}
                        setToken={setToken}
                    />
                }
            </Paper >
            <ChangePasswordDialog
                open={dialogIsOpen}
                onClose={onCloseDialog}
            />
        </React.Fragment>
    );
};
