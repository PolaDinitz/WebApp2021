using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp2021.Utils
{
    public class IfModelIsInvalidAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.ModelState.Remove("Id");

            if (!context.ModelState.IsValid)
                context.Result = new ViewResult();

            base.OnActionExecuting(context);
        }
    }
}
