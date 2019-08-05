import * as React from 'react';
import { ClientAppBar } from './ClientAppBar';
import { MainForm } from './MainForm';

export const EntryPoint: React.FunctionComponent<any> = () => (
    <div>
        <ClientAppBar />
        <main>
            <MainForm />
        </main>
    </div>
);
