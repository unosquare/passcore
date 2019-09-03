import IconButton from '@material-ui/core/IconButton/IconButton';
import InputAdornment from '@material-ui/core/InputAdornment/InputAdornment';
import TextField from '@material-ui/core/TextField/TextField';
import Save from '@material-ui/icons/Save';
import Visibility from '@material-ui/icons/Visibility';
import VisibilityOff from '@material-ui/icons/VisibilityOff';
import * as React from 'react';
import { SnackbarContext } from '../Provider/GlobalContext';

export const PasswordGenerator: React.FunctionComponent<any> = () => {
    const { sendMessage } = React.useContext(SnackbarContext);
    const [visibility, setVisibility] = React.useState(false);
    // ToDo: call the BE to get the generated password
    // ToDo: While awaiting for the BE response, a loading icon should appear insted the TextField.
    const onMouseVisibility = () => setVisibility(!visibility);

    const copyPassword = () => {
        navigator.clipboard.writeText('testing write to clipboard'); // ToDo: replace with the value returned from the B.E.
        sendMessage('Password copied');
    };

    return (
        <TextField
            id='generatedPassword'
            disabled={true}
            label='New Password'
            value='testing password'
            type={visibility ? 'text' : 'Password'}
            style={{
                height: '20px',
                margin: '30px 0 30px 0',
            }}
            InputProps={{
                endAdornment:
                    <InputAdornment position='end'>
                        <IconButton
                            aria-label='Toggle password visibility'
                            onMouseDown={onMouseVisibility}
                            onMouseUp={onMouseVisibility}
                            tabIndex={-1}

                        >
                            {
                                visibility ? <Visibility /> : <VisibilityOff />
                            }
                        </IconButton>
                        <IconButton
                            aria-label='Copy password to clipboard'
                            onClick={copyPassword}
                            tabIndex={-1}
                        >
                            <Save />
                        </IconButton>
                    </InputAdornment>,
            }}
        />
    );
};
