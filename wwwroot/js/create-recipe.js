let stepCount = 0;
const ingredients = [];
const measures = [];
const selectedCategories = [];
const selectedAreas = [];

document.addEventListener('DOMContentLoaded', function () {

    // Auto-focus on first field
    const mealInput = document.getElementById('StrMeal');
    if (mealInput) {
        mealInput.focus();
    }

    // Load categories and areas
    loadCategories();
    loadAreas();

    // Initialize with one step
    addStep();

    // Ingredient input handling
    const ingredientInput = document.getElementById('ingredient-input');
    const measureInput = document.getElementById('measure-input');
    const addIngredientBtn = document.getElementById('add-ingredient-btn');

    function addIngredientFromInputs() {
        if (!ingredientInput || !measureInput) return;
        const ingredient = ingredientInput.value.trim();
        const measure = measureInput.value.trim();
        if (ingredient) {
            addIngredient(ingredient, measure);
            ingredientInput.value = '';
            measureInput.value = '';
            if (ingredientInput) ingredientInput.focus();
        }
    }

    if (ingredientInput) {
        ingredientInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                addIngredientFromInputs();
            }
        });
    }

    if (measureInput) {
        measureInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                addIngredientFromInputs();
            }
        });
    }

    if (addIngredientBtn) {
        addIngredientBtn.addEventListener('click', function () {
            addIngredientFromInputs();
        });
    }

    // Add step button
    const addStepBtn = document.getElementById('add-step');
    if (addStepBtn) {
        addStepBtn.addEventListener('click', function () {
            addStep();
        });
    }

    // Image preview
    const imageInput = document.querySelector('input[name="StrMealThumb"]');
    const imagePreview = document.getElementById('image-preview');
    imageInput.addEventListener('input', function () {
        const url = this.value.trim();
        if (url && isValidUrl(url)) {
            imagePreview.src = url;
            imagePreview.style.display = 'block';
        } else {
            imagePreview.style.display = 'none';
        }
    });

    // Category selection
    document.addEventListener('click', function (e) {
        if (e.target.closest('.category-card')) {
            const card = e.target.closest('.category-card');
            const categoryId = card.dataset.id;
            const isFromApi = card.dataset.isFromApi === 'true';

            // Allow selection of API categories but prevent deselection if required
            const index = selectedCategories.indexOf(categoryId);
            if (index > -1) {
                // If it's an API category and required, don't allow deselection
                if (isFromApi && card.classList.contains('required')) {
                    return;
                }
                selectedCategories.splice(index, 1);
                card.classList.remove('selected');
            } else {
                selectedCategories.push(categoryId);
                card.classList.add('selected');
            }
            updateSelectedCategories();
        }
    });

    // Area selection
    document.addEventListener('click', function (e) {
        if (e.target.classList.contains('option-btn') && e.target.closest('#areas-section')) {
            document.querySelectorAll('#areas-section .option-btn').forEach(btn => btn.classList.remove('selected'));
            e.target.classList.add('selected');
            selectedAreas.length = 0;
            selectedAreas.push(e.target.dataset.id);
            updateSelectedAreas();
        }
    });

    // Add category modal
    const addCategoryBtn = document.getElementById('add-category-btn');
    if (addCategoryBtn) {
        addCategoryBtn.addEventListener('click', function () {
            const modal = new bootstrap.Modal(document.getElementById('addCategoryModal'));
            modal.show();
        });
    }

    // Save category
    const saveCategoryBtn = document.getElementById('save-category-btn');
    if (saveCategoryBtn) {
        saveCategoryBtn.addEventListener('click', function () {
            const categoryName = document.getElementById('category-name-input').value.trim();
            const categoryImageUrl = document.getElementById('category-image-input').value.trim();
            const categoryDescription = document.getElementById('category-description-input').value.trim();
            if (categoryName) {
                saveCategory(categoryName, categoryImageUrl, categoryDescription);
            }
        });
    }

    // Form submission
    document.getElementById('recipe-form').addEventListener('submit', function (e) {
        // Validation
        const mealName = document.getElementById('StrMeal').value.trim();
        const stepInputs = document.querySelectorAll('.step-input');
        let isValid = true;
        let errorMessage = '';

        if (!mealName) {
            isValid = false;
            errorMessage = 'Recipe name is required.';
        } else if (selectedCategories.length === 0) {
            isValid = false;
            errorMessage = 'At least one category must be selected.';
        } else if (selectedAreas.length === 0) {
            isValid = false;
            errorMessage = 'Area must be selected.';
        } else if (ingredients.length === 0) {
            isValid = false;
            errorMessage = 'At least one ingredient must be added.';
        } else {
            // Check if all steps are filled
            for (let input of stepInputs) {
                if (!input.value.trim()) {
                    isValid = false;
                    errorMessage = 'All cooking steps must be filled.';
                    break;
                }
            }
        }

        if (!isValid) {
            e.preventDefault();
            alert(errorMessage);
            return;
        }

        // Set hidden fields
        // Clear existing category inputs
        document.querySelectorAll('input[name="SelectedCategoryIds"]').forEach(input => input.remove());
        // Create hidden inputs for each selected category
        selectedCategories.forEach(categoryId => {
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'SelectedCategoryIds';
            input.value = categoryId;
            document.getElementById('recipe-form').appendChild(input);
        });
        document.getElementById('selected-areas').value = selectedAreas.length > 0 ? selectedAreas[0] : '';
        document.getElementById('ingredients-list').value = ingredients.join(',');
        document.getElementById('measures-list').value = measures.join(',');

        // Combine steps into instructions
        const stepsInputs = document.querySelectorAll('.step-input');
        const steps = Array.from(stepsInputs).map(input => input.value.trim()).filter(step => step);
        document.getElementById('steps-list').value = steps.join('\n');
    });
});

