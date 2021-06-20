using WebApp2021.BL;
using WebApp2021.DAL;
using WebApp2021.Models;
using WebApp2021.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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

        public static bool IsManager(ISession session)
        {
            return UserController.GetPermission(session) == UserState.Manager;
        }

        #endregion
    }
}
