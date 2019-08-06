import Button from '@material-ui/core/Button/Button';
import FormGroup from '@material-ui/core/FormGroup/FormGroup';
import Paper from '@material-ui/core/Paper/Paper';
import * as React from 'react';
import { TextValidator } from 'uno-material-ui';
import { useStateForModel, ValidatorForm } from 'uno-react';

interface IChangePasswordFormInitialModel {
    confirmationPassword: string;
    currentPassword: string;
    newPassword: string;
    userName: string;
}

const defaultState: IChangePasswordFormInitialModel = {
    confirmationPassword: '',
    currentPassword: '',
    newPassword: '',
    userName: '',
};

export const MainForm: React.FunctionComponent<any> = () => {
    const [fields, handleChange] = useStateForModel({ ...defaultState });

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
                        id='userName'
                        label='Username'
                        helperText='Your organization`s email address'
                        name='userName'
                        margin='dense'
                        onChange={handleChange}
                        validators={['required']}
                        value={fields.userName}
                        style={{
                            height: '20px',
                            marginBottom: '15%',
                        }}
                        fullWidth={true}
                        errorMessages={[
                            'This field is required',
                        ]}
                    />
                    <TextValidator
                        helperText='Enter your current password'
                        id='currentPassword'
                        name='currentPassword'
                        onChange={handleChange}
                        type='password'
                        validators={['required']}
                        value={fields.currentPassword}
                        style={{
                            height: '20px',
                            marginBottom: '15%',
                        }}
                        fullWidth={true}
                        errorMessages={[
                            'This field is required',
                        ]}
                    />
                    <TextValidator
                        helperText='Enter a strong password. You can use this tool to help you create one; use the XKCD (random sep, pad digit), or NTLM, options.'
                        id='newPassword'
                        name='newPassword'
                        onChange={handleChange}
                        type='password'
                        validators={['required']}
                        value={fields.newPassword}
                        style={{
                            height: '20px',
                            marginBottom: '15%',
                        }}
                        fullWidth={true}
                        errorMessages={[
                            'This field is required',
                        ]}
                    />
                    <TextValidator
                        helperText='Enter your new password again'
                        id='confirmationPassword'
                        label='Re-enter New Password'
                        name='confirmationPassword'
                        onChange={handleChange}
                        type='password'
                        validators={['required']}
                        value={fields.confirmationPassword}
                        style={{
                            height: '20px',
                            marginBottom: '15%',
                        }}
                        fullWidth={true}
                        errorMessages={[
                            'This field is required',
                        ]}
                    />
                </FormGroup>
                <Button
                    type='submit'
                    variant='contained'
                    color='primary'
                    style={{
                        marginLeft: '35.5%',
                        marginTop: '10%',
                    }}
                >
                    Change Password
                </Button>
            </ValidatorForm>
        </Paper >
    );
};
