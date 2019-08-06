import * as React from 'react';
import { ClientAppBar } from './ClientAppBar';
import { MainForm } from './MainForm';

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
            <MainForm />
        </main>
    </div>
);