function loadCategories() {
    const container = document.getElementById('categories-available');
    container.innerHTML = '<div class="text-center"><div class="spinner-border spinner-border-sm" role="status"><span class="visually-hidden">Loading...</span></div> Loading categories...</div>';

    fetch('/Admin/GetCategories')
        .then(response => response.json())
        .then(data => {
            console.log('Loaded categories:', data);
            container.innerHTML = '';
            data.forEach(category => {
                const card = document.createElement('div');
                card.className = `category-card ${category.isFromApi ? 'api-category' : 'db-category'}`;
                card.dataset.id = category.id;
                card.dataset.isFromApi = category.isFromApi;
                card.tabIndex = 0; // Make focusable

                card.innerHTML = `
                    <div class="category-name">${category.name}</div>
                `;

                container.appendChild(card);
                console.log(`Added category: ${category.name} (API: ${category.isFromApi})`);
            });
            console.log(`Total categories displayed: ${data.length}`);
        })
        .catch(error => {
            console.error('Error loading categories:', error);
            container.innerHTML = '<div class="alert alert-danger"><i class="fas fa-exclamation-triangle me-2"></i>Failed to load categories. Please refresh the page.</div>';
        });
}

function loadAreas() {
    fetch('/Admin/GetAreas')
        .then(response => response.json())
        .then(data => {
            const container = document.getElementById('areas-section').querySelector('.options-grid');
            container.innerHTML = '';
            data.forEach(area => {
                const btn = document.createElement('button');
                btn.type = 'button';
                btn.className = 'option-btn api-category'; // Add theme class
                btn.dataset.id = area;
                btn.textContent = area;
                container.appendChild(btn);
            });
        });
}

function addStep() {
    stepCount++;
    const stepsContainer = document.getElementById('steps-container');
    const stepItem = document.createElement('div');
    stepItem.className = 'step-item';
    stepItem.innerHTML = `
        <span class="step-number">${stepCount}.</span>
        <input type="text" class="form-control step-input" name="Steps[${stepCount - 1}]" placeholder="Enter cooking step" required />
        <button type="button" class="btn btn-danger remove-step" onclick="removeStep(this)">×</button>
    `;
    stepsContainer.appendChild(stepItem);
}

