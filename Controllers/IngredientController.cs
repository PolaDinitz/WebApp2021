using System.Net;
using System.Threading.Tasks;
using WebApp2021.BL;
using WebApp2021.DAL;
using WebApp2021.Models;
using WebApp2021.Utils;
using Microsoft.AspNetCore.Mvc;

namespace WebApp2021.Controllers
{
    public class IngredientController : Controller
    {
        private readonly IngredientBL bl;

        public IngredientController(AppDbContext db) => this.bl = new IngredientBL(db);

        public ActionResult Index()
        {
            return View(this.bl.GetIngredients());
        }

        // GET: Ingredient/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.BadRequest));
            }

            Ingredient ingredient = await this.bl.GetIngredientById(id);

            if (ingredient == null)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.NotFound));
            }

            return View(ingredient);
        }

        // GET: Ingredient/Create
        public ActionResult Create()
        {
            UserState state = UserController.GetPermission(this.HttpContext.Session);
            if (state == UserState.NotLoggedIn)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));
            }

            return View();
        }

        // POST: Ingredient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [IfModelIsInvalid]
        public async Task<ActionResult> Create([FromForm] Ingredient ingredient)
        {
            if (await this.bl.SaveIngredient(ingredient) > 0)
                return RedirectToAction("Index");
            
            return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.InternalServerError));
        }

        // GET: Ingredient/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.BadRequest));
            }

            if (UserController.GetPermission(this.HttpContext.Session) == UserState.NotLoggedIn)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));
            } 
            else if (UserController.GetPermission(this.HttpContext.Session) != UserState.Manager)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));
            }

            Ingredient ingredient = await this.bl.GetIngredientById(id);

            if (ingredient == null)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.NotFound));
            }

            return View(ingredient);
        }

        // POST: Ingredient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [IfModelIsInvalid]
        public async Task<ActionResult> Edit([FromForm] Ingredient ingredient)
        {
            if (await this.bl.UpdateIngredient(ingredient) > 0)
                return RedirectToAction("Index");

            return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.InternalServerError));
        }

        // GET: Ingredient/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.BadRequest));
            }

            if (UserController.GetPermission(this.HttpContext.Session) == UserState.NotLoggedIn)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));
            }

            Ingredient ingredient = await this.bl.GetIngredientById(id);

            if (ingredient == null)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.NotFound));
            }

            return View(ingredient);
        }

        // POST: Ingredient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await this.bl.DeleteIngredient(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetAllIngredientsNames()
        {
            return Json(new { ingredients = this.bl.GetDistinctIngredientsNames() });
        }
    }
}