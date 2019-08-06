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
                marginTop: '70px',
            }}
        >
            <ValidatorForm
                autoComplete='off'
                instantValidate={true}
            // onSubmit={onSubmitForm}
            >
                <TextValidator
                    id='userName'
                    //label='Email Active Directory Attribute'
                    name='userName'
                    value={fields.userName}
                    onChange={handleChange}
                    validators={['required'}
                    errorMessages={[
                        'This field is required',
                    ]}
                />
                
            </ValidatorForm>
        </Paper >
    );
};
