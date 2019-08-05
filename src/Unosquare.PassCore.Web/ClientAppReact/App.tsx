import ThemeProvider from '@material-ui/styles/ThemeProvider';
import * as React from 'react';
import { render } from 'react-dom';
import { UnoTheme } from 'uno-material-ui';
import { Main } from './Main';

render(
    (
        <ThemeProvider
            theme={UnoTheme}
        >
            <Main />
        </ThemeProvider>
    ), document.getElementById('rootNode'),
);
