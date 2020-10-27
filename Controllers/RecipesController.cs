using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class RecipesController : Controller
    {
        private readonly OnlineHubContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RecipesController(OnlineHubContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Recipes
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var onlineHubContext = _context.Recipe.Include(r => r.Level);
            List<RecipeVM> recipeVMs = new List<RecipeVM>();
            foreach(Recipe recipes in onlineHubContext)
            {
                RecipeVM recipeVM = new RecipeVM();
                recipeVM.Recipe = recipes;

                recipeVM.Steps = _context.Steps.Where(s => s.RecipeId == recipes.RecipeId);
                recipeVM.Ingredient = _context.Ingredient.Where(s => s.RecipeId == recipes.RecipeId);

                recipeVMs.Add(recipeVM);
            }
            return View(recipeVMs);
        }



        public async Task<IActionResult> Join()
        {
            var Recipe = (from r in _context.Recipe
                          join l in _context.Level
                          on r.LevelId equals l.LevelId
                          orderby r.RecipeId ascending
                          select new NewRecipeVM
                          {
                              RecipeId = r.RecipeId,
                              RecipeTitle = r.RecipeTitle,
                              LevelName = l.LevelName,
                              Step = _context.Steps.Where(s => s.RecipeId == r.RecipeId).OrderByDescending(v => v.StepsId).Select(
                                    st => st.StepName
                                ).SingleOrDefault()
                          }
                          );


            return View(await Recipe.ToListAsync());
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipe
                .Include(r => r.Level)
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            RecipeVM recipeVM = new RecipeVM();
            recipeVM.Recipe = recipe;

            var steps = await _context.Steps
                .Where(m => m.RecipeId == id)
                .ToListAsync();

            recipeVM.Steps = steps;

            var ingredients = await _context.Ingredient
                .Where(m => m.RecipeId == id)
                .ToListAsync();

            recipeVM.Ingredient = ingredients;

            return View(recipeVM);
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Print(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipe
                .Include(r => r.Level)
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: Recipes/Create
        public IActionResult Create()
        {
            ViewData["LevelId"] = new SelectList(_context.Level, "LevelId", "LevelName");
            return View();
        }

        // GET: Recipes/Test
        public IActionResult Test(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "RecipeTitle_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "DateAdded" ? "DateAdded_desc" : "DateAdded";
            ViewData["IDSortParm"] = sortOrder == "RecipeID" ? "RecipeID_desc" : "RecipeID";
            ViewData["CurrentSort"] = sortOrder;
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            IEnumerable<NewRecipeVMSam> _recipes = _context.Recipe.Select(recipe => new NewRecipeVMSam()
            {
                RecipeId = recipe.RecipeId,
                RecipeTitle = recipe.RecipeTitle,
                DateAdded = recipe.DateAdded,
                Steps = _context.Steps.Where(r => r.RecipeId == recipe.RecipeId).Select(
                    st => new Steps()
                    {
                        StepName = st.StepName,
                        StepsId = st.StepsId
                    }
                    ),
                Ingredient = _context.Ingredient.Where(r => r.RecipeId == recipe.RecipeId).Select(
                    st => new Ingredient()
                    {
                        IngredientName = st.IngredientName,
                        IngredientId = st.IngredientId
                    }
                    )
            });

            if (!String.IsNullOrEmpty(searchString))
            {
                _recipes = _recipes.Where(s => s.RecipeTitle.ToUpper().Contains(searchString.ToUpper()));
            }

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            switch (sortOrder)
            {
                case "RecipeTitle_desc":
                    _recipes = _recipes.OrderByDescending(s => s.RecipeTitle);
                    break;
                case "DateAdded":
                    _recipes = _recipes.OrderBy(s => s.DateAdded);
                    break;
                case "RecipeID_desc":
                    _recipes = _recipes.OrderByDescending(s => s.RecipeId);
                    break;
                case "RecipeID":
                    _recipes = _recipes.OrderBy(s => s.RecipeId);
                    break;
                case "DateAdded_desc":
                    _recipes = _recipes.OrderByDescending(s => s.DateAdded);
                    break;
                default:
                    _recipes = _recipes.OrderBy(s => s.RecipeTitle);
                    break;
            }

            int pageSize = 3;
            return View(PaginatedList<NewRecipeVMSam>.CreateAsync(_recipes, pageNumber ?? 1, pageSize));
        }
        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipeId,RecipeTitle,Image1,Image2,Image3,LevelId,DateAdded")] Recipe recipe, string[] StepsTextBox, string[] IngredientsTextbox, IFormFile image1, IFormFile image2, IFormFile image3)
        {
            if (ModelState.IsValid)
            {
                Recipe recipe1 = new Recipe
                {
                    RecipeTitle = recipe.RecipeTitle,
                    DateAdded = DateTime.Now,
                    LevelId = recipe.LevelId
                };


                if (image1 != null)
                {
                    var FileName = Guid.NewGuid() + Path.GetExtension(image1.FileName);
                    var SavePath = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot", "uploads", FileName);
                    using (var stream = new FileStream(SavePath, FileMode.Create))
                    {
                        image1.CopyTo(stream);
                    }
                    recipe1.Image1 = FileName;

                }

                if (image2 != null)
                {
                    var FileName = Guid.NewGuid() + Path.GetExtension(image2.FileName);
                    var SavePath = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot", "uploads", FileName);
                    using (var stream = new FileStream(SavePath, FileMode.Create))
                    {
                        image2.CopyTo(stream);
                    }
                    recipe1.Image2 = FileName;
                }

                if (image3 != null)
                {
                    var FileName = Guid.NewGuid() + Path.GetExtension(image3.FileName);
                    var SavePath = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot", "uploads", FileName);
                    using (var stream = new FileStream(SavePath, FileMode.Create))
                    {
                        image3.CopyTo(stream);
                    }
                    recipe1.Image3 = FileName;
                }

                _context.Add(recipe1);
                _context.SaveChanges();

                foreach (string Step in StepsTextBox)
                {
                    if(Step != null)
                    {
                        Steps stp = new Steps
                        {
                            StepName = Step,
                            RecipeId = recipe1.RecipeId
                        };
                        _context.Add(stp);
                    }
                }

                foreach (string Ingredients in IngredientsTextbox)
                {
                    if (Ingredients != null)
                    {
                        Ingredient stp = new Ingredient
                        {
                            IngredientName = Ingredients,
                            RecipeId = recipe1.RecipeId
                        };
                        _context.Add(stp);
                    }
                }
                 await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["LevelId"] = new SelectList(_context.Level, "LevelId", "LevelName", recipe.LevelId);
            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipe.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            ViewData["LevelId"] = new SelectList(_context.Level, "LevelId", "LevelName", recipe.LevelId);
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecipeId,RecipeTitle,Image1,Image2,Image3,LevelId,DateAdded")] Recipe recipe)
        {
            
            if (id != recipe.RecipeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.RecipeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LevelId"] = new SelectList(_context.Level, "LevelId", "LevelName", recipe.LevelId);
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipe
                .Include(r => r.Level)
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipe.FindAsync(id);
            _context.Recipe.Remove(recipe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipe.Any(e => e.RecipeId == id);
        }


        [HttpPost]
        public async Task<IActionResult> PrintDataNewAsync([FromRoute] string id)
        {
            var render = new IronPdf.HtmlToPdf();
            var doc = render.RenderUrlAsPdf("https://localhost:44354/Recipes/Print/" + id);

            var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot","uploads", DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf");

            doc.SaveAs(path);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/pdf", Path.GetFileName(path));

        }
    }
}
