using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Recipe
    {
        public Recipe()
        {
            Ingredient = new HashSet<Ingredient>();
            Steps = new HashSet<Steps>();
        }

        public int RecipeId { get; set; }
        public string RecipeTitle { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public int? LevelId { get; set; }
        public DateTime? DateAdded { get; set; }

        public virtual Level Level { get; set; }
        public virtual ICollection<Ingredient> Ingredient { get; set; }
        public virtual ICollection<Steps> Steps { get; set; }
    }
}
