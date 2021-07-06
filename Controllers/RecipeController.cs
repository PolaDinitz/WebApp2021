using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using WebApp2021.BL;
using WebApp2021.DAL;
using WebApp2021.Models;
using WebApp2021.Utils;
using Microsoft.AspNetCore.Mvc;

namespace WebApp2021.Controllers
{
    public class RecipeController : Controller
    {
        private readonly RecipeBL recipeBL;
        private readonly RecipeUserEventBL recipeUserEventBL;
        private readonly IngredientBL ingredientBL;
        private readonly FacebookBL facebookBL;
        private readonly AppDbContext db;

        public RecipeController(AppDbContext db)
        {
            this.recipeBL = new RecipeBL(db);
            this.recipeUserEventBL = new RecipeUserEventBL(db);
            this.ingredientBL = new IngredientBL(db);
            this.facebookBL = new FacebookBL(db);
            this.db = db;
        }

        // GET: Recipe
        public ActionResult Index([FromQuery] RecipeQuery query)
        {
            Func<Recipe, bool> ingredientsPredicate = s => true;
            Func<Recipe, bool> prepTimePredicate = s => true;
            Func<Recipe, bool> kosherTypePredicate = s => true;

            if (query.ingredients != null)
            {
                ingredientsPredicate = r => r.Ingredients.Select(ing => ing.IngredientId).Any(id => query.ingredients.Contains(id));
            }

            if (query.prepTime != null)
            {
                prepTimePredicate = r => r.PrepTime <= query.prepTime;
            }

            if (query.kosherType != null)
            {
                kosherTypePredicate = r => r.KosherType == query.kosherType;
            }

            ViewData["Ingredients"] = this.ingredientBL.GetIngredients();

            return View(db.Recipes.AsEnumerable().Where(r => ingredientsPredicate(r) && prepTimePredicate(r) && kosherTypePredicate(r)));
        }

        // GET: Recipe/Create
        public ActionResult Create()
        {
            UserState state = UserController.GetPermission(this.HttpContext.Session);
            if (state == UserState.NotLoggedIn)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));

            ViewData["Ingredients"] = this.ingredientBL.GetIngredients();
            ViewData["checkedIngredients"] = new HashSet<int>();
            ViewData["SubmitLabel"] = "Upload";

