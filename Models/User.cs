using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace CookingRecipesWeb.Models
{
    [Table("users")]
    public class User : BaseModel
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;
        [Column("email")]
        public string Email { get; set; } = string.Empty;
    }
}
