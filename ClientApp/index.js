import React from 'react';
import ReactDOM from 'react-dom/client';
import HCaptchaComponent from './HCaptchaComponent';

window.renderHCaptcha = (containerId, sitekey, onVerify, onExpire, onError) => {
    const container = document.getElementById(containerId);
    if (container) {
        const root = ReactDOM.createRoot(container);
        const captchaRef = React.createRef();
        window.captchaRef = captchaRef; // Expose ref globally for reset
        root.render(
            <HCaptchaComponent
                ref={captchaRef}
                sitekey={sitekey}
                onVerify={onVerify}
                onExpire={onExpire}
                onError={onError}
            />
        );
    }
};
