import LinearProgress from '@material-ui/core/LinearProgress';
import * as React from 'react';
import * as zxcvbn from 'zxcvbn';

const measureStrength = (password: string): number => Math.min(
    zxcvbn.default(password).guesses_log10 * 10,
    100,
);

const getProgressColor = (strength: number) => strength < 33 ? '#ff5722' : strength < 66 ? '#ffc107' : '#4caf50';

export const PasswordStrengthBar: React.FunctionComponent<any> = ({ newPassword/*, reset*/ }) => {
    const newStrength = measureStrength(newPassword);
    const primeColor = getProgressColor(newStrength);

    return (
        <LinearProgress
            style={{
                backgroundColor: primeColor,
                color: '#ffcdd2',
                display: 'flex',
                flexGrow: 1,
            }}
            variant='determinate'
            value={newStrength}
        />
    );
};
