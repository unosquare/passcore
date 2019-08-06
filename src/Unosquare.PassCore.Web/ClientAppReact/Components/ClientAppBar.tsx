import AppBar from '@material-ui/core/AppBar/AppBar';
import Grid from '@material-ui/core/Grid/Grid';
import Tooltip from '@material-ui/core/Tooltip/Tooltip';
import Typography from '@material-ui/core/Typography/Typography';
import HelpIcon from '@material-ui/icons/Help';
import * as React from 'react';

export const ClientAppBar: React.FunctionComponent<any> = () => (
    <AppBar
        position='fixed'
        style={{
            backgroundColor: '#304FF3',
            height: '64px',
        }}
        elevation={0}
    >
        <Grid
            container={true}
            style={{ height: '64px' }}
            direction='row'
            justify='space-between'
            alignItems='center'
        >
            <Typography
                variant='h6'
                color='secondary'
                style={{
                    paddingLeft: '1.5%',
                }}
            >
                Change Account Password
            </Typography>
            <Tooltip
                title='If you are having trouble with this tool, please contact IT Support'
                placement='left-end'
            >
                <HelpIcon color='secondary' style={{ paddingRight: '1%' }} />
            </Tooltip>
        </Grid>
    </AppBar >
);