            return View();
        }

        // POST: Recipe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [IfModelIsInvalid]
        public async Task<ActionResult> Create([FromForm] Recipe recipe, string[] ingredients)
        {
            UserState state = UserController.GetPermission(this.HttpContext.Session);
            if (state == UserState.NotLoggedIn)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));

            recipe.UserId = UserController.GetLoggedInUser(this.HttpContext.Session).Id;
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (await this.recipeBL.AddRecipe(recipe) > 0)
                    {
                        foreach (var ingredientId in ingredients)
                        {
                            recipe.Ingredients.Add(new RecipeIngredient
                            {
                                RecipeId = recipe.Id,
                                IngredientId = int.Parse(ingredientId),
                            });
                        }
                        if (await this.recipeBL.UpdateRecipe(recipe) > 0)
                        {
                            transaction.Commit();
                            return RedirectToAction("Index");
                        }
                        else
                            transaction.Rollback();
                    }
                    return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.InternalServerError));
                }
                catch
                {
                    transaction.Rollback();
                    return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.InternalServerError));
                }
            }
        }

        // GET: Recipe/Edit/:id
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.BadRequest));

            Recipe recipe = await this.recipeBL.GetRecipeById(id.Value);

            UserState state = UserController.GetPermission(this.HttpContext.Session, recipe.UserId);

            if (state == UserState.NotLoggedIn)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));

            if (state == UserState.RegularUserForbidden)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));

            ViewData["Ingredients"] = this.ingredientBL.GetIngredients();
            ViewData["checkedIngredients"] = recipe.Ingredients.Select(ri => ri.IngredientId).ToHashSet();
            ViewData["SubmitLabel"] = "Edit";

            return View(recipe);
        }

        // POST: Recipe/Edit/:id
        [HttpPost]
        [ValidateAntiForgeryToken]
        [IfModelIsInvalid]
        public async Task<ActionResult> Edit([FromForm] Recipe recipe, string[] ingredients)
        {
            Recipe oldRecipe = await this.recipeBL.GetRecipeById(recipe.Id);
            
            if (oldRecipe == null)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.NotFound));
 
            UserState state = UserController.GetPermission(this.HttpContext.Session, oldRecipe.UserId);

            if (state == UserState.NotLoggedIn)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));

            if (state == UserState.RegularUserForbidden)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));


            oldRecipe.Ingredients.Clear();

            foreach (var ingredientId in ingredients)
            {
                oldRecipe.Ingredients.Add(new RecipeIngredient
                {
                    RecipeId = recipe.Id,
                    IngredientId = int.Parse(ingredientId),
                });
            }

            foreach (PropertyInfo prop in recipe.GetType().GetProperties())
            {
                if (Attribute.IsDefined(prop, typeof(IsUpdatableAttribute)) && prop.SetMethod != null)
                    prop.SetValue(oldRecipe, prop.GetValue(recipe));
            }

            if (await this.recipeBL.UpdateRecipe(oldRecipe) > 0)
                return RedirectToAction("Index");
                   
            return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.InternalServerError));
        }

        // POST: Recipe/Delete/:id
        [HttpPost]
        public async Task<JsonResult> Delete([FromForm]int? id)
        {
            if (id == null)
                return Json(string.Format("/Error/?code={0}", (int)HttpStatusCode.BadRequest));

            Recipe recipe = await this.recipeBL.GetRecipeById(id.Value);

            if (recipe == null)
                return Json(string.Format("/Error/?code={0}", (int)HttpStatusCode.NotFound));

            User loggedUser = UserController.GetLoggedInUser(this.HttpContext.Session);
            if (loggedUser == null)
                return Json(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));
            
            if (!(loggedUser.Id == recipe.UserId || loggedUser.IsManager))
                return Json(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));

            if (await this.recipeBL.DeleteRecipe(id.Value) > 0)
                return Json(this.Url.Action("Index"));

            return Json(string.Format("/Error/?code={0}", (int)HttpStatusCode.InternalServerError));
        }

        // GET: Recipe/Details/:id
        public async Task<ActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.BadRequest));

            Recipe recipe = await this.recipeBL.GetRecipeById(id.Value);

            if (recipe == null)
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.NotFound));
           
            if (UserController.IsLoggedIn(this.HttpContext.Session))
            {
                int userId = UserController.GetLoggedInUser(this.HttpContext.Session).Id;
                ViewBag.isFavorite = this.recipeUserEventBL.IsFavorite(userId, id.Value);

                var preState = await this.facebookBL.FacebookPostDecider(id.Value, EventType.Viewed);

                if (await this.recipeUserEventBL.ViewRecipe(userId, id.Value) > 0)
                {
                    if (await this.facebookBL.FacebookPostDecider(id.Value, EventType.Viewed) && !preState)
                    {
                        this.facebookBL.PostRecipe(id.Value, EventType.Viewed);
                    }

                    return View(recipe);
                }

                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.InternalServerError));
            }

            return View(recipe);
        }

        [HttpPost]
        public async Task<JsonResult> PerformFavoriteRecipeState([FromForm]int recipeId)
        {
            bool operationSucceeded = false;
            bool isFavorite = false;

            if (UserController.IsLoggedIn(this.HttpContext.Session))
            {
                int userId = UserController.GetLoggedInUser(this.HttpContext.Session).Id;
                isFavorite = this.recipeUserEventBL.IsFavorite(userId, recipeId);
                var preState = await this.facebookBL.FacebookPostDecider(recipeId, EventType.Liked);

                int favRes;
                if (isFavorite)
                    favRes = await this.recipeUserEventBL.RemoveFavorite(userId, recipeId);
                else
                    favRes = await this.recipeUserEventBL.SaveFavoriteRecipe(userId, recipeId);

                operationSucceeded = favRes > 0;
                if (operationSucceeded)
                {
                    if (await this.facebookBL.FacebookPostDecider(recipeId, EventType.Liked) && !preState)
                    {
                        this.facebookBL.PostRecipe(recipeId, EventType.Liked);
                    }

                    isFavorite = !isFavorite;
                }
            }

            return Json(new { success = operationSucceeded, isRecipeFavorited = isFavorite });
        }
    }

    public class RecipeQuery
    {
        public KosherType? kosherType { get; set; }
        public int? prepTime { get; set; }

        public int[] ingredients { get; set; }
    }
}
