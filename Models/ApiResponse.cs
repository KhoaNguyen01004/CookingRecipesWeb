using System.Collections.Generic;

namespace CookingRecipesWeb.Models
{
    public class ApiResponse<T>
    {
        public List<T>? Meals { get; set; }
        public List<T>? Categories { get; set; }
    }
}
