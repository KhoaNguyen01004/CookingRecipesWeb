# TODO: Fix hCaptcha Integration

## Steps to Complete

- [x] Remove custom hidden input 'captchaToken' from Views/Account/Login.cshtml
- [x] Remove custom hidden input 'captchaToken' from Views/Account/Register.cshtml
- [x] Remove custom hidden input 'captchaToken' from Views/Account/ResetPassword.cshtml
- [x] Update Controllers/AccountController.cs: Change captchaToken parameter to [FromForm(Name = "h-captcha-response")] in Login, Register, ResetPassword methods
- [x] Add logging in VerifyCaptchaToken method for debugging
- [x] Add hCaptcha test keys to appsettings.Development.json
- [x] Comment out unused React hCaptcha components in ClientApp/index.js
- [x] Test the application and verify CAPTCHA verification works
