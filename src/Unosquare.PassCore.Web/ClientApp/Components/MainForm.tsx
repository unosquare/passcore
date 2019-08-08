import Button from '@material-ui/core/Button/Button';
import FormGroup from '@material-ui/core/FormGroup/FormGroup';
import Paper from '@material-ui/core/Paper/Paper';
import * as React from 'react';
import { TextValidator } from 'uno-material-ui';
import { useStateForModel, ValidatorForm } from 'uno-react';
import { GlobalContext } from '../Provider/GlobalContext';
import { PasswordStrengthBar } from './PasswordStrengthBar';
import { ReCaptchaComponent } from './ReCaptcha';

interface IChangePasswordFormInitialModel {
    CurrentPassword: string;
    NewPassword: string;
    NewPasswordVerify: string;
    Recaptcha: string;
    Username: string;
}

const defaultState: IChangePasswordFormInitialModel = {
    CurrentPassword: '',
    NewPassword: '',
    NewPasswordVerify: '',
    Recaptcha: '',
    Username: '',
};

export const MainForm: React.FunctionComponent<any> = () => {

    const {
        changePasswordForm,
        errorsPasswordForm,
        useEmail,
        showPasswordMeter,
    } = React.useContext(GlobalContext);

    const {
        changePasswordButtonLabel,
        currentPasswordHelpblock,
        currentPasswordLabel,
        newPasswordHelpblock,
        newPasswordLabel,
        newPasswordVerifyHelpblock,
        newPasswordVerifyLabel,
        usernameDefaultDomainHelperBlock,
        usernameHelpblock,
        usernameLabel,
    } = changePasswordForm;

    const {
        fieldRequired,
        passwordMatch,
        usernameEmailPattern,
        usernamePattern,
    } = errorsPasswordForm;

    const [fields, handleChange] = useStateForModel({ ...defaultState });
    const [token, setToken] = React.useState('');
    const [shouldReset, setShouldReset] = React.useState(1);
    //newPasswordHelpblock
    console.log('newPasswordHelpblock: ', newPasswordHelpblock);
    return (
        < Paper
            style={{
                borderRadius: '10px',
                height: '64.5%',
                marginTop: '3.5%',
                width: '34%',
                zIndex: 1,
            }}
            elevation={6}
        >
            <ValidatorForm
                autoComplete='off'
                instantValidate={true}
                onSubmit={() => { }}
            >
                <FormGroup
                    row={false}
                    style={{ width: '80%', marginLeft: '10%' }}
                >
                    <TextValidator
                        id='Username'
                        label={usernameLabel}
                        helperText={useEmail ? usernameHelpblock : usernameDefaultDomainHelperBlock}
                        name='Username'
                        onChange={handleChange}
                        validators={['required']} // ToDo: add the email validation if useEmail is active
                        value={fields.Username}
                        style={{
                            height: '20px',
                            marginBottom: '15%',
                        }}
                        fullWidth={true}
                        errorMessages={[
                            fieldRequired,
                            useEmail ? usernameEmailPattern : usernamePattern,
                        ]}
                    />
                    <TextValidator
                        label={currentPasswordLabel}
                        helperText={currentPasswordHelpblock}
                        id='CurrentPassword'
                        name='CurrentPassword'
                        onChange={handleChange}
                        type='password'
                        validators={['required']}
                        value={fields.CurrentPassword}
                        style={{
                            height: '20px',
                            marginBottom: '15%',
                        }}
                        fullWidth={true}
                        errorMessages={[
                            fieldRequired,
                        ]}
                    />
                    <TextValidator
                        label={newPasswordLabel}
                        helperText={newPasswordHelpblock}
                        id='NewPassword'
                        name='NewPassword'
                        onChange={handleChange}
                        type='password'
                        validators={['required']}
                        value={fields.NewPassword}
                        style={{
                            height: '20px',
                            marginBottom: '15%',
                        }}
                        fullWidth={true}
                        errorMessages={[
                            fieldRequired,
                        ]}
                    />
                    {
                        // showPasswordMeter &&
                        // <PasswordStrengthBar
                        //     newPassword={fields.newPassword}
                        // // reset={}
                        // />
                    }
                    <TextValidator
                        label={newPasswordVerifyLabel}
                        helperText={newPasswordVerifyHelpblock}
                        id='NewPasswordVerify'
                        name='NewPasswordVerify'
                        onChange={handleChange}
                        type='password'
                        validators={['required']}
                        value={fields.NewPasswordVerify}
                        style={{
                            height: '20px',
                            marginBottom: '15%',
                        }}
                        fullWidth={true}
                        errorMessages={[
                            fieldRequired,
                            passwordMatch, /// ToDo: need to set the password match validation
                        ]}
                    />
                </FormGroup>
                {/* <ReCaptchaComponent
                    setToken={setToken}
                    shouldReset={shouldReset}
                /> */}
                <Button
                    type='submit'
                    variant='contained'
                    color='primary'
                    style={{
                        marginLeft: '35.5%',
                        marginTop: '10%',
                    }}
                >
                    {changePasswordButtonLabel}
                </Button>
            </ValidatorForm>
        </Paper >
    );
};
