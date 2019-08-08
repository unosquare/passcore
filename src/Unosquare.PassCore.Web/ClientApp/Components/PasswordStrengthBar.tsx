import LinearProgress from '@material-ui/core/LinearProgress';
import * as React from 'react';
import * as zxcvbn from 'zxcvbn';

const measureStrength = (password: string) => {
    const result = zxcvbn.default(password);
    return Math.min(
        result.guesses_log10 * 10,
        100,
    );
};

const getProgressColor = (strength: number) => strength < 33 ? '#ff5722' : strength < 66 ? '#ffc107' : '#4caf50';

export const PasswordStrengthBar: React.FunctionComponent<any> = ({ newPassword/*, reset*/ }) => {
    const newStrength = measureStrength(newPassword);
    const primeColor = getProgressColor(newStrength);

    return (
        <LinearProgress
            style={{
                backgroundColor: primeColor,
                color: '#000000',
                display: 'flex',
                flexGrow: 1,
                marginBottom: '20px',
            }}
            variant='determinate'
            value={newStrength}
        />
    );
};
