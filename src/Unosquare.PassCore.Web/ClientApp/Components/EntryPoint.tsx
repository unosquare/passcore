import * as React from 'react';
import { ChangePassword } from './ChangePassword';
import { ClientAppBar } from './ClientAppBar';

export const EntryPoint: React.FunctionComponent<any> = () => (
    <div
        style={{
            height: '100%',
        }}
    >
        <ClientAppBar />
        <main
            style={{
                display: 'flex',
                flexGrow: 1,
                height: '100%',
                justifyContent: 'center',
            }}
        >
            <ChangePassword />
        </main>
    </div>
);
