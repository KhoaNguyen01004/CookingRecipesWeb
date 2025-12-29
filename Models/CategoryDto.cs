namespace CookingRecipesWeb.Models
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFromApi { get; set; }
    }
}
