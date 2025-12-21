using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace CookingRecipesWeb.Models
{
    [Table("favorites")]
    public class Favorite : BaseModel
    {
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("recipe_id")]
        public string RecipeId { get; set; } = string.Empty;
    }
}
