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

    // Show error toast for TempData error message
    if (window.errorMessage) {
        showErrorToast(window.errorMessage);
    }

    // Show error toast for ModelState errors
    if (window.modelStateErrors && window.modelStateErrors.length > 0) {
        const errorMessage = window.modelStateErrors.join('\n');
        showErrorToast(errorMessage);
    }
});
