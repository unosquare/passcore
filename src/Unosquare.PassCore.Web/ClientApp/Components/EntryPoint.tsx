import * as React from 'react';
import { ChangePassword } from './ChangePassword';
import { ClientAppBar } from './ClientAppBar';
import { Footer } from './Footer';

export const EntryPoint: React.FunctionComponent<any> = () => (
    <React.Fragment>
        <ClientAppBar />
        <main
            style={{
                marginLeft: 'calc((100% - 650px)/2)',
            }}
        >
            <ChangePassword />
            <Footer />
        </main>
    </React.Fragment>
);
