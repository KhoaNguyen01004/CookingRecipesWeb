import React, { useRef, forwardRef, useImperativeHandle } from 'react';
import HCaptcha from '@hcaptcha/react-hcaptcha';

const HCaptchaComponent = forwardRef(({ sitekey, onVerify, onExpire, onError }, ref) => {
    const captchaRef = useRef(null);

    const onVerifyHandler = (token) => {
        if (onVerify) onVerify(token);
    };

    const onExpireHandler = () => {
        if (onExpire) onExpire();
    };

    const onErrorHandler = (err) => {
        if (onError) onError(err);
    };

    const resetCaptcha = () => {
        if (captchaRef.current) {
            captchaRef.current.resetCaptcha();
        }
    };

    // Expose resetCaptcha to parent
    useImperativeHandle(ref, () => ({
        resetCaptcha,
    }));

    return (
        <HCaptcha
            sitekey={sitekey}
            onVerify={onVerifyHandler}
            onExpire={onExpireHandler}
            onError={onErrorHandler}
            ref={captchaRef}
        />
    );
});

export default HCaptchaComponent;
