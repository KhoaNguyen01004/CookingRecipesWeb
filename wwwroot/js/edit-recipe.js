document.addEventListener('DOMContentLoaded', function () {
    // Handle form submission
    document.querySelector('form').addEventListener('submit', function (e) {
        // Get selected categories and areas
        const selectedCategories = Array.from(document.getElementById('categories-select').selectedOptions).map(option => option.value);

        // Create hidden inputs for categories
        selectedCategories.forEach(categoryId => {
            const categoryInput = document.createElement('input');
            categoryInput.type = 'hidden';
            categoryInput.name = 'SelectedCategoryIds';
            categoryInput.value = categoryId;
            this.appendChild(categoryInput);
        });
    });
});
