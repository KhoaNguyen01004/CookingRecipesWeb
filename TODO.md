# TODO: Improve Error/Warning Messages to Toast Notifications

## Current Issues Found:
- Multiple views use Bootstrap alerts for TempData messages (ManageFavorites, ManageCategories)
- Login.js uses alert() instead of toast for error messages
- Admin pages (ManageCategories, ManageRecipes) use alert() in JavaScript
- ModelState errors in Login/Register use Bootstrap alerts
- Inconsistent message display across the application

## Tasks to Complete:

### 1. Create Global Toast Utility
- [ ] Add toast utility functions to site.js for showing success/error toasts
- [ ] Add function to check and display TempData messages on page load

### 2. Update Login/Register Pages
- [ ] Update login.js to use toast instead of alert() for error messages
- [ ] Update Login.cshtml to use toast for ModelState errors
- [ ] Update Register.cshtml to use toast for ModelState errors
- [ ] Ensure success messages also use toast

### 3. Update Admin Views
- [ ] Update ManageFavorites.cshtml to use toast instead of Bootstrap alerts
- [ ] Update ManageCategories.cshtml to use toast instead of Bootstrap alerts
- [ ] Update ManageRecipes.cshtml JavaScript to use toast instead of alert()
- [ ] Update ManageCategories.cshtml JavaScript to use toast instead of alert()

### 4. Handle Success Messages
- [ ] Ensure all success messages (TempData["Success"], TempData["SuccessMessage"]) use toast
- [ ] Update any missing success message handling

### 5. Testing
- [ ] Test all updated pages to ensure toasts display correctly
- [ ] Verify toast auto-dismiss behavior
- [ ] Check toast styling in both light and dark themes
