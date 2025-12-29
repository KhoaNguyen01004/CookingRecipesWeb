using CookingRecipesWeb.Models;
using Supabase;
using System.Text.Json;

namespace CookingRecipesWeb.Services
{
    public class RecipeService
    {
        private readonly Client _supabase;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        // Static cache for API recipes
        private static List<Recipe>? _cachedApiRecipes;
        private static DateTime _cacheTimestamp;

        public RecipeService(Client supabase, HttpClient httpClient)
        {
            _supabase = supabase;
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // =======================
        // THEMEALDB API
        // =======================

        public async Task<List<Recipe>> GetRandomRecipesAsync(int count)
        {
            var recipes = new List<Recipe>();

            for (int i = 0; i < count; i++)
            {
                var response = await _httpClient.GetAsync(
                    "https://www.themealdb.com/api/json/v1/1/random.php"
                );
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<
                    MealDBResponse<MealDBRecipe>
                >(json, _jsonOptions);

                var meal = data?.Meals?.FirstOrDefault();
                if (meal != null)
                {
                    recipes.Add(MapMealDBRecipeToRecipe(meal));
                }
            }

            return recipes;
        }

        public async Task<List<Recipe>> GetRecipesByCategoryAsync(string category)
        {
            var response = await _httpClient.GetAsync(
                $"https://www.themealdb.com/api/json/v1/1/filter.php?c={category}"
            );
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<
                MealDBResponse<MealDBRecipe>
            >(json, _jsonOptions);

            var recipes = new List<Recipe>();

            if (data?.Meals == null) return recipes;

            foreach (var meal in data.Meals)
            {
                if (!string.IsNullOrEmpty(meal.IdMeal))
                {
                    var recipe = await GetRecipeByIdAsync(meal.IdMeal);
                    if (recipe != null)
                        recipes.Add(recipe);
                }
            }

            return recipes;
        }

        public async Task<Recipe?> GetRecipeByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync(
                $"https://www.themealdb.com/api/json/v1/1/lookup.php?i={id}"
            );
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // Check if the response contains valid data
            if (string.IsNullOrWhiteSpace(json) || json.Contains("\"meals\":null") || json.Contains("\"meals\": null"))
            {
                return null;
            }

            try
            {
                var data = JsonSerializer.Deserialize<
                    MealDBResponse<MealDBRecipe>
                >(json, _jsonOptions);

                var meal = data?.Meals?.FirstOrDefault();
                return meal == null ? null : MapMealDBRecipeToRecipe(meal);
            }
            catch (JsonException)
            {
                // If deserialization fails, return null
                return null;
            }
        }

        public async Task<List<Recipe>> SearchRecipesAsync(string query)
        {
            var response = await _httpClient.GetAsync(
                $"https://www.themealdb.com/api/json/v1/1/search.php?s={query}"
            );
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<
                MealDBResponse<MealDBRecipe>
            >(json, _jsonOptions);

            return data?.Meals?
                .Select(MapMealDBRecipeToRecipe)
                .ToList()
                ?? new List<Recipe>();
        }

        public async Task<List<MealDBCategory>> GetMealDBCategoriesAsync()
        {
            var response = await _httpClient.GetAsync(
                "https://www.themealdb.com/api/json/v1/1/categories.php"
            );
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<
                MealDBCategoryResponse
            >(json, _jsonOptions);

            return data?.Categories ?? new List<MealDBCategory>();
        }

        public async Task<List<string>> GetAreasAsync()
        {
            var response = await _httpClient.GetAsync(
                "https://www.themealdb.com/api/json/v1/1/list.php?a=list"
            );
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<
                MealDBAreasResponse
            >(json, _jsonOptions);

            return data?.Meals?
                .Select(m => m.StrArea)
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .ToList()
                ?? new List<string>();
        }

