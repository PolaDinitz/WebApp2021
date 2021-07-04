using WebApp2021.BL;
using WebApp2021.DAL;
using WebApp2021.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebApp2021.Utils;

namespace WebApp2021.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserBL userBL;

        public LoginController(AppDbContext db) => this.userBL = new UserBL(db);


        // POST: Login/Index
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index([FromForm] User user)
        {
            User loggedUser = (await userBL.GetUsers(x => x.UserName == user.UserName && 
                                    x.Password == user.Password)).FirstOrDefault();

            if (loggedUser != null)
            {
                this.HttpContext.Session.SetObject("CurrentUser", loggedUser);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Incorrect Username or Password.";
            return View();
        }

        public ActionResult Logout()
        {
            this.HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}