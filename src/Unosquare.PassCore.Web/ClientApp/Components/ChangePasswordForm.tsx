import FormGroup from '@material-ui/core/FormGroup/FormGroup';
import * as React from 'react';
import { TextValidator } from 'uno-material-ui';
import { useStateForModel } from 'uno-react';
import { GlobalContext } from '../Provider/GlobalContext';
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

export const ChangePasswordForm: React.FunctionComponent<any> = ({
    submitData,
    toSubmitData,
    parentRef,
    onValidated,
}) => {

    const [fields, handleChange] = useStateForModel({ ...defaultState });

    const {
        changePasswordForm,
        errorsPasswordForm,
        useEmail,
        showPasswordMeter,
    } = React.useContext(GlobalContext);

    const {
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

    if (submitData) {
        toSubmitData(fields);
    }

    if (parentRef.current && parentRef.current.isFormValid) {
        parentRef.current.isFormValid().then((response: any) => onValidated(!response));
    }

    return (
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
                id='NewPassword'
                name='NewPassword'
                onChange={handleChange}
                type='password'
                validators={['required']}
                value={fields.NewPassword}
                style={{
                    height: '20px',
                    marginBottom: '6%',
                }}
                fullWidth={true}
                errorMessages={[
                    fieldRequired,
                ]}
            />
            {
                showPasswordMeter &&
                <PasswordStrengthBar
                    newPassword={fields.NewPassword}
                />
            }
            <div
                dangerouslySetInnerHTML={{ __html: newPasswordHelpblock }}
                style={{ font: '12px Roboto,Helvetica, Arial, sans-serif' }}
            />
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
                    margin: '7% 0 15% 0',
                }}
                fullWidth={true}
                errorMessages={[
                    fieldRequired,
                    passwordMatch, /// ToDo: need to set the password match validation
                ]}
            />
        </FormGroup>
    );
};
