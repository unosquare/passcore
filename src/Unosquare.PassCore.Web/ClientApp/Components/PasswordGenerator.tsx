import TextField from '@material-ui/core/TextField/TextField';
import * as React from 'react';

export const PasswordGenerator: React.FunctionComponent<any> = () => {

    //ToDo: call the BE to get the generated password

    //ToDo: While awaiting for the BE response, a loading icon should appear insted the TextField.
    return (
        <TextField
            disabled={true}
            label='New Password'
            value='testing password'
            type='Password'
            style={{
                height: '20px',
                marginBottom: '50px',
            }}
        />
    );
};
