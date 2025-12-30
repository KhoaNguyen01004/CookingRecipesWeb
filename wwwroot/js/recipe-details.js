document.addEventListener('DOMContentLoaded', function () {
    const favoriteBtn = document.getElementById('favorite-btn');
    const favoriteText = document.getElementById('favorite-text');
    const recipeId = window.recipeId;

    // =======================
    // FAVORITES FUNCTIONALITY
    // =======================

    if (favoriteBtn) {
        // Check initial favorite status
        fetch('/Home/IsFavorite?recipeId=' + recipeId)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(isFavorite => {
                updateFavoriteButton(isFavorite);
            })
            .catch(error => {
                console.error('Error checking favorite status:', error);
            });

        favoriteBtn.addEventListener('click', function () {
            fetch('/Home/ToggleFavorite', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: 'recipeId=' + recipeId
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }
                    return response.json();
                })
                .then(data => {
                    if (data.success) {
                        updateFavoriteButton(data.isFavorite);
                    } else {
                        alert('Error: ' + (data.message || 'Failed to toggle favorite'));
                    }
                })
                .catch(error => {
                    console.error('Error toggling favorite:', error);
                    alert('Failed to toggle favorite. Please try again.');
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

    // =======================
    // RATINGS & REVIEWS FUNCTIONALITY
    // =======================

    console.log('Initializing ratings and reviews functionality...');

    // Load initial data
    loadRatingSummary();
    loadUserRating();
    loadReviews();

    // Rating stars interaction
    const userRatingStars = document.getElementById('user-rating-stars');
    let currentUserRating = 0;
    let hoveredRating = 0;

    if (userRatingStars) {
        // Create 5 stars for rating
        for (let i = 1; i <= 5; i++) {
            const star = document.createElement('i');
            star.className = 'bi bi-star';
            star.dataset.rating = i;
            star.style.cursor = 'pointer';
            star.style.fontSize = '1.5rem';
            star.style.color = '#666';
            star.style.transition = 'color 0.2s ease';

            star.addEventListener('click', function () {
                submitRating(i);
            });

            star.addEventListener('mouseover', function () {
                hoveredRating = i;
                updateStarDisplay();
            });

            star.addEventListener('mouseout', function () {
                hoveredRating = 0;
                updateStarDisplay();
            });

            userRatingStars.appendChild(star);
        }
    }

    // Review submission
    const submitReviewBtn = document.getElementById('submit-review-btn');
    const reviewText = document.getElementById('review-text');
    const reviewStatus = document.getElementById('review-status');

    if (submitReviewBtn && reviewText) {
        submitReviewBtn.addEventListener('click', function () {
            const text = reviewText.value.trim();
            if (text.length === 0) {
                showReviewStatus('Please enter a review text.', 'danger');
                return;
            }

            submitReview(text);
        });
    }

    // =======================
    // FUNCTIONS
    // =======================

    function loadRatingSummary() {
        fetch(`/api/recipe/${recipeId}/rating-summary`)
            .then(response => response.json())
            .then(data => {
                document.getElementById('average-rating').textContent = data.average.toFixed(1);
                document.getElementById('total-ratings').textContent = `(${data.total} ratings)`;

                // Update stars display
                const ratingStars = document.getElementById('rating-stars');
                ratingStars.innerHTML = '';
                const fullStars = Math.floor(data.average);
                const hasHalfStar = data.average % 1 >= 0.5;

                for (let i = 1; i <= 5; i++) {
                    const star = document.createElement('i');
                    star.style.fontSize = '1.2rem';
                    if (i <= fullStars) {
                        star.className = 'bi bi-star-fill text-warning';
                    } else if (i === fullStars + 1 && hasHalfStar) {
                        star.className = 'bi bi-star-half text-warning';
                    } else {
                        star.className = 'bi bi-star text-warning';
                    }
                    ratingStars.appendChild(star);
                }
            })
            .catch(error => console.error('Error loading rating summary:', error));
    }

    function loadUserRating() {
        fetch(`/api/recipe/${recipeId}/user-rating`)
            .then(response => {
                if (response.status === 401) return; // User not logged in
                return response.json();
            })
            .then(data => {
                if (data && data.rating > 0) {
                    updateUserRatingDisplay(data.rating);
                }
            })
            .catch(error => console.error('Error loading user rating:', error));
    }

    function submitRating(rating) {
        fetch(`/api/recipe/${recipeId}/rating`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ rating: rating })
        })
            .then(response => response.json())
            .then(data => {
                if (data.message) {
                    currentUserRating = rating;
                    updateStarDisplay();
                    updateUserRatingText();
                    loadRatingSummary(); // Refresh the summary
                } else if (data.error) {
                    alert('Error: ' + data.error);
                }
            })
            .catch(error => console.error('Error submitting rating:', error));
    }

    function updateUserRatingDisplay(rating = 0) {
        currentUserRating = rating;
        updateStarDisplay();
        updateUserRatingText();
    }

    function updateStarDisplay() {
        const stars = userRatingStars.querySelectorAll('i');
        const displayRating = hoveredRating > 0 ? hoveredRating : currentUserRating;

        stars.forEach((star, index) => {
            if (index < displayRating) {
                star.style.color = '#ffc107'; // Gold color
                star.className = 'bi bi-star-fill';
            } else {
                star.style.color = '#666';
                star.className = 'bi bi-star';
            }
        });
    }

    function updateUserRatingText() {
        const display = document.getElementById('user-rating-display');

        if (currentUserRating > 0) {
            display.textContent = `You rated this recipe ${currentUserRating} star${currentUserRating > 1 ? 's' : ''}.`;
        } else {
            display.textContent = "You haven't rated this recipe yet.";
        }
    }

    function highlightStars(rating) {
        // This function is now handled by updateStarDisplay
    }

    function loadReviews() {
        fetch(`/api/recipe/${recipeId}/reviews`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(reviews => {
                const container = document.getElementById('reviews-container');
                container.innerHTML = '';

                if (reviews.length === 0) {
                    container.innerHTML = '<p class="text-muted">No reviews yet. Be the first to review this recipe!</p>';
                    return;
                }

                reviews.forEach(review => {
                    const reviewDiv = document.createElement('div');
                    reviewDiv.className = 'border-bottom pb-3 mb-3';
                    reviewDiv.setAttribute('data-review-id', review.id);

                    let actionButtons = '';
                    const isLoggedIn = document.getElementById('user-rating-stars') !== null;
                    if (isLoggedIn) {
                        actionButtons = `<button class="btn btn-sm btn-outline-secondary reply-btn" data-review-id="${review.id}">Reply</button>`;
                    }
                    if (review.isOwner) {
                        actionButtons += `
                            <button class="btn btn-sm btn-outline-primary edit-review-btn ms-2" data-review-id="${review.id}">Edit</button>
                            <button class="btn btn-sm btn-outline-danger delete-review-btn ms-2" data-review-id="${review.id}">Delete</button>
                        `;
                    }

                    let repliesHtml = '';
                    if (review.replies && review.replies.length > 0) {
                        repliesHtml = '<div class="replies mt-3 ms-4">';
                        review.replies.forEach(reply => {
                            let replyActionButtons = '';
                            if (isLoggedIn) {
                                replyActionButtons = `<button class="btn btn-sm btn-outline-secondary reply-to-reply-btn" data-review-id="${review.id}">Reply</button>`;
                            }
                            if (reply.isOwner) {
                                replyActionButtons += `
                                    <button class="btn btn-sm btn-outline-primary edit-reply-btn ms-2" data-reply-id="${reply.id}">Edit</button>
                                    <button class="btn btn-sm btn-outline-danger delete-reply-btn ms-2" data-reply-id="${reply.id}">Delete</button>
                                `;
                            }

                            repliesHtml += `
                                <div class="reply border-start ps-3 mb-2" data-reply-id="${reply.id}">
                                    <div class="d-flex justify-content-between align-items-start">
                                        <div>
                                            <strong>${reply.userName || 'Anonymous'}</strong>
                                            <small class="text-muted ms-2">${new Date(reply.createdAt).toLocaleDateString()}</small>
                                        </div>
                                        <div>
                                            ${replyActionButtons}
                                        </div>
                                    </div>
                                    <p class="mt-1 mb-0">${reply.replyText}</p>
                                </div>
                            `;
                        });
                        repliesHtml += '</div>';
                    }

                    reviewDiv.innerHTML = `
                        <div class="d-flex justify-content-between align-items-start">
                            <div>
                                <strong>${review.userName || 'Anonymous'}</strong>
                                <small class="text-muted ms-2">${new Date(review.createdAt).toLocaleDateString()}</small>
                            </div>
                            <div>
                                ${actionButtons}
                            </div>
                        </div>
                        <p class="mt-2 mb-2">${review.reviewText}</p>
                        ${repliesHtml}
                    `;
                    container.appendChild(reviewDiv);
                });

                // Attach event listeners after adding to DOM
                attachReviewEventListeners();
            })
            .catch(error => {
                console.error('Error loading reviews:', error);
                const container = document.getElementById('reviews-container');
                container.innerHTML = '<p class="text-danger">Error loading reviews. Please try refreshing the page.</p>';
            });
    }

    function submitReview(reviewText) {
        fetch(`/api/recipe/${recipeId}/review`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + getAuthToken()
            },
            body: JSON.stringify({ reviewText: reviewText })
        })
            .then(response => response.json())
            .then(data => {
                if (data.message) {
                    showReviewStatus('Review submitted successfully!', 'success');
                    reviewText.value = '';
                    loadReviews(); // Refresh reviews
                } else if (data.error) {
                    showReviewStatus('Error: ' + data.error, 'danger');
                }
            })
            .catch(error => {
                console.error('Error submitting review:', error);
                showReviewStatus('Error submitting review. Please try again.', 'danger');
            });
    }

    function showReviewStatus(message, type) {
        reviewStatus.innerHTML = `<div class="alert alert-${type} mt-2">${message}</div>`;
        setTimeout(() => {
            reviewStatus.innerHTML = '';
        }, 5000);
    }

    function getAuthToken() {
        // This should be implemented based on your authentication system
        // For now, return empty string - the API should handle unauthorized requests
        return '';
    }

    // =======================
    // REVIEW ACTIONS
    // =======================

    function attachReviewEventListeners() {
        // Edit review buttons
        document.querySelectorAll('.edit-review-btn').forEach(btn => {
            btn.addEventListener('click', function () {
                const reviewId = this.getAttribute('data-review-id');
                const reviewDiv = this.closest('[data-review-id]');
                const reviewText = reviewDiv.querySelector('p').textContent;

                document.getElementById('edit-review-text').value = reviewText;
                document.getElementById('save-edit-review-btn').setAttribute('data-review-id', reviewId);

                const modal = new bootstrap.Modal(document.getElementById('editReviewModal'));
                modal.show();
            });
        });

        // Delete review buttons
        document.querySelectorAll('.delete-review-btn').forEach(btn => {
            btn.addEventListener('click', function () {
                const reviewId = this.getAttribute('data-review-id');
                if (confirm('Are you sure you want to delete this review?')) {
                    deleteReview(reviewId);
                }
            });
        });

        // Reply buttons
        document.querySelectorAll('.reply-btn').forEach(btn => {
            btn.addEventListener('click', function () {
                const reviewId = this.getAttribute('data-review-id');
                document.getElementById('submit-reply-btn').setAttribute('data-review-id', reviewId);

                const modal = new bootstrap.Modal(document.getElementById('replyModal'));
                modal.show();
            });
        });

        // Delete reply buttons
        document.querySelectorAll('.delete-reply-btn').forEach(btn => {
            btn.addEventListener('click', function () {
                const replyId = this.getAttribute('data-reply-id');
                if (confirm('Are you sure you want to delete this reply?')) {
                    deleteReply(replyId);
                }
            });
        });

        // Edit reply buttons
        document.querySelectorAll('.edit-reply-btn').forEach(btn => {
            btn.addEventListener('click', function () {
                const replyId = this.getAttribute('data-reply-id');
                const replyDiv = this.closest('.reply');
                const replyText = replyDiv.querySelector('p').textContent;

                document.getElementById('edit-reply-text').value = replyText;
                document.getElementById('save-edit-reply-btn').setAttribute('data-reply-id', replyId);

                const modal = new bootstrap.Modal(document.getElementById('editReplyModal'));
                modal.show();
            });
        });

        // Reply to reply buttons (replies to parent review)
        document.querySelectorAll('.reply-to-reply-btn').forEach(btn => {
            btn.addEventListener('click', function () {
                const reviewId = this.getAttribute('data-review-id');
                document.getElementById('submit-reply-btn').setAttribute('data-review-id', reviewId);

                const modal = new bootstrap.Modal(document.getElementById('replyModal'));
                modal.show();
            });
        });
    }

    // Edit review functionality
    document.getElementById('save-edit-review-btn').addEventListener('click', function () {
        const reviewId = this.getAttribute('data-review-id');
        const reviewText = document.getElementById('edit-review-text').value.trim();

        if (reviewText.length === 0) {
            alert('Please enter review text.');
            return;
        }

        fetch(`/api/recipe/review/${reviewId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ reviewText: reviewText })
        })
            .then(response => response.json())
            .then(data => {
                if (data.message) {
                    const modal = bootstrap.Modal.getInstance(document.getElementById('editReviewModal'));
                    modal.hide();
                    loadReviews(); // Refresh reviews
                } else if (data.error) {
                    alert('Error: ' + data.error);
                }
            })
            .catch(error => {
                console.error('Error editing review:', error);
                alert('Error editing review. Please try again.');
            });
    });

    // Submit reply functionality
    document.getElementById('submit-reply-btn').addEventListener('click', function () {
        const reviewId = this.getAttribute('data-review-id');
        const replyText = document.getElementById('reply-text').value.trim();

        if (replyText.length === 0) {
            alert('Please enter reply text.');
            return;
        }

        fetch(`/api/recipe/review/${reviewId}/reply`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ replyText: replyText })
        })
            .then(response => response.json())
            .then(data => {
                if (data.message) {
                    document.getElementById('reply-text').value = '';
                    const modal = bootstrap.Modal.getInstance(document.getElementById('replyModal'));
                    modal.hide();
                    loadReviews(); // Refresh reviews
                } else if (data.error) {
                    alert('Error: ' + data.error);
                }
            })
            .catch(error => {
                console.error('Error submitting reply:', error);
                alert('Error submitting reply. Please try again.');
            });
    });

    function deleteReview(reviewId) {
        fetch(`/api/recipe/review/${reviewId}`, {
            method: 'DELETE'
        })
            .then(response => response.json())
            .then(data => {
                if (data.message) {
                    loadReviews(); // Refresh reviews
                } else if (data.error) {
                    alert('Error: ' + data.error);
                }
            })
            .catch(error => {
                console.error('Error deleting review:', error);
                alert('Error deleting review. Please try again.');
            });
    }

    function deleteReply(replyId) {
        fetch(`/api/recipe/reply/${replyId}`, {
            method: 'DELETE'
        })
            .then(response => response.json())
            .then(data => {
                if (data.message) {
                    loadReviews(); // Refresh reviews
                } else if (data.error) {
                    alert('Error: ' + data.error);
                }
            })
            .catch(error => {
                console.error('Error deleting reply:', error);
                alert('Error deleting reply. Please try again.');
            });
    }

    // Edit reply functionality
    document.getElementById('save-edit-reply-btn').addEventListener('click', function () {
        const replyId = this.getAttribute('data-reply-id');
        const replyText = document.getElementById('edit-reply-text').value.trim();

        if (replyText.length === 0) {
            alert('Please enter reply text.');
            return;
        }

        fetch(`/api/recipe/reply/${replyId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ replyText: replyText })
        })
            .then(response => response.json())
            .then(data => {
                if (data.message) {
                    const modal = bootstrap.Modal.getInstance(document.getElementById('editReplyModal'));
                    modal.hide();
                    loadReviews(); // Refresh reviews
                } else if (data.error) {
                    alert('Error: ' + data.error);
                }
            })
            .catch(error => {
                console.error('Error editing reply:', error);
                alert('Error editing reply. Please try again.');
            });
    });
});
