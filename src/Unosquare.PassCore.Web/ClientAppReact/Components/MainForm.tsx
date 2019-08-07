import Button from '@material-ui/core/Button/Button';
import FormGroup from '@material-ui/core/FormGroup/FormGroup';
import Paper from '@material-ui/core/Paper/Paper';
import * as React from 'react';
import { TextValidator } from 'uno-material-ui';
import { useStateForModel, ValidatorForm } from 'uno-react';
import { GlobalContext } from '../Provider/GlobalContext';
import { ReCaptchaComponent } from './ReCaptcha';
import { PasswordStrengthBar } from './PasswordStrengthBar';

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

    const { ChangePasswordForm,
        ErrorsPasswordForm,
        UseEmail,
        ShowPasswordMeter,
    } = React.useContext(GlobalContext);

    const {
        ChangePasswordButtonLabel,
        CurrentPasswordHelpblock,
        CurrentPasswordLabel,
        NewPasswordHelpblock,
        NewPasswordLabel,
        NewPasswordVerifyHelpblock,
        NewPasswordVerifyLabel,
        UsernameDefaultDomainHelperBlock,
        UsernameHelpblock,
        UsernameLabel,
    } = ChangePasswordForm;

    const {
        FieldRequired,
        PasswordMatch,
        UsernameEmailPattern,
        UsernamePattern,
    } = ErrorsPasswordForm;

    const [fields, handleChange] = useStateForModel({ ...defaultState });
    const [token, setToken] = React.useState('');
    const [shouldReset, setShouldReset] = React.useState(1);

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
                        label={UsernameLabel}
                        helperText={UseEmail ? UsernameHelpblock : UsernameDefaultDomainHelperBlock}
                        name='Username'
                        onChange={handleChange}
                        validators={['required']} //ToDo: add the email validation if UseEmail is active
                        value={fields.Username}
                        style={{
                            height: '20px',
                            marginBottom: '15%',
                        }}
                        fullWidth={true}
                        errorMessages={[
                            FieldRequired,
                            UseEmail ? UsernameEmailPattern : UsernamePattern,
                        ]}
                    />
                    <TextValidator
                        label={CurrentPasswordLabel}
                        helperText={CurrentPasswordHelpblock}
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
                            FieldRequired,
                        ]}
                    />
                    <TextValidator
                        label={NewPasswordLabel}
                        helperText={NewPasswordHelpblock}
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
                            FieldRequired,
                        ]}
                    />
                    {
                        ShowPasswordMeter &&
                        <PasswordStrengthBar
                            newPassword={fields.newPassword}
                        //reset={}
                        />
                    }
                    <TextValidator
                        label={NewPasswordVerifyLabel}
                        helperText={NewPasswordVerifyHelpblock}
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
                            FieldRequired,
                            PasswordMatch ///ToDo: need to set the password match validation
                        ]}
                    />
                </FormGroup>
                <ReCaptchaComponent
                    setToken={setToken}
                    shouldReset={shouldReset}
                />
                <Button
                    type='submit'
                    variant='contained'
                    color='primary'
                    style={{
                        marginLeft: '35.5%',
                        marginTop: '10%',
                    }}
                >
                    {ChangePasswordButtonLabel}
                </Button>
            </ValidatorForm>
        </Paper >
    );
};
