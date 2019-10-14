import * as React from 'react';

const noop = (): any => undefined;
interface IReCaptchaProps {
    badge: 'bottomright' | 'bottomleft' | 'inline';
    hl: string;
    inherit: boolean;
    isolated: boolean;
    onError: () => any;
    onExpired: () => any;
    onLoad: () => any;
    onSuccess: () => any;
    onloadCallback: () => any;
    verifyCallback: (recaptchaToken: string) => any;
    render: string;
    sitekey: string;
    size: 'compact' | 'normal' | 'invisible';
    tabIndex: string | number;
    theme: 'dark' | 'light';
    type: string;
}

interface IReCaptchaState {
    ready: boolean;
}

class GoogleReCaptcha extends React.Component<Partial<IReCaptchaProps>, IReCaptchaState> {

    protected static defaultProps: Partial<IReCaptchaProps> = {
        badge: 'bottomright',
        hl: 'en',
        inherit: true,
        isolated: false,
        onError: noop,
        onExpired: noop,
        onLoad: noop,
        onSuccess: noop,
        size: 'normal',
        tabIndex: 0,
        theme: 'light',
        type: 'image',
    };

    public readyIntervalId = setInterval(() => this._updateReadyState(), 1000);
    public recaptcha = React.createRef();

    private widgetId: string;

    constructor(props: any) {
        super(props);
        this.state = {
            ready: this.isReady(),
        };
    }

    public isReady = () =>
        typeof window !== 'undefined' &&
        typeof window.grecaptcha !== 'undefined' &&
        typeof window.grecaptcha.render === 'function'

    public componentWillUnmount() {
        clearInterval(this.readyIntervalId);
    }

    public componentDidUpdate(_prevProps: IReCaptchaProps, prevState: any) {
        if (!prevState.ready && this.state.ready) {
            this.widgetId = this.grecaptcha().render(
                this.recaptcha.current,
                {
                    'badge': this.props.badge,
                    'callback': this.props.onSuccess,
                    'error-callback': this.props.onError,
                    'expired-callback': this.props.onExpired,
                    'isolated': this.props.isolated,
                    'sitekey': this.props.sitekey,
                    'size': this.props.size,
                    'tabindex': this.props.tabIndex,
                    'theme': this.props.theme,
                },
                this.props.inherit,
            );
        }
    }

    public reset = () => {
        if (this.isReady()) {
            this.grecaptcha().reset(this.widgetId);
        }
    }

    public execute = () => {
        this.grecaptcha().execute(this.widgetId);
    }

    public shouldComponentUpdate(_nextProps: any, nextState: IReCaptchaState) {
        return !this.state.ready && nextState.ready;
    }

    public render() {
        const {
            onError,
            onExpired,
            onLoad,
            onSuccess,
            inherit,
            isolated,
            sitekey,
            theme,
            type,
            size,
            badge,
            tabIndex,
            ...rest
        } = this.props;

        return (
            <div
                ref={this.recaptcha}
                data-sitekey={sitekey}
                data-theme={theme}
                data-type={type}
                data-size={size}
                data-badge={badge}
                data-tabindex={tabIndex}
                {...rest}
            />
        );
    }

    private grecaptcha = () => window.grecaptcha;

    private _updateReadyState = () => {
        if (this.isReady()) {
            this.setState(() => ({
                ready: true,
            }));
            clearInterval(this.readyIntervalId);
            this.props.onLoad();
        }
    }
}

export default GoogleReCaptcha;
