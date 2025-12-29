using Microsoft.AspNetCore.Mvc;
using CookingRecipesWeb.Models;
using CookingRecipesWeb.Services;

namespace CookingRecipesWeb.Controllers
{
    [ApiController]
    [Route("api/recipe")]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeService _recipeService;
        private readonly UserService _userService;

        public RecipeController(RecipeService recipeService, UserService userService)
        {
            _recipeService = recipeService;
            _userService = userService;
        }

        // GET /api/recipe/{id}/rating-summary
        [HttpGet("{id}/rating-summary")]
        public async Task<IActionResult> GetRatingSummary(string id)
        {
            try
            {
                var ratings = await _recipeService.GetRatingsByRecipeIdAsync(id);
                if (!ratings.Any())
                {
                    return Ok(new { average = 0.0, total = 0 });
                }

                var average = ratings.Average(r => r.RatingValue);
                return Ok(new { average = Math.Round(average, 1), total = ratings.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET /api/recipe/{id}/user-rating
        [HttpGet("{id}/user-rating")]
        public async Task<IActionResult> GetUserRating(string id)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized(new { error = "User not logged in" });
                }

                var rating = await _recipeService.GetUserRatingAsync(userId, id);
                if (rating == null)
                {
                    return Ok(new { rating = 0 });
                }

                return Ok(new { rating = rating.RatingValue });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST /api/recipe/{id}/rating
        [HttpPost("{id}/rating")]
        public async Task<IActionResult> SubmitRating(string id, [FromBody] RatingRequest request)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized(new { error = "User not logged in" });
                }

                if (request.Rating < 1 || request.Rating > 5)
                {
                    return BadRequest(new { error = "Rating must be between 1 and 5" });
                }

                var success = await _recipeService.SubmitRatingAsync(userId, id, request.Rating);
                if (!success)
                {
                    return StatusCode(500, new { error = "Failed to submit rating" });
                }

                return Ok(new { message = "Rating submitted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET /api/recipe/{id}/reviews
        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetReviews(string id)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                Guid? currentUserId = null;
                if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out Guid parsedUserId))
                {
                    currentUserId = parsedUserId;
                }

                var reviews = new List<Review>();
                try
                {
                    reviews = await _recipeService.GetReviewsByRecipeIdAsync(id);
                }
                catch (Exception ex)
                {
                    // Log the error and continue with empty reviews
                    Console.WriteLine($"Error getting reviews for recipe {id}: {ex.Message}");
                }
                var reviewDtos = new List<ReviewDto>();

                foreach (var review in reviews)
                {
                    User? user = null;
                    try
                    {
                        user = await _userService.GetUserByIdAsync(review.UserId);
                    }
                    catch (Exception ex)
                    {
                        // Log the error and continue with anonymous user
                        Console.WriteLine($"Error getting user for review {review.Id}: {ex.Message}");
                    }

                    var replyDtos = new List<ReviewReplyDto>();
                    try
                    {
                        var replies = await _recipeService.GetReviewRepliesByReviewIdAsync(review.Id);

                        foreach (var reply in replies)
                        {
                            User? replyUser = null;
                            try
                            {
                                replyUser = await _userService.GetUserByIdAsync(reply.UserId);
                            }
                            catch (Exception ex)
                            {
                                // Log the error and continue with anonymous user
                                Console.WriteLine($"Error getting user for reply {reply.Id}: {ex.Message}");
                            }

                            replyDtos.Add(new ReviewReplyDto
                            {
                                Id = reply.Id,
                                ParentReviewId = reply.ParentReviewId,
                                UserId = reply.UserId,
                                ReplyText = reply.ReplyText,
                                CreatedAt = reply.CreatedAt,
                                UpdatedAt = reply.UpdatedAt,
                                UserName = replyUser != null ? $"{replyUser.FirstName} {replyUser.LastName}" : "Anonymous",
                                IsOwner = currentUserId.HasValue && reply.UserId == currentUserId.Value
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error and continue without replies
                        Console.WriteLine($"Error getting replies for review {review.Id}: {ex.Message}");
                    }

                    var reviewDto = new ReviewDto
                    {
                        Id = review.Id,
                        UserId = review.UserId,
                        RecipeId = review.RecipeId,
                        ReviewText = review.ReviewText,
                        CreatedAt = review.CreatedAt,
                        UpdatedAt = review.UpdatedAt,
                        UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Anonymous",
                        IsOwner = currentUserId.HasValue && review.UserId == currentUserId.Value,
                        Replies = replyDtos
                    };
                    reviewDtos.Add(reviewDto);
                }

                return Ok(reviewDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST /api/recipe/{id}/review
        [HttpPost("{id}/review")]
        public async Task<IActionResult> SubmitReview(string id, [FromBody] ReviewRequest request)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized(new { error = "User not logged in" });
                }

                if (string.IsNullOrWhiteSpace(request.ReviewText))
                {
                    return BadRequest(new { error = "Review text is required" });
                }

                var success = await _recipeService.SubmitReviewAsync(userId, id, request.ReviewText);
                if (!success)
                {
                    return StatusCode(500, new { error = "Failed to submit review" });
                }

                return Ok(new { message = "Review submitted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST /api/recipe/review/{reviewId}/reply
        [HttpPost("review/{reviewId}/reply")]
        public async Task<IActionResult> SubmitReviewReply(Guid reviewId, [FromBody] ReviewReplyRequest request)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized(new { error = "User not logged in" });
                }

                if (string.IsNullOrWhiteSpace(request.ReplyText))
                {
                    return BadRequest(new { error = "Reply text is required" });
                }

                var success = await _recipeService.SubmitReviewReplyAsync(userId, reviewId, request.ReplyText);
                if (!success)
                {
                    return StatusCode(500, new { error = "Failed to submit reply" });
                }

                return Ok(new { message = "Reply submitted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // PUT /api/recipe/review/{reviewId}
        [HttpPut("review/{reviewId}")]
        public async Task<IActionResult> EditReview(Guid reviewId, [FromBody] ReviewRequest request)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized(new { error = "User not logged in" });
                }

                if (string.IsNullOrWhiteSpace(request.ReviewText))
                {
                    return BadRequest(new { error = "Review text is required" });
                }

                var success = await _recipeService.EditReviewAsync(reviewId, userId, request.ReviewText);
                if (!success)
                {
                    return NotFound(new { error = "Review not found or you don't have permission to edit it" });
                }

                return Ok(new { message = "Review updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DELETE /api/recipe/review/{reviewId}
        [HttpDelete("review/{reviewId}")]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized(new { error = "User not logged in" });
                }

                var success = await _recipeService.DeleteReviewAsync(reviewId, userId);
                if (!success)
                {
                    return NotFound(new { error = "Review not found or you don't have permission to delete it" });
                }

                return Ok(new { message = "Review deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DELETE /api/recipe/reply/{replyId}
        [HttpDelete("reply/{replyId}")]
        public async Task<IActionResult> DeleteReply(Guid replyId)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized(new { error = "User not logged in" });
                }

                // For simplicity, we'll check if the reply exists and belongs to the user
                // In a real app, you'd have a service method for this
                var reply = await _recipeService.GetReviewReplyByIdAsync(replyId);
                if (reply == null || reply.UserId != userId)
                {
                    return NotFound(new { error = "Reply not found or you don't have permission to delete it" });
                }

                var success = await _recipeService.DeleteReviewReplyAsync(replyId, userId);
                if (!success)
                {
                    return NotFound(new { error = "Reply not found or you don't have permission to delete it" });
                }
                return Ok(new { message = "Reply deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class RatingRequest
    {
        public int Rating { get; set; }
    }

    public class ReviewRequest
    {
        public string ReviewText { get; set; } = string.Empty;
    }

    public class ReviewReplyRequest
    {
        public string ReplyText { get; set; } = string.Empty;
    }

    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string RecipeId { get; set; } = string.Empty;
        public string ReviewText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsOwner { get; set; }
        public List<ReviewReplyDto> Replies { get; set; } = new List<ReviewReplyDto>();
    }

    public class ReviewReplyDto
    {
        public Guid Id { get; set; }
        public Guid ParentReviewId { get; set; }
        public Guid UserId { get; set; }
        public string ReplyText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsOwner { get; set; }
    }
}
