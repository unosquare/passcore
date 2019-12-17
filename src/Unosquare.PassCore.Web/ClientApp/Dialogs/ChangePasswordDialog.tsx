import Button from '@material-ui/core/Button/Button';
import Dialog from '@material-ui/core/Dialog/Dialog';
import DialogContent from '@material-ui/core/DialogContent/DialogContent';
import DialogTitle from '@material-ui/core/DialogTitle/DialogTitle';
import Typography from '@material-ui/core/Typography/Typography';
import * as React from 'react';
import { GlobalContext } from '../Provider/GlobalContext';

export const ChangePasswordDialog: React.FunctionComponent<any> = ({ open, onClose }) => {
    const { successAlertBody, successAlertTitle } = React.useContext(GlobalContext).alerts;
    return (
        <Dialog open={open} disableEscapeKeyDown={true} disableBackdropClick={true}>
            <DialogTitle>{successAlertTitle}</DialogTitle>
            <DialogContent>
                <Typography variant="subtitle1">{successAlertBody}</Typography>
                <Button
                    variant="contained"
                    color="primary"
                    onClick={onClose}
                    style={{
                        margin: '10px 0 0 75%',
                        width: '25%',
                    }}
                >
                    Ok
                </Button>
            </DialogContent>
        </Dialog>
    );
};
