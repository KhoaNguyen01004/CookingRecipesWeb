# TODO: Refactor Authentication Pages for Standard hCaptcha

## Steps to Complete

- [x] Update Views/Account/Login.cshtml: Add hCaptcha script tag in @section Scripts.
- [x] Update Views/Account/Register.cshtml: Add hCaptcha script tag in @section Scripts.
- [x] Update wwwroot/js/login.js: Replace React code with standard hCaptcha, add form submit handler to copy .h-captcha-response to captchaToken.
- [x] Update wwwroot/js/register.js: Replace React code with standard hCaptcha, add form submit handler to copy .h-captcha-response to captchaToken.
- [x] Update wwwroot/js/reset-password.js: Add dummy token for localhost testing.
- [x] Add debug logs to JavaScript for localhost testing.
- [x] Update TODO.md: Mark tasks as completed.
- [x] Test login, register, and reset password pages with hCaptcha.
- [x] Verify debug logs show token submission on localhost.
- [x] Added client-side and server-side logging for CAPTCHA debugging.
