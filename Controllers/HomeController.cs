using System;
using System.Collections.Generic;
using System.Linq;
using WebApp2021.BL;
using WebApp2021.DAL;
using WebApp2021.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApp2021.Controllers
{
    public class HomeController : Controller
    {
        private readonly RecipeBL bl;
        

        public HomeController(AppDbContext db)
        { 
            this.bl = new RecipeBL(db);
        }

        public ActionResult Index()
        {
            IEnumerable<Tuple<Recipe, float>> selectedRecipes;
            if (UserController.IsLoggedIn(HttpContext.Session))
            {
                selectedRecipes = this.bl.GetUserRecommendation(UserController.GetLoggedInUser(this.HttpContext.Session)).ToList();
            }
            else
            {
                selectedRecipes = this.bl.GetMostPopularRecipes().Distinct().ToList();
                
            }
            return View(selectedRecipes);
        }
    }
}