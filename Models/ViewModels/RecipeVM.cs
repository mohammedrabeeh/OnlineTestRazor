using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models.ViewModels
{
    public class RecipeVM
    {
        public Recipe Recipe { get; set; }
        public IEnumerable<Steps> Steps { get; set; }

        public IEnumerable<Ingredient> Ingredient { get; set; }
    }

    public class NewRecipeVM
    {
        public int RecipeId { get; set; }
        public string RecipeTitle { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string LevelName { get; set; }
        public string Step { get; set; }
        public string Ingredient { get; set; }

    }

    public class NewRecipeVMSam
    {
        public int RecipeId { get; set; }
        public string RecipeTitle { get; set; }
        public DateTime? DateAdded { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public IEnumerable<Steps> Steps { get; set; }

        public IEnumerable<Ingredient> Ingredient { get; set; }

    }
}
