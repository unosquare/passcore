import * as React from 'react';
import { GlobalActionsContext } from "./GlobalContext";

export const GlobalActionsProvider: React.FunctionComponent<any> = ({
    children,
}) => {

    const provActions = {
        //here should go all the actions.
    };

    return (
        <GlobalActionsContext.Provider value={provActions}>
            {children}
        </GlobalActionsContext.Provider>
    );
};
