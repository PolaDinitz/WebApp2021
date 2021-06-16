using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebApp2021.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            bool Parsed = int.TryParse(HttpContext.Request.Query["code"], out int code);

            if (!Parsed)
                code = (int)HttpStatusCode.InternalServerError;

            Response.StatusCode = code;
            ViewBag.ErrorMessage = (HttpStatusCode)code;

            return View();
        }
    }
}