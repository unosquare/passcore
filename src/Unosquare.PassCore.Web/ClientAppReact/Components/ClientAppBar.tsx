import AppBar from '@material-ui/core/AppBar/AppBar';
import Fab from '@material-ui/core/Fab/Fab';
import Grid from '@material-ui/core/Grid/Grid';
import Typography from '@material-ui/core/Typography/Typography';
import HelpIcon from '@material-ui/icons/Help';
import * as React from 'react';

// ToDo: Add action to be called at onClick in Fab component
export const ClientAppBar: React.FunctionComponent<any> = () => {
    return (
        <AppBar
            position='fixed'
            style={{
                backgroundColor: '#304FF3',
                height: '6%',
            }}
        >
            <Grid
                alignItems='center'
                container={true}
                direction='row'
                justify='space-between'
            >
                <Grid
                    item={true}
                    xs={11}
                >
                    <Typography
                        variant='h4'
                        color='inherit'
                    >
                        Change Account Password
                    </Typography>
                </Grid>
                <Grid
                    item={true}
                    xs={1}
                >
                    <Fab
                        color='primary'
                    >
                        <HelpIcon
                            style={{
                                height: '100%',
                                width: '100%',
                            }}
                        />
                    </Fab>
                </Grid>
            </Grid>
        </AppBar>
    );
};
