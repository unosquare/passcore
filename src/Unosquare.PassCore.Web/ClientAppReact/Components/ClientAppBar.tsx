import AppBar from '@material-ui/core/AppBar/AppBar';
import Grid from '@material-ui/core/Grid/Grid';
import IconButton from '@material-ui/core/IconButton/IconButton';
import Typography from '@material-ui/core/Typography/Typography';
import HelpIcon from '@material-ui/icons/Help';
import * as React from 'react';

// ToDo: Add action to be called at onClick in Fab component
export const ClientAppBar: React.FunctionComponent<any> = () => (
    <AppBar
        position='fixed'
        style={{
            backgroundColor: '#304FF3',
            height: '65px',
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
                alignItems='center'
            >
                <Typography
                    variant='h6'
                    style={{
                        color: 'white',
                    }}
                >
                    Change Account Password
                </Typography>
            </Grid>
            <Grid
                item={true}
                xs={1}
            >
                <IconButton
                    style={{
                        height: '25px',
                        width: '25px',
                    }}
                    color='default'
                >
                    <HelpIcon />
                </IconButton>
            </Grid>
        </Grid>
    </AppBar>
);
