namespace CookingRecipesWeb.Models
{
    public class MealDBFilterItem
    {
        public string? IdMeal { get; set; }
        public string? StrMeal { get; set; }
        public string? StrMealThumb { get; set; }
    }

    public class MealDBFilterResponse
    {
        public List<MealDBFilterItem>? Meals { get; set; }
    }
}
