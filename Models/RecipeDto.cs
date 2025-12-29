namespace CookingRecipesWeb.Models
{
    public class RecipeDto
    {
        public string Id { get; set; } = string.Empty;
        public string StrMeal { get; set; } = string.Empty;
        public string? StrCategory { get; set; }
        public string? StrArea { get; set; }
        public string? StrInstructions { get; set; }
        public string? StrMealThumb { get; set; }
        public string? StrTags { get; set; }
        public string? StrYoutube { get; set; }
        public string? StrIngredients { get; set; }
        public string? StrMeasures { get; set; }
        public string? StrSource { get; set; }
        public string? StrImageSource { get; set; }
        public string? StrCreativeCommonsConfirmed { get; set; }
        public string? DateModified { get; set; }
    }
}
