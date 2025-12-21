using System.Net.Http.Json;
using CookingRecipesWeb.Models;

namespace CookingRecipesWeb.Services
{
    public class RecipeService
    {
        private const string BaseUrl =
            "https://www.themealdb.com/api/json/v1/1/";

        private readonly HttpClient _httpClient;

        public RecipeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Recipe>> SearchRecipesAsync(string query)
        {
            var url = $"{BaseUrl}search.php?s={query}";

            var response = await _httpClient
                .GetFromJsonAsync<ApiResponse<Recipe>>(url);

            return response?.Meals ?? new List<Recipe>();
        }

        public async Task<Recipe?> GetRecipeByIdAsync(string id)
        {
            var url = $"{BaseUrl}lookup.php?i={id}";

            var response = await _httpClient
                .GetFromJsonAsync<ApiResponse<Recipe>>(url);

            return response?.Meals?.FirstOrDefault();
        }

        public async Task<List<Recipe>> GetRandomRecipesAsync(int count)
        {
            var recipes = new List<Recipe>();
            for (int i = 0; i < count; i++)
            {
                var recipe = await GetRandomRecipeAsync();
                if (recipe != null)
                {
                    recipes.Add(recipe);
                }
            }
            return recipes;
        }

        private async Task<Recipe?> GetRandomRecipeAsync()
        {
            var url = $"{BaseUrl}random.php";

            var response = await _httpClient
                .GetFromJsonAsync<ApiResponse<Recipe>>(url);

            return response?.Meals?.FirstOrDefault();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var url = $"{BaseUrl}categories.php";

            var response = await _httpClient
                .GetFromJsonAsync<ApiResponse<Category>>(url);

            return response?.Categories ?? new List<Category>();
        }

        public async Task<List<Recipe>> GetRecipesByCategoryAsync(string category)
        {
            var url = $"{BaseUrl}filter.php?c={category}";

            var response = await _httpClient
                .GetFromJsonAsync<ApiResponse<Recipe>>(url);

            return response?.Meals ?? new List<Recipe>();
        }
    }
}
