using CookingRecipesWeb.Models;
using CookingRecipesWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace CookingRecipesWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly RecipeService _recipeService;
        private readonly UserService _userService;
        private readonly IMemoryCache _cache;

        public HomeController(RecipeService recipeService, UserService userService, IMemoryCache cache)
        {
            _recipeService = recipeService;
            _userService = userService;
            _cache = cache;
        }

        public async Task<IActionResult> Index(string category)
        {
            List<Recipe> recipes;

            if (!string.IsNullOrEmpty(category))
            {
                string cacheKey = $"category_recipes_{category}";

                if (!_cache.TryGetValue(cacheKey, out recipes))
                {
                    recipes = await _recipeService.GetRecipesByCategoryAsync(category);

                    _cache.Set(
                        cacheKey,
                        recipes,
                        TimeSpan.FromMinutes(10)
                    );
                }

                ViewBag.Category = category;
                ViewBag.TotalRecipes = recipes != null ? recipes.Count : 0;
                // Take only first 9 recipes for initial display
                recipes = recipes != null ? recipes.Take(9).ToList() : new List<Recipe>();
            }
            else
            {
                const string cacheKey = "random_recipes";

                if (!_cache.TryGetValue(cacheKey, out recipes))
                {
                    recipes = await _recipeService.GetApiRecipesAsync(9);

                    _cache.Set(
                        cacheKey,
                        recipes,
                        TimeSpan.FromMinutes(5)
                    );
                }
            }

            return View(recipes);
        }

        [HttpGet]
        public async Task<IActionResult> RecipeDetails(string id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        public async Task<IActionResult> Categories()
        {
            const string cacheKey = "meal_categories";

            if (!_cache.TryGetValue(cacheKey, out List<Category> categories))
            {
                categories = await _recipeService.GetCategoriesAsync();
                _cache.Set(cacheKey, categories, TimeSpan.FromHours(1));
            }

            return View(categories);
        }

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction("Index");
            }
            var recipes = await _recipeService.SearchRecipesAsync(query);
            ViewBag.SearchQuery = query;
            return View("Index", recipes);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreRecipes(string category, int skip = 0)
        {
            List<Recipe> recipes;

            if (!string.IsNullOrEmpty(category))
            {
                string cacheKey = $"category_recipes_{category}";
                List<Recipe> allRecipes;

                if (!_cache.TryGetValue(cacheKey, out allRecipes))
                {
                    allRecipes = await _recipeService.GetRecipesByCategoryAsync(category);
                    _cache.Set(cacheKey, allRecipes, TimeSpan.FromMinutes(10));
                }

                recipes = allRecipes != null ? allRecipes.Skip(skip).Take(9).ToList() : new List<Recipe>();
            }
            else
            {
                recipes = await _recipeService.GetRandomRecipesAsync(9);
            }

            var recipeDtos = recipes.Select(r => _recipeService.MapRecipeToDto(r)).ToList();
            return Json(recipeDtos);
        }

        public async Task<IActionResult> RandomDish()
        {
            // Force refresh when getting random dish
            var recipes = await _recipeService.GetApiRecipesAsync(1, forceRefresh: true);
            if (recipes != null && recipes.Any())
            {
                return RedirectToAction("RecipeDetails", new { id = recipes.First().Id });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(string recipeId)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString))
                {
                    return Json(new { success = false, message = "User not logged in" });
                }

                if (!Guid.TryParse(userIdString, out var userId))
                {
                    return Json(new { success = false, message = "Invalid user session" });
                }

                if (string.IsNullOrEmpty(recipeId))
                {
                    return Json(new { success = false, message = "Invalid recipe ID" });
                }

                var isFavorite = await _userService.IsFavoriteAsync(userId, recipeId);

                if (isFavorite)
                {
                    await _userService.RemoveFavoriteAsync(userId, recipeId);
                    return Json(new { success = true, isFavorite = false });
                }
                else
                {
                    await _userService.AddFavoriteAsync(userId, recipeId);
                    return Json(new { success = true, isFavorite = true });
                }
            }
            catch (Exception ex)
            {
                // Log the error (you might want to use a proper logging framework)
                Console.WriteLine($"Error in ToggleFavorite: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating favorites" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> IsFavorite(string recipeId)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return Json(false);
            }

            var userId = Guid.Parse(userIdString);
            var isFavorite = await _userService.IsFavoriteAsync(userId, recipeId);
            return Json(isFavorite);
        }

        public async Task<IActionResult> MyFavorites()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var favorites = await _userService.GetUserFavoritesAsync(userId);

            var favoriteRecipeIds = favorites.Select(f => f.RecipeId).ToList();
            var favoriteRecipes = new List<Recipe>();

            foreach (var recipeId in favoriteRecipeIds)
            {
                var recipe = await _recipeService.GetRecipeByIdAsync(recipeId);
                if (recipe != null)
                {
                    favoriteRecipes.Add(recipe);
                }
            }

            return View(favoriteRecipes);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
