using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace CookingRecipesWeb.Models
{
    [Table("ratings")]
    public class Rating : BaseModel
    {
        [PrimaryKey("id")]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("recipe_id")]
        public string RecipeId { get; set; } = string.Empty;
        [Column("rating")]
        public int RatingValue { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