        public async Task<List<Recipe>> GetApiRecipesAsync(int count, bool forceRefresh = false)
        {
            // Check if cache is valid (less than 1 hour old) and not forcing refresh
            if (!forceRefresh && _cachedApiRecipes != null &&
                (DateTime.Now - _cacheTimestamp).TotalHours < 1 &&
                _cachedApiRecipes.Count >= count)
            {
                // Return cached recipes, take only the requested count
                return _cachedApiRecipes.Take(count).ToList();
            }

            // Fetch new recipes
            var recipes = await GetRandomRecipesAsync(count);

            // Update cache
            _cachedApiRecipes = recipes;
            _cacheTimestamp = DateTime.Now;

            return recipes;
        }

        public void ClearApiRecipesCache()
        {
            _cachedApiRecipes = null;
            _cacheTimestamp = DateTime.MinValue;
        }

        // =======================
        // SUPABASE - RECIPES
        // =======================

        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            var response = await _supabase.From<Recipe>().Get();
            return response.Models;
        }

        public async Task<bool> CreateRecipeAsync(Recipe recipe)
        {
            var response = await _supabase.From<Recipe>().Insert(recipe);
            return response.ResponseMessage?.IsSuccessStatusCode == true;
        }

        public async Task<bool> UpdateRecipeAsync(Recipe recipe)
        {
            var response = await _supabase
                .From<Recipe>()
                .Where(r => r.Id == recipe.Id)
                .Update(recipe);

            return response.ResponseMessage?.IsSuccessStatusCode == true;
        }

        public async Task<bool> DeleteRecipeAsync(string id)
        {
            await _supabase
                .From<Recipe>()
                .Where(r => r.Id == id)
                .Delete();

            return true;
        }

        public async Task<Recipe?> GetRecipeByIdFromDbAsync(string id)
        {
            var response = await _supabase
                .From<Recipe>()
                .Where(r => r.Id == id)
                .Get();

            return response.Models.FirstOrDefault();
        }

        // =======================
        // SUPABASE - CATEGORIES
        // =======================

        public async Task<List<Category>> GetCategoriesAsync()
        {
            // Fetch from Supabase
            var supabaseCategories = new List<Category>();
            try
            {
                var response = await _supabase.From<Category>().Get();
                supabaseCategories = response.Models;
            }
            catch
            {
                // If Supabase fails, continue with API
            }

            // Fetch from TheMealDB API
            var apiCategories = new List<Category>();
            try
            {
                var mealDBCategories = await GetMealDBCategoriesAsync();
                apiCategories = mealDBCategories.Select(c => new Category
                {
                    Id = Guid.NewGuid(), // Temporary Guid IDs
                    Name = c.StrCategory ?? "",
                    ThumbnailUrl = c.StrCategoryThumb,
                    CreatedAt = DateTime.UtcNow,
                    IsFromApi = true
                }).ToList();
            }
            catch
            {
                // If API fails, continue with Supabase
            }

            // Combine and remove duplicates by Name
            var allCategories = supabaseCategories.Concat(apiCategories)
                .GroupBy(c => c.Name)
                .Select(g => g.First())
                .ToList();

            return allCategories;
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            var response = await _supabase
                .From<Category>()
                .Where(c => c.Id == id)
                .Get();

            return response.Models.FirstOrDefault();
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            var response = await _supabase.From<Category>().Insert(category);
            return response.ResponseMessage?.IsSuccessStatusCode == true;
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            var response = await _supabase
                .From<Category>()
                .Where(c => c.Id == category.Id)
                .Update(category);

            return response.ResponseMessage?.IsSuccessStatusCode == true;
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            await _supabase
                .From<Category>()
                .Where(c => c.Id == id)
                .Delete();

            return true;
        }

        public async Task<bool> CreateRecipeCategoriesAsync(
            string recipeId,
            List<Guid> categoryIds
        )
        {
            var rows = categoryIds.Select(id => new RecipeCategory
            {
                RecipeId = recipeId,
                CategoryId = id
            }).ToList();

            var response = await _supabase
                .From<RecipeCategory>()
                .Insert(rows);

            return response.ResponseMessage?.IsSuccessStatusCode == true;
        }

