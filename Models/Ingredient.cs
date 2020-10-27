using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Ingredient
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; }
        public int? RecipeId { get; set; }

        public virtual Recipe Recipe { get; set; }
    }
}
