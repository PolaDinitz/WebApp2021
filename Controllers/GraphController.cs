using System.Collections.Generic;
using System.Net;
using WebApp2021.BL;
using WebApp2021.DAL;
using WebApp2021.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApp2021.Controllers
{
    public class GraphController : Controller
    {
        private readonly RecipeBL recipeBL;

        public GraphController(AppDbContext db)
        {
            this.recipeBL = new RecipeBL(db);
        }

        public ActionResult Index()
        {
            UserState state = UserController.GetPermission(this.HttpContext.Session);
            if (state == UserState.NotLoggedIn)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));
            }
            if (state != UserState.Manager)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));
            }

            ViewData["Graph1"] = this.recipeBL.GetRecipesTotalViews();

            Dictionary<string, int> recipesKosher = new Dictionary<string, int>();
            foreach (var item in this.recipeBL.GetRecipesKosher())
            {
                recipesKosher.Add(item.kosherType, item.count);
            }
            ViewData["Graph2"] = recipesKosher;

            return View();
        }
    }
}
