using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace CookingRecipesWeb.Models
{
    [Table("review_replies")]
    public class ReviewReply : BaseModel
    {
        [PrimaryKey("id")]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("parent_review_id")]
        public Guid ParentReviewId { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("reply_text")]
        public string ReplyText { get; set; } = string.Empty;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