        public async Task<bool> UpdateRecipeCategoriesAsync(
            string recipeId,
            List<Guid> categoryIds
        )
        {
            await _supabase
                .From<RecipeCategory>()
                .Where(rc => rc.RecipeId == recipeId)
                .Delete();

            return await CreateRecipeCategoriesAsync(recipeId, categoryIds);
        }

        // =======================
        // MAPPING
        // =======================

        private Recipe MapMealDBRecipeToRecipe(MealDBRecipe meal)
        {
            var ingredients = new List<string?>();
            var measures = new List<string?>();

            for (int i = 1; i <= 20; i++)
            {
                var ingredient = meal.GetType()
                    .GetProperty($"StrIngredient{i}")
                    ?.GetValue(meal) as string;

                var measure = meal.GetType()
                    .GetProperty($"StrMeasure{i}")
                    ?.GetValue(meal) as string;

                if (!string.IsNullOrWhiteSpace(ingredient))
                {
                    ingredients.Add(ingredient);
                    measures.Add(measure ?? "");
                }
            }

            return new Recipe
            {
                Id = meal.IdMeal ?? "",
                StrMeal = meal.StrMeal ?? "",
                StrCategory = meal.StrCategory,
                StrArea = meal.StrArea,
                StrInstructions = meal.StrInstructions,
                StrMealThumb = meal.StrMealThumb,
                StrTags = meal.StrTags,
                StrYoutube = meal.StrYoutube,
                StrIngredients = string.Join(",", ingredients.Where(i => i != null)),
                StrMeasures = string.Join(",", measures.Where(m => m != null)),
                StrSource = meal.StrSource,
                StrImageSource = meal.StrImageSource,
                StrCreativeCommonsConfirmed = meal.StrCreativeCommonsConfirmed,
                DateModified = meal.DateModified
            };
        }

        public RecipeDto MapRecipeToDto(Recipe recipe)
        {
            return new RecipeDto
            {
                Id = recipe.Id,
                StrMeal = recipe.StrMeal,
                StrCategory = recipe.StrCategory,
                StrArea = recipe.StrArea,
                StrInstructions = recipe.StrInstructions,
                StrMealThumb = recipe.StrMealThumb,
                StrTags = recipe.StrTags,
                StrYoutube = recipe.StrYoutube,
                StrIngredients = recipe.StrIngredients,
                StrMeasures = recipe.StrMeasures,
                StrSource = recipe.StrSource,
                StrImageSource = recipe.StrImageSource,
                StrCreativeCommonsConfirmed = recipe.StrCreativeCommonsConfirmed,
                DateModified = recipe.DateModified
            };
        }

        // =======================
        // RATINGS
        // =======================

        public async Task<List<Rating>> GetRatingsByRecipeIdAsync(string recipeId)
        {
            var response = await _supabase
                .From<Rating>()
                .Where(r => r.RecipeId == recipeId)
                .Get();

            return response.Models;
        }

        public async Task<Rating?> GetUserRatingAsync(Guid userId, string recipeId)
        {
            var response = await _supabase
                .From<Rating>()
                .Where(r => r.UserId == userId && r.RecipeId == recipeId)
                .Get();

            return response.Models.FirstOrDefault();
        }

        public async Task<bool> SubmitRatingAsync(Guid userId, string recipeId, int ratingValue)
        {
            var existingRating = await GetUserRatingAsync(userId, recipeId);
            if (existingRating != null)
            {
                // Update existing rating
                existingRating.RatingValue = ratingValue;
                existingRating.UpdatedAt = DateTime.UtcNow;

                var updateResponse = await _supabase
                    .From<Rating>()
                    .Where(r => r.Id == existingRating.Id)
                    .Update(existingRating);

                return updateResponse.ResponseMessage?.IsSuccessStatusCode == true;
            }
            else
            {
                // Create new rating
                var newRating = new Rating
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    RecipeId = recipeId,
                    RatingValue = ratingValue,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var insertResponse = await _supabase.From<Rating>().Insert(newRating);
                return insertResponse.ResponseMessage?.IsSuccessStatusCode == true;
            }
        }

        // =======================
        // REVIEWS
        // =======================

