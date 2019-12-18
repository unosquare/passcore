import * as React from 'react';
import { GlobalContext } from '../Provider/GlobalContext';
import GoogleReCaptcha from './GoogleReCaptcha';

interface IRecaptchaProps {
    setToken: any;
    shouldReset: boolean;
}

export const ReCaptcha: React.FunctionComponent<IRecaptchaProps> = ({ setToken, shouldReset }: IRecaptchaProps) => {
    // tslint:disable-next-line
    let captchaRef: any;

    const { siteKey } = React.useContext(GlobalContext).recaptcha;

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
        <div
            style={{
                marginLeft: 'calc(100% - 440px)',
                marginTop: '25px',
            }}
        >
            <GoogleReCaptcha
                ref={(el: any) => {
                    captchaRef = el;
                }}
                size="normal"
                render="explicit"
                sitekey={siteKey}
                onloadCallback={onLoadRecaptcha}
                onSuccess={verifyCallback}
            />
        </div>
    );
};
