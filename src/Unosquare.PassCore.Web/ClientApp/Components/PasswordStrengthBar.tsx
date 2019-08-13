import LinearProgress from '@material-ui/core/LinearProgress';
import makeStyles from '@material-ui/core/styles/makeStyles';
import * as React from 'react';
import * as zxcvbn from 'zxcvbn';

const useStyles = makeStyles({
    progressBar: {
        color: '#000000',
        display: 'flex',
        flexGrow: 1,
    },
    progressBarColor_HIGH: {
        backgroundColor: '#4caf50',
    },
    progressBarColor_LOW: {
        backgroundColor: '#ff5722',
    },
    progressBarColor_MEDIUM: {
        backgroundColor: '#ffc107',
    },
});

const measureStrength = (password: string): number => Math.min(
    zxcvbn.default(password).guesses_log10 * 10,
    100,
);

export const PasswordStrengthBar: React.FunctionComponent<any> = ({ newPassword }) => {
    const classes = useStyles();

    const getProgressColor = (strength: number) => ({
            barColorPrimary: strength < 33 ? classes.progressBarColor_LOW :
                strength < 66 ? classes.progressBarColor_MEDIUM :
                    classes.progressBarColor_HIGH,
    });

    const newStrength = measureStrength(newPassword);
    const primeColor = getProgressColor(newStrength);

    return (
        <LinearProgress
            classes={primeColor}
            variant='determinate'
            value={newStrength}
            className={classes.progressBar}
        />
    );
};
