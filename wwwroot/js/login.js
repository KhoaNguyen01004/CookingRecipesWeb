document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form');

    form.addEventListener('submit', function (e) {
        // Get the hCaptcha response token
        let token = '';
        if (typeof hcaptcha !== 'undefined') {
            token = hcaptcha.getResponse();
        }

        // For localhost testing with test sitekey, always provide a dummy token
        if (!token && window.hCaptchaSiteKey === '10000000-ffff-ffff-ffff-000000000001') {
            token = 'test-token-' + Date.now();
        }

        // Copy to hidden input
        document.getElementById('captchaToken').value = token;

        // Debug log for localhost testing
        console.log('[DEBUG] Login form submit - Captcha token:', token);

        // Reset the captcha after submission if hcaptcha is available
        if (typeof hcaptcha !== 'undefined') {
            hcaptcha.reset();
        }
    });

    // Show error message if present
    const errorMessage = window.errorMessage;
    if (errorMessage) {
        alert(errorMessage);
    }
});
