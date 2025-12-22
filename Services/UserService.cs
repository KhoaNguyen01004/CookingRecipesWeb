using CookingRecipesWeb.Models;
using Supabase;

namespace CookingRecipesWeb.Services
{
    public class UserService
    {
        private readonly Supabase.Client _client;

        public UserService(Supabase.Client client)
        {
            _client = client;
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

        public async Task CreateUserAsync(User user)
        {
            await _client.Postgrest
                .Table<User>()
                .Insert(user);
        }

        public async Task<List<Favorite>> GetUserFavoritesAsync(Guid userId)
        {
            var res = await _client.Postgrest
                .Table<Favorite>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Get();

            return res.Models;
        }

        public async Task AddFavoriteAsync(Guid userId, string recipeId)
        {
            await _client.Postgrest
                .Table<Favorite>()
                .Insert(new Favorite
                {
                    UserId = userId,
                    RecipeId = recipeId
                });
        }

        public async Task RemoveFavoriteAsync(Guid userId, string recipeId)
        {
            await _client.Postgrest
                .Table<Favorite>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("recipe_id", Supabase.Postgrest.Constants.Operator.Equals, recipeId)
                .Delete();
        }

        public async Task<bool> IsFavoriteAsync(Guid userId, string recipeId)
        {
            var res = await _client.Postgrest
                .Table<Favorite>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("recipe_id", Supabase.Postgrest.Constants.Operator.Equals, recipeId)
                .Get();

            return res.Models.Any();
        }
    }
}
