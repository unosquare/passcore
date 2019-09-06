import IconButton from '@material-ui/core/IconButton/IconButton';
import InputAdornment from '@material-ui/core/InputAdornment/InputAdornment';
import TextField from '@material-ui/core/TextField/TextField';
import FileCopy from '@material-ui/icons/FileCopy';
import Visibility from '@material-ui/icons/Visibility';
import VisibilityOff from '@material-ui/icons/VisibilityOff';
import * as React from 'react';
import { LoadingIcon } from 'uno-material-ui/dist/LoadingIcon';
import { SnackbarContext } from '../Provider/GlobalContext';
import { fetchRequest } from '../Utils/FetchRequest';

export const PasswordGenerator: React.FunctionComponent<any> = ({ value, setValue }) => {
    const { sendMessage } = React.useContext(SnackbarContext);
    const [visibility, setVisibility] = React.useState(false);
    const [isLoading, setLoading] = React.useState(true);

    const onMouseDownVisibility = () => setVisibility(true);
    const onMouseUpVisibility = () => setVisibility(false);

    const copyPassword = () => {
        navigator.clipboard.writeText(value);
        sendMessage('Password copied');
    };

    const retrievePassword = () => {
        fetchRequest(
            'api/password/generated',
            'GET',
        ).then((response: any) => {
            if (response && response.password) {
                setValue(response.password);
                setLoading(false);
            }
        });
    };

    React.useEffect(() => {
        retrievePassword();
    }, []);

    return (
        isLoading ?
            (
                <div
                    style={{ paddingTop: '30px' }}
                >
                    <LoadingIcon />
                </div>
            )
            :
            (
                <TextField
                    id='generatedPassword'
                    disabled={true}
                    label='New Password'
                    value={value}
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
                                    onMouseDown={onMouseDownVisibility}
                                    onMouseUp={onMouseUpVisibility}
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
                                    <FileCopy />
                                </IconButton>
                            </InputAdornment>,
                    }}
                />
            )
    );
};