function removeStep(button) {
    const stepItem = button.parentElement;
    stepItem.remove();
    updateStepNumbers();
}

function updateStepNumbers() {
    const stepItems = document.querySelectorAll('.step-item');
    stepItems.forEach((item, index) => {
        item.querySelector('.step-number').textContent = `${index + 1}.`;
        item.querySelector('.step-input').name = `Steps[${index}]`;
    });
    stepCount = stepItems.length;
}

function addIngredient(ingredient, measure = '') {
    if (!ingredients.includes(ingredient)) {
        ingredients.push(ingredient);
        measures.push(measure);
        updateIngredientsDisplay();
    }
}

function updateIngredientsDisplay() {
    const container = document.getElementById('ingredients-tags');
    container.innerHTML = '';
    ingredients.forEach((ing, index) => {
        const measure = measures[index] || '';
        const displayText = measure ? `${ing} (${measure})` : ing;
        const tag = document.createElement('span');
        tag.className = 'ingredient-tag';
        tag.innerHTML = `${displayText} <span class="remove-tag" onclick="removeIngredient('${ing}')">×</span>`;
        container.appendChild(tag);
    });
    document.getElementById('ingredients-list').value = ingredients.join(',');
    document.getElementById('measures-list').value = measures.join(',');
}

function removeIngredient(ingredient) {
    const index = ingredients.indexOf(ingredient);
    if (index > -1) {
        ingredients.splice(index, 1);
        measures.splice(index, 1);
        updateIngredientsDisplay();
    }
}

function updateSelectedCategories() {
    const container = document.getElementById('selected-categories-display');
    container.innerHTML = '';
    selectedCategories.forEach(id => {
        const chip = document.createElement('span');
        chip.className = 'selected-chip';
        chip.innerHTML = `<span class="chip-text">${document.querySelector(`[data-id="${id}"]`).textContent}</span><span class="remove-chip" onclick="removeSelectedCategory('${id}')">×</span>`;
        container.appendChild(chip);
    });
}

function removeSelectedCategory(id) {
    const index = selectedCategories.indexOf(id);
    if (index > -1) {
        selectedCategories.splice(index, 1);
        document.querySelector(`[data-id="${id}"]`).classList.remove('selected');
        updateSelectedCategories();
    }
}

function updateSelectedAreas() {
    const container = document.getElementById('selected-areas-display');
    container.innerHTML = '';
    selectedAreas.forEach(id => {
        const chip = document.createElement('span');
        chip.className = 'selected-chip';
        chip.innerHTML = `<span class="chip-text">${document.querySelector(`#areas-section [data-id="${id}"]`).textContent}</span><span class="remove-chip" onclick="removeSelectedArea('${id}')">×</span>`;
        container.appendChild(chip);
    });
}

function removeSelectedArea(id) {
    selectedAreas.length = 0;
    document.querySelectorAll('#areas-section .option-btn').forEach(btn => btn.classList.remove('selected'));
    updateSelectedAreas();
}

function saveCategory(name, imageUrl) {
    const btn = document.getElementById('save-category-btn');
    const spinner = btn.querySelector('.spinner-border');
    const errorDiv = document.getElementById('category-error');

    btn.disabled = true;
    spinner.classList.remove('d-none');
    errorDiv.textContent = '';

    fetch('/Admin/CreateCategoryAjax', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name: name, thumbnailUrl: imageUrl })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                loadCategories();
                bootstrap.Modal.getInstance(document.getElementById('addCategoryModal')).hide();
                document.getElementById('category-name-input').value = '';
                document.getElementById('category-image-input').value = '';
            } else {
                errorDiv.textContent = data.message;
            }
        })
        .catch(() => {
            errorDiv.textContent = 'An error occurred. Please try again.';
        })
        .finally(() => {
            btn.disabled = false;
            spinner.classList.add('d-none');
        });
}

function isValidUrl(string) {
    try {
        new URL(string);
        return true;
    } catch (_) {
        return false;
    }
}
