using WebApp2021.BL;
using WebApp2021.DAL;
using WebApp2021.Models;
using WebApp2021.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebApp2021.Controllers
{
    public class UserController : Controller
    {
        private readonly UserBL userBL;
        private readonly ISession session;

        public UserController(AppDbContext db, IHttpContextAccessor accessor)
        {
            this.userBL = new UserBL(db);
            this.session = accessor.HttpContext.Session;
        }

#pragma warning disable CS1998
        public async Task<ActionResult> Index()
#pragma warning restore CS1998
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

            var users = await this.userBL.GetUsers();

            return View(users);
        }

        // GET: User/Details/:id
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.BadRequest));
            }

            User user = await this.userBL.GetUser(id.Value);
            if (user == null)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.NotFound));
            }

            UserState state = UserController.GetPermission(this.HttpContext.Session, user.Id);
            if (state == UserState.NotLoggedIn)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));
            }
            if (state == UserState.RegularUserForbidden)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));
            }

            return View(user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] User user)
        {
            ViewBag.ErrorMessage = "";
            if (ModelState.IsValid)
            {
                User u = (await this.userBL.GetUsers(x => x.UserName == user.UserName)).FirstOrDefault();
                if (u != null) {
                    ViewBag.ErrorMessage = "Username already exists.";
                    return View(user);
                }
                await this.userBL.Create(user);
                return RedirectToAction("Index", "Login");
            }

            return View(user);
        }

        // GET: User/Edit/:id
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.BadRequest));
            }

            User user = await this.userBL.GetUser(id.Value);
            if (user == null)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.NotFound));
            }

            UserState state = UserController.GetPermission(this.HttpContext.Session, user.Id);
            if (state == UserState.NotLoggedIn)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));
            }
            if (state == UserState.RegularUserForbidden)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));
            }

            return View(user);
        }

        // POST: User/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] User user)
        {
            ViewBag.ErrorMessage = "";
            if (ModelState.IsValid)
            {
                User duplicatedUser = (await this.userBL.GetUsers(x => x.UserName == user.UserName &&
                                            x.Id != user.Id)).FirstOrDefault();
                if (duplicatedUser != null)
                {
                    ViewBag.ErrorMessage = "Username already exists.";
                    return View(user);
                }

                await this.userBL.Edit(user);

                if (UserController.GetLoggedInUser(this.HttpContext.Session).Id == user.Id)
                    this.HttpContext.Session.SetObject("CurrentUser", user);

                return RedirectToAction("Details", "User", new { id = user.Id });
            }

            return View(user);
        }

        // GET: User/Delete/:id
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.BadRequest));
            }

            if (id == Constants.SystemUserID)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));
            }

            User user = await this.userBL.GetUser(id.Value);
            if (user == null)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.NotFound));
            }

            UserState state = UserController.GetPermission(this.HttpContext.Session, user.Id);
            if (state == UserState.NotLoggedIn)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Unauthorized));
            }
            if (state == UserState.RegularUserForbidden)
            {
                return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.Forbidden));
            }

            return View(user);
        }

        // POST: User/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (await this.userBL.Delete(id) > 0)
            {
                if (UserController.GetLoggedInUser(this.HttpContext.Session).Id == id)
                    return RedirectToAction("Logout", "Login");

                return RedirectToAction("Index");
            }

            return Redirect(string.Format("/Error/?code={0}", (int)HttpStatusCode.InternalServerError));
        }

        #region User Session Functions 
        public static User GetLoggedInUser(ISession session)
        {
            User user = session.GetObject<User>("CurrentUser");
            return (user != null) ? (user as User) : null;
        }

        public static UserState GetPermission(ISession session)
        {
            User user = UserController.GetLoggedInUser(session);

            if (user == null)
                return UserState.NotLoggedIn;

            return (user.IsManager) ? UserState.Manager : UserState.RegularUserForbidden;
        }

        public static UserState GetPermission(ISession session, int id)
        {
            User user = UserController.GetLoggedInUser(session);

            if (user == null)
                return UserState.NotLoggedIn;

            if (!user.IsManager)
                return (user.Id == id) ? UserState.RegularUser : UserState.RegularUserForbidden;

            return UserState.Manager;
        }

        public static bool IsLoggedIn(ISession session)
        {
            return UserController.GetLoggedInUser(session) != null;
        }

        public static bool IsManager(ISession session)
        {
            return UserController.GetPermission(session) == UserState.Manager;
        }

        public static bool IsCurrentUserOrManager(ISession session, int id)
        {
            UserState state = UserController.GetPermission(session, id);

            return (state == UserState.RegularUser || state == UserState.Manager);
        }
        #endregion
    }
}
