document.addEventListener('DOMContentLoaded', function () {
    const password = document.getElementById('password');
    const confirmPassword = document.getElementById('confirmPassword');
    const passwordMatchError = document.getElementById('passwordMatchError');
    const registerBtn = document.getElementById('registerBtn');

    function validatePasswords() {
        if (password.value && confirmPassword.value) {
            if (password.value === confirmPassword.value) {
                passwordMatchError.style.display = 'none';
                registerBtn.disabled = false;
                return true;
            } else {
                passwordMatchError.style.display = 'block';
                registerBtn.disabled = true;
                return false;
            }
        } else {
            passwordMatchError.style.display = 'none';
            registerBtn.disabled = false;
            return true;
        }
    }

    password.addEventListener('input', validatePasswords);
    confirmPassword.addEventListener('input', validatePasswords);

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
        console.log('[DEBUG] Register form submit - Captcha token:', token);

        // Reset the captcha after submission if hcaptcha is available
        if (typeof hcaptcha !== 'undefined') {
            hcaptcha.reset();
        }
    });

    // Also show error toast if TempData has error message
    if (globalThis.errorMessage) {
        const errorToast = new bootstrap.Toast(document.getElementById('errorToast'));
        document.getElementById('errorMessage').textContent = window.errorMessage;
        errorToast.show();
    }
});
