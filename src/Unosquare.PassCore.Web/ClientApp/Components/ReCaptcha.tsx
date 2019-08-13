import * as React from 'react';
import { ReCaptcha } from 'react-recaptcha-v3';

export const ReCaptchaComponent: React.FunctionComponent<any> = ({ setToken, siteKey }) => {
    const verifyCallback = (recaptchaToken: any) => setToken(recaptchaToken);

    return (
        <ReCaptcha
            sitekey={siteKey}
            verifyCallback={verifyCallback}
        />
    );
};
