using CookingRecipesWeb.Models;
using Supabase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CookingRecipesWeb.Services
{
    public class UserService
    {
        private readonly Supabase.Client _client;
        private readonly Supabase.Client? _adminClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(Supabase.Client client, IConfiguration configuration, ILogger<UserService> logger)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;

            // Create admin client for operations that need to bypass RLS
            var serviceRoleKey = _configuration["Supabase:ServiceRoleKey"];
            if (!string.IsNullOrEmpty(serviceRoleKey))
            {
                _logger.LogInformation("Service role key found, creating admin client");
                var options = new SupabaseOptions
                {
                    AutoConnectRealtime = false
                };
                _adminClient = new Supabase.Client(
                    _configuration["Supabase:Url"],
                    serviceRoleKey,
                    options
                );
                _logger.LogInformation("Admin client created successfully");
            }
            else
            {
                _logger.LogWarning("Service role key not found, admin client will not be available");
                _adminClient = null;
            }
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _client.Postgrest
                .Table<User>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId)
                .Single();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _client.Postgrest
                .Table<User>()
                .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
                .Single();
        }

        public async Task CreateUserAsync(User user, string accessToken = null, string refreshToken = null)
        {
            // Use admin client for user creation to bypass RLS policies
            var clientToUse = _adminClient ?? _client;

            _logger.LogInformation($"Creating user profile for {user.Email} using {(clientToUse == _adminClient ? "admin" : "regular")} client");

            if (!string.IsNullOrEmpty(accessToken) && clientToUse == _client)
            {
                await _client.Auth.SetSession(accessToken, refreshToken);
            }

            try
            {
                await clientToUse.Postgrest
                    .Table<User>()
                    .Insert(user);
                _logger.LogInformation($"Successfully created user profile for {user.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create user profile for {user.Email}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Favorite>> GetUserFavoritesAsync(Guid userId, string accessToken = null, string refreshToken = null)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                await _client.Auth.SetSession(accessToken, refreshToken);
            }
            var res = await _client.Postgrest
                .Table<Favorite>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Get();

            return res.Models;
        }

        public async Task AddFavoriteAsync(Guid userId, string recipeId, string accessToken = null, string refreshToken = null)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                await _client.Auth.SetSession(accessToken, refreshToken);
            }
            await _client.Postgrest
                .Table<Favorite>()
                .Insert(new Favorite
                {
                    UserId = userId,
                    RecipeId = recipeId
                });
        }

        public async Task RemoveFavoriteAsync(Guid userId, string recipeId, string accessToken = null, string refreshToken = null)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                await _client.Auth.SetSession(accessToken, refreshToken);
            }
            await _client.Postgrest
                .Table<Favorite>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("recipe_id", Supabase.Postgrest.Constants.Operator.Equals, recipeId)
                .Delete();
        }

        public async Task<bool> IsFavoriteAsync(Guid userId, string recipeId, string accessToken = null, string refreshToken = null)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                await _client.Auth.SetSession(accessToken, refreshToken);
            }
            var res = await _client.Postgrest
                .Table<Favorite>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("recipe_id", Supabase.Postgrest.Constants.Operator.Equals, recipeId)
                .Get();

            return res.Models.Any();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var res = await _client.Postgrest
                .Table<User>()
                .Get();

            return res.Models;
        }

        public async Task<List<Favorite>> GetAllFavoritesAsync()
        {
            var res = await _client.Postgrest
                .Table<Favorite>()
                .Get();

            return res.Models;
        }

        public async Task<bool> UpdateUserRoleAsync(string userId, string role)
        {
            try
            {
                await _client.Postgrest
                    .Table<User>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId)
                    .Set(u => u.Role, role)
                    .Update();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
