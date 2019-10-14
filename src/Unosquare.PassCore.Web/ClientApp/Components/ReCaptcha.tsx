import * as React from 'react';
import { GlobalContext } from '../Provider/GlobalContext';
import GoogleReCaptcha from './GoogleReCaptcha';

export const ReCaptcha: React.FunctionComponent<any> = ({ setToken, shouldReset }) => {

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
        marginLeft: 'calc(100% - 481px)',
        marginTop: '25px',
      }}
    >
      <GoogleReCaptcha
        ref={(el: any) => { captchaRef = el; }}
        size='normal'
        render='explicit'
        sitekey={siteKey}
        onloadCallback={onLoadRecaptcha}
        verifyCallback={verifyCallback}
      />
    </div>
  );
};
