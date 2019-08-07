import * as React from 'react';
import { ReCaptcha } from 'react-recaptcha-google';
import { GlobalContext } from '../Provider/GlobalContext';

export const ReCaptchaComponent: React.FunctionComponent<any> = ({ setToken, shouldReset }) => {

    const { SiteKey } = React.useContext(GlobalContext).Recaptcha;

    // tslint:disable-next-line
    let captchaRef: any;

    React.useEffect(() => {
        if (captchaRef) {
            captchaRef.reset();
        }
    }, [shouldReset]);

    const onLoadRecaptcha = () => {
        if (captchaRef) {
            captchaRef.reset();
        }
    };

    const verifyCallback = (recaptchaToken: any) => setToken(recaptchaToken);

    return (
        <ReCaptcha
            ref={(el: any) => { captchaRef = el; }}
            size='normal'
            render='explicit'
            sitekey={SiteKey}
            onloadCallback={onLoadRecaptcha}
            verifyCallback={verifyCallback}
        />
    );
};
