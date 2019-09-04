import Grid from '@material-ui/core/Grid/Grid';
import Typography from '@material-ui/core/Typography/Typography';
import * as React from 'react';
import { loadReCaptcha } from 'react-recaptcha-v3';
import { LoadingIcon } from 'uno-material-ui/dist/LoadingIcon';
import { useEffectWithLoading } from 'uno-react';
import { EntryPoint } from './Components/EntryPoint';
import { GlobalContextProvider } from './Provider/GlobalContextProvider';
import { SnackbarContextProvider } from './Provider/SnackbarContextProvider';
import { resolveAppSettings } from './Utils/AppSettings';

export const Main: React.FunctionComponent<any> = () => {
    const [settings, isLoading] = useEffectWithLoading(resolveAppSettings, {}, []);

    React.useEffect(() => {
        if (settings && settings.recaptcha) {
            if (settings.recaptcha.siteKey !== '') {
                loadReCaptcha(settings.recaptcha.siteKey);
            }
        }
    }, [settings]);

    if (isLoading) {
        return (
            <React.Fragment>
                <Grid
                    container={true}
                    alignItems='center'
                    direction='column'
                    justify='center'
                >
                    <Grid
                        item={true}
                        key='title'
                    >
                        <Typography
                            variant='h3'
                            align='center'
                        >
                            Loading Passcore...
                        </Typography>
                    </Grid>
                    <Grid
                        item={true}
                    >
                        <LoadingIcon />
                    </Grid>
                </Grid>
            </React.Fragment>
        );
    }

    return (
        <GlobalContextProvider settings={settings}>
            <SnackbarContextProvider>
                <EntryPoint />
            </SnackbarContextProvider>
        </GlobalContextProvider>
    );
};
