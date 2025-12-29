using System.Text.Json.Serialization;

namespace CookingRecipesWeb.Models
{
    public class MealDBCategoryResponse
    {
        [JsonPropertyName("categories")]
        public List<MealDBCategory>? Categories { get; set; }
    }
}
