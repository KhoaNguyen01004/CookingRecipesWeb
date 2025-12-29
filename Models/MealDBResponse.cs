namespace CookingRecipesWeb.Models
{
    public class MealDBResponse<T>
    {
        public List<T>? Meals { get; set; }
    }
}
