using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CookingRecipesWeb.Services;
using CookingRecipesWeb.Models;

namespace CookingRecipesWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly RecipeService _recipeService;
        private readonly UserService _userService;

        public AdminController(RecipeService recipeService, UserService userService)
        {
            _recipeService = recipeService;
            _userService = userService;
        }

        // ======================
        // ADMIN INDEX
        // ======================
        [HttpGet]
        public IActionResult Index()
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Dashboard");
        }

        // ======================
        // ADMIN DASHBOARD
        // ======================
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        // ======================
        // MANAGE CATEGORIES
        // ======================
        [HttpGet]
        public async Task<IActionResult> ManageCategories()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var categories = await _recipeService.GetCategoriesAsync();
            return View(categories);
        }

        // ======================
        // MANAGE RECIPES
        // ======================
        [HttpGet]
        public async Task<IActionResult> ManageRecipes()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var apiRecipes = await _recipeService.GetApiRecipesAsync(9);
            var dbRecipes = await _recipeService.GetAllRecipesAsync();

            var viewModel = new ManageRecipesViewModel
            {
                ApiRecipes = apiRecipes,
                DbRecipes = dbRecipes
            };

            return View(viewModel);
        }

        // ======================
        // MANAGE USERS
        // ======================
        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        // ======================
        // MANAGE FAVORITES
        // ======================
        [HttpGet]
        public async Task<IActionResult> ManageFavorites()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var favorites = await _userService.GetAllFavoritesAsync();
            return View(favorites);
        }

        // ======================
        // REMOVE FAVORITE
        // ======================
        [HttpPost]
        public async Task<IActionResult> RemoveFavorite(Guid userId, string recipeId)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            await _userService.RemoveFavoriteAsync(userId, recipeId);
            return Json(new { success = true });
        }

        // ======================
        // CREATE RECIPE
        // ======================
        [HttpGet]
        public async Task<IActionResult> CreateRecipe()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var categories = await _recipeService.GetCategoriesAsync();
            ViewBag.Categories = categories;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe(Recipe recipe, List<string> SelectedCategoryNames)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // Check for duplicates in API
                var apiRecipes = await _recipeService.GetApiRecipesAsync(100); // Fetch more to check
                if (apiRecipes.Any(r => r.StrMeal?.ToLower() == recipe.StrMeal?.ToLower()))
                {
                    ModelState.AddModelError("", "A recipe with this name already exists in the API.");
                    return View(recipe);
                }

                // Generate new ID
                recipe.Id = Guid.NewGuid().ToString();

                var success = await _recipeService.CreateRecipeAsync(recipe);
                if (success)
                {
                    // Handle categories - create them if they don't exist in database
                    if (SelectedCategoryNames != null && SelectedCategoryNames.Any())
                    {
                        var categoryIds = new List<Guid>();

                        foreach (var categoryName in SelectedCategoryNames)
                        {
                            // Check if category exists in database
                            var allCategories = await _recipeService.GetCategoriesAsync();
                            var existingCategory = allCategories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

                            if (existingCategory != null)
                            {
                                categoryIds.Add(existingCategory.Id);
                            }
                            else
                            {
                                // Create new category in database
                                var newCategory = new Category
                                {
                                    Name = categoryName,
                                    CreatedAt = DateTime.UtcNow
                                };

                                var createSuccess = await _recipeService.CreateCategoryAsync(newCategory);
                                if (createSuccess)
                                {
                                    // Get the newly created category to get its ID
                                    var updatedCategories = await _recipeService.GetCategoriesAsync();
                                    var createdCategory = updatedCategories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
                                    if (createdCategory != null)
                                    {
                                        categoryIds.Add(createdCategory.Id);
                                    }
                                }
                            }
                        }

                        if (categoryIds.Any())
                        {
                            var categorySuccess = await _recipeService.CreateRecipeCategoriesAsync(recipe.Id, categoryIds);
                            if (!categorySuccess)
                            {
                                ModelState.AddModelError("", "Recipe created but failed to assign categories.");
                                return View(recipe);
                            }
                        }
                    }

                    return RedirectToAction("ManageRecipes");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create recipe.");
                }
            }

            return View(recipe);
        }

        // ======================
        // EDIT RECIPE
        // ======================
        [HttpGet]
        public async Task<IActionResult> EditRecipe(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            var categories = await _recipeService.GetCategoriesAsync();
            ViewBag.Categories = categories;

            return View(recipe);
        }

        [HttpPost]
        public async Task<IActionResult> EditRecipe(Recipe recipe, List<Guid> SelectedCategoryIds)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var success = await _recipeService.UpdateRecipeAsync(recipe);
                if (success && SelectedCategoryIds != null)
                {
                    var categorySuccess = await _recipeService.UpdateRecipeCategoriesAsync(recipe.Id, SelectedCategoryIds);
                    if (categorySuccess)
                    {
                        return RedirectToAction("ManageRecipes");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Recipe updated but failed to update categories.");
                        return View(recipe);
                    }
                }
                else if (success)
                {
                    return RedirectToAction("ManageRecipes");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update recipe.");
                }
            }

            return View(recipe);
        }

        // ======================
        // DELETE RECIPE
        // ======================
        [HttpPost]
        public async Task<IActionResult> DeleteRecipe(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var success = await _recipeService.DeleteRecipeAsync(id);
            return Json(new { success = success });
        }

        // ======================
        // GET CATEGORIES
        // ======================
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _recipeService.GetCategoriesAsync();
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ThumbnailUrl = c.ThumbnailUrl,
                CreatedAt = c.CreatedAt,
                IsFromApi = c.IsFromApi
            }).ToList();
            return Json(categoryDtos);
        }

        // ======================
        // GET AREAS
        // ======================
        [HttpGet]
        public async Task<IActionResult> GetAreas()
        {
            var areas = await _recipeService.GetAreasAsync();
            return Json(areas);
        }

        // ======================
        // UPDATE USER ROLE
        // ======================
        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(string userId, string role)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (role != "user" && role != "admin")
            {
                return Json(new { success = false, message = "Invalid role" });
            }

            var success = await _userService.UpdateUserRoleAsync(userId, role);
            return Json(new { success = success });
        }

        // ======================
        // CREATE CATEGORY
        // ======================
        [HttpGet]
        public IActionResult CreateCategory()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var success = await _recipeService.CreateCategoryAsync(category);
                if (success)
                {
                    return RedirectToAction("ManageCategories");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create category.");
                }
            }

            return View(category);
        }

        // ======================
        // EDIT CATEGORY
        // ======================
        [HttpGet]
        public async Task<IActionResult> EditCategory(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var category = await _recipeService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Check if category is from API
            if (category.IsFromApi)
            {
                TempData["Error"] = "API categories cannot be edited.";
                return RedirectToAction("ManageCategories");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Server-side validation: Check if category is from API
            var existingCategory = await _recipeService.GetCategoryByIdAsync(category.Id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            if (existingCategory.IsFromApi)
            {
                ModelState.AddModelError("", "API categories cannot be edited.");
                return View(category);
            }

            if (ModelState.IsValid)
            {
                var success = await _recipeService.UpdateCategoryAsync(category);
                if (success)
                {
                    return RedirectToAction("ManageCategories");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update category.");
                }
            }

            return View(category);
        }

        // ======================
        // DELETE CATEGORY
        // ======================
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var success = await _recipeService.DeleteCategoryAsync(id);
            if (!success)
            {
                return Json(new { success = false, message = "Failed to delete category. It may be an API-fetched category that cannot be deleted." });
            }

            return Json(new { success = true });
        }

        // ======================
        // CREATE CATEGORY AJAX
        // ======================
        [HttpPost]
        public async Task<IActionResult> CreateCategoryAjax([FromBody] Category category)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return Json(new { success = false, message = "Category name is required." });
            }

            // Check for duplicates (case-insensitive) against all categories
            var allCategories = await _recipeService.GetCategoriesAsync();
            if (allCategories.Any(c => c.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Json(new { success = false, message = "A category with this name already exists." });
            }

            // Set creation timestamp and generate ID
            category.Id = Guid.NewGuid();
            category.CreatedAt = DateTime.UtcNow;
            category.IsFromApi = false;

            var success = await _recipeService.CreateCategoryAsync(category);
            if (success)
            {
                return Json(new
                {
                    success = true,
                    category = new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        ThumbnailUrl = category.ThumbnailUrl,
                        CreatedAt = category.CreatedAt,
                        IsFromApi = category.IsFromApi
                    }
                });
            }
            else
            {
                return Json(new { success = false, message = "Failed to create category." });
            }
        }
    }
}
