using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace CookingRecipesWeb.Models
{
    [Table("reviews")]
    public class Review : BaseModel
    {
        [PrimaryKey("id")]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("recipe_id")]
        public string RecipeId { get; set; } = string.Empty;
        [Column("review_text")]
        public string ReviewText { get; set; } = string.Empty;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
