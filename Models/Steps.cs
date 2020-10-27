using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Steps
    {
        public int StepsId { get; set; }
        public string StepName { get; set; }
        public int? RecipeId { get; set; }

        public virtual Recipe Recipe { get; set; }
    }
}
