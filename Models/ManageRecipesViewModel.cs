using System.Collections.Generic;

namespace CookingRecipesWeb.Models
{
    public class ManageRecipesViewModel
    {
        public List<Recipe> ApiRecipes { get; set; } = new List<Recipe>();
        public List<Recipe> DbRecipes { get; set; } = new List<Recipe>();
    }
}
