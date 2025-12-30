// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Toast utility functions
function showSuccessToast(message) {
    const successToast = new bootstrap.Toast(document.getElementById('successToast'));
    document.getElementById('successMessage').textContent = message;
    successToast.show();
}

function showErrorToast(message) {
    const errorToast = new bootstrap.Toast(document.getElementById('errorToast'));
    document.getElementById('errorMessage').textContent = message;
    errorToast.show();
}

// Check for TempData messages on page load
function checkForTempDataMessages() {
    if (window.successMessage) {
        showSuccessToast(window.successMessage);
    }
    if (window.errorMessage) {
        showErrorToast(window.errorMessage);
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const themeToggle = document.getElementById('theme-toggle');
    const themeText = document.getElementById('theme-text');
    const html = document.documentElement;

    // Check for TempData messages
    checkForTempDataMessages();

    // Check for saved theme preference or default to light mode
    const currentTheme = localStorage.getItem('theme') || 'light';
    if (currentTheme === 'dark') {
        html.setAttribute('data-bs-theme', 'dark');
        themeText.textContent = 'Light Mode';
    } else {
        html.setAttribute('data-bs-theme', 'light');
        themeText.textContent = 'Dark Mode';
    }

    // Toggle theme on button click
    themeToggle.addEventListener('click', function () {
        if (html.getAttribute('data-bs-theme') === 'dark') {
            html.setAttribute('data-bs-theme', 'light');
            themeText.textContent = 'Dark Mode';
            localStorage.setItem('theme', 'light');
        } else {
            html.setAttribute('data-bs-theme', 'dark');
            themeText.textContent = 'Light Mode';
            localStorage.setItem('theme', 'dark');
        }
    });

    // Load More functionality
    const loadMoreBtn = document.getElementById('load-more');
    if (loadMoreBtn) {
        loadMoreBtn.addEventListener('click', function () {
            loadMoreBtn.disabled = true;
            loadMoreBtn.textContent = 'Loading...';

            const category = loadMoreBtn.getAttribute('data-category') || '';
            const skip = parseInt(loadMoreBtn.getAttribute('data-skip') || '0');

            let url = '/Home/LoadMoreRecipes?';
            if (category) {
                url += `category=${encodeURIComponent(category)}&`;
            }
            url += `skip=${skip}`;

            fetch(url)
                .then(response => response.json())
                .then(data => {
                    if (data && data.length > 0) {
                        const row = document.querySelector('.row');
                        data.forEach(recipe => {
                            const col = document.createElement('div');
                            col.className = 'col-md-4 mb-4';
                            col.innerHTML = `
                                <div class="card h-100">
                                    <img src="${recipe.strMealThumb || '/images/default-recipe.jpg'}" class="card-img-top" alt="${recipe.strMeal || 'Recipe'}" style="height: 200px; object-fit: cover;">
                                    <div class="card-body d-flex flex-column">
                                        <h5 class="card-title">${recipe.strMeal || 'Unknown Recipe'}</h5>
                                        <p class="card-text">${recipe.strCategory || 'Unknown'} - ${recipe.strArea || 'Unknown'}</p>
                                        <a href="/Home/RecipeDetails?id=${recipe.idMeal}" class="btn btn-primary mt-auto">View Recipe</a>
                                    </div>
                                </div>
                            `;
                            row.appendChild(col);
                        });

                        // Update skip count for next load
                        loadMoreBtn.setAttribute('data-skip', skip + 9);

                        loadMoreBtn.disabled = false;
                        loadMoreBtn.textContent = 'Load More';
                    } else {
                        // No more recipes to load
                        loadMoreBtn.style.display = 'none';
                    }
                })
                .catch(error => {
                    console.error('Error loading more recipes:', error);
                    loadMoreBtn.disabled = false;
                    loadMoreBtn.textContent = 'Load More';
                });
        });
    }

    // Random Dish functionality
    const randomDishBtn = document.getElementById('random-dish');
    if (randomDishBtn) {
        randomDishBtn.addEventListener('click', function () {
            window.location.href = '/Home/RandomDish';
        });
    }

    // Restore loaded recipes from localStorage (only on index page)
    const recipesContainer = document.getElementById('recipes-container');
    if (recipesContainer && (window.location.pathname === '/' || window.location.pathname === '/Home' || window.location.pathname === '/Home/Index')) {
        const storedRecipes = localStorage.getItem('loadedRecipes');
        if (storedRecipes) {
            const recipes = JSON.parse(storedRecipes);
            recipes.forEach(recipe => {
                const col = document.createElement('div');
                col.className = 'col-md-4 mb-4';
                col.innerHTML = `
                    <div class="card h-100">
                        <img src="${recipe.strMealThumb || '/images/default-recipe.jpg'}" class="card-img-top" alt="${recipe.strMeal || 'Recipe'}" style="height: 200px; object-fit: cover;">
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title">${recipe.strMeal || 'Unknown Recipe'}</h5>
                            <p class="card-text">${recipe.strCategory || 'Unknown'} - ${recipe.strArea || 'Unknown'}</p>
                            <a href="/Home/RecipeDetails?id=${recipe.idMeal}" class="btn btn-primary mt-auto">View Recipe</a>
                        </div>
                    </div>
                `;
                recipesContainer.appendChild(col);
            });
        }
    } else {
        // Clear stored recipes if not on index page
        localStorage.removeItem('loadedRecipes');
    }
});
