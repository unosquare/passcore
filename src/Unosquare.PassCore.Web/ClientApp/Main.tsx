import Grid from '@material-ui/core/Grid/Grid';
import Typography from '@material-ui/core/Typography/Typography';
import * as React from 'react';
import { loadReCaptcha } from 'react-recaptcha-google';
import { LoadingIcon } from 'uno-material-ui';
import { useEffectWithLoading } from 'uno-react';
import { EntryPoint } from './Components/EntryPoint';
import { GlobalActionsProvider } from './Provider/GlobalActionsProvider';
import { GlobalContextProvider } from './Provider/GlobalContextProvider';
import { resolveAppSettings } from './Utils/AppSettings';

export const Main: React.FunctionComponent<any> = () => {
    const [settings, isLoading] = useEffectWithLoading(resolveAppSettings, {}, []);

    React.useEffect(() => {
        if (settings) {
            loadReCaptcha();
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
                        // className={classes.loadingtitle}
                        >
                            Loading Passcore...
                        </Typography>
                    </Grid>
                    <Grid
                        item={true}
                    >
                        <LoadingIcon
                        // className={classes.loadingIcon}
                        />
                    </Grid>
                </Grid>
            </React.Fragment>
        );
    }

    return (
        <GlobalContextProvider settings={settings}>
            <GlobalActionsProvider>
                <EntryPoint />
            </GlobalActionsProvider>
        </GlobalContextProvider>
    );
};
