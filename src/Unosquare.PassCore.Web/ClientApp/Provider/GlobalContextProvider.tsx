import * as React from 'react';
import { GlobalContext } from './GlobalContext';

interface IGlobalContextProviderProps {
    children: any;
    settings: any;
}

export const GlobalContextProvider: React.FunctionComponent<IGlobalContextProviderProps> = ({
    children,
    settings,
}: IGlobalContextProviderProps) => {
    const [getProviderValue] = React.useState({ ...settings });
    return <GlobalContext.Provider value={getProviderValue}>{children}</GlobalContext.Provider>;
};
