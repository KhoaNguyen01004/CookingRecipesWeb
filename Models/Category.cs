using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CookingRecipesWeb.Models
{
    [Table("categories")]
    public class Category : BaseModel
    {
        [PrimaryKey("id", false)]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("is_from_api")]
        public bool IsFromApi { get; set; }
    }
}
