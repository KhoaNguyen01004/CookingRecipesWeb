document.addEventListener('DOMContentLoaded', function () {
    const favoriteBtn = document.getElementById('favorite-btn');
    const favoriteText = document.getElementById('favorite-text');
    const recipeId = '@Model?.Id';

    if (favoriteBtn) {
        // Check initial favorite status
        fetch('/Home/IsFavorite?recipeId=' + recipeId)
            .then(response => response.json())
            .then(isFavorite => {
                updateFavoriteButton(isFavorite);
            });

        favoriteBtn.addEventListener('click', function () {
            fetch('/Home/ToggleFavorite', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: 'recipeId=' + recipeId
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        updateFavoriteButton(data.isFavorite);
                    }
                });
        });
    }

    function updateFavoriteButton(isFavorite) {
        if (isFavorite) {
            favoriteBtn.classList.remove('btn-outline-warning');
            favoriteBtn.classList.add('btn-warning');
            favoriteText.textContent = 'Remove from Favorites';
        } else {
            favoriteBtn.classList.remove('btn-warning');
            favoriteBtn.classList.add('btn-outline-warning');
            favoriteText.textContent = 'Add to Favorites';
        }
    }
});