        public async Task<List<Review>> GetReviewsByRecipeIdAsync(string recipeId)
        {
            var response = await _supabase
                .From<Review>()
                .Where(r => r.RecipeId == recipeId)
                .Order(r => r.CreatedAt, Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();

            return response.Models;
        }

        public async Task<Review?> GetUserReviewAsync(Guid userId, string recipeId)
        {
            var response = await _supabase
                .From<Review>()
                .Where(r => r.UserId == userId && r.RecipeId == recipeId)
                .Get();

            return response.Models.FirstOrDefault();
        }

        public async Task<bool> SubmitReviewAsync(Guid userId, string recipeId, string reviewText)
        {
            var existingReview = await GetUserReviewAsync(userId, recipeId);
            if (existingReview != null)
            {
                // Update existing review
                existingReview.ReviewText = reviewText;
                existingReview.UpdatedAt = DateTime.UtcNow;

                var updateResponse = await _supabase
                    .From<Review>()
                    .Where(r => r.Id == existingReview.Id)
                    .Update(existingReview);

                return updateResponse.ResponseMessage?.IsSuccessStatusCode == true;
            }
            else
            {
                // Create new review
                var newReview = new Review
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    RecipeId = recipeId,
                    ReviewText = reviewText,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var insertResponse = await _supabase.From<Review>().Insert(newReview);
                return insertResponse.ResponseMessage?.IsSuccessStatusCode == true;
            }
        }

        public async Task<List<ReviewReply>> GetReviewRepliesByReviewIdAsync(Guid reviewId)
        {
            var response = await _supabase
                .From<ReviewReply>()
                .Where(r => r.ParentReviewId == reviewId)
                .Order(r => r.CreatedAt, Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            return response.Models;
        }

        public async Task<bool> SubmitReviewReplyAsync(Guid userId, Guid parentReviewId, string replyText)
        {
            var newReply = new ReviewReply
            {
                Id = Guid.NewGuid(),
                ParentReviewId = parentReviewId,
                UserId = userId,
                ReplyText = replyText,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var insertResponse = await _supabase.From<ReviewReply>().Insert(newReply);
            return insertResponse.ResponseMessage?.IsSuccessStatusCode == true;
        }

        public async Task<ReviewReply?> GetReviewReplyByIdAsync(Guid replyId)
        {
            var response = await _supabase
                .From<ReviewReply>()
                .Where(r => r.Id == replyId)
                .Get();

            return response.Models.FirstOrDefault();
        }

        public async Task<bool> DeleteReviewReplyAsync(Guid replyId, Guid userId)
        {
            var reply = await _supabase
                .From<ReviewReply>()
                .Where(r => r.Id == replyId && r.UserId == userId)
                .Get();

            if (!reply.Models.Any())
            {
                return false; // Reply not found or not owned by user
            }

            await _supabase
                .From<ReviewReply>()
                .Where(r => r.Id == replyId)
                .Delete();

            return true;
        }

        public async Task<bool> EditReviewAsync(Guid reviewId, Guid userId, string reviewText)
        {
            var review = await _supabase
                .From<Review>()
                .Where(r => r.Id == reviewId && r.UserId == userId)
                .Get();

            if (!review.Models.Any())
            {
                return false; // Review not found or not owned by user
            }

            var existingReview = review.Models.First();
            existingReview.ReviewText = reviewText;
            existingReview.UpdatedAt = DateTime.UtcNow;

            var updateResponse = await _supabase
                .From<Review>()
                .Where(r => r.Id == reviewId)
                .Update(existingReview);

            return updateResponse.ResponseMessage?.IsSuccessStatusCode == true;
        }

        public async Task<bool> DeleteReviewAsync(Guid reviewId, Guid userId)
        {
            var review = await _supabase
                .From<Review>()
                .Where(r => r.Id == reviewId && r.UserId == userId)
                .Get();

            if (!review.Models.Any())
            {
                return false; // Review not found or not owned by user
            }

            await _supabase
                .From<Review>()
                .Where(r => r.Id == reviewId)
                .Delete();

            return true;
        }
    }
}
