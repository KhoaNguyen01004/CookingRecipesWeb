document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form');

    form.addEventListener('submit', function (e) {
        e.preventDefault();

        // Get captcha token
        let captchaToken = '';
        if (typeof hcaptcha !== 'undefined') {
            captchaToken = hcaptcha.getResponse();
        }

        // For localhost testing with test sitekey, always provide a dummy token
        if (!captchaToken && window.hCaptchaSiteKey === '10000000-ffff-ffff-ffff-000000000001') {
            captchaToken = 'test-token-' + Date.now();
        }

        if (!captchaToken) {
            alert('Please complete the captcha.');
            return;
        }

        // Add captcha token to form
        let tokenInput = document.querySelector('input[name="captchaToken"]');
        if (!tokenInput) {
            tokenInput = document.createElement('input');
            tokenInput.type = 'hidden';
            tokenInput.name = 'captchaToken';
            form.appendChild(tokenInput);
        }
        tokenInput.value = captchaToken;

        // Debug log for localhost testing
        console.log('[DEBUG] Reset Password form submit - Captcha token:', captchaToken);

        // Submit form
        form.submit();
    });

    // Display messages if any
    if (window.errorMessage) {
        alert(window.errorMessage);
    }
    if (window.successMessage) {
        alert(window.successMessage);
    }
});
