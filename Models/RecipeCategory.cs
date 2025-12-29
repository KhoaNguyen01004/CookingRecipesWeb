using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CookingRecipesWeb.Models
{
    [Table("recipe_categories")]
    public class RecipeCategory : BaseModel
    {
        [Column("recipe_id")]
        public string RecipeId { get; set; } = string.Empty;

        [Column("category_id")]
        public Guid CategoryId { get; set; }
    }
}
