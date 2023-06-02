using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CI_Platform.Controllers
{
    public class BaseController : Controller
    {
        //private readonly string _token;
        //public BaseController(string token)
        //{
        //    _token = token;
        //}
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var Role = context.HttpContext.User.Claims;
            var role = Role.FirstOrDefault();
            if (role.Value != "Admin")
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "User",
                    action = "Invalid"
                }));
            }
            base.OnActionExecuting(context);
        }

        //public void OnAuthorization(AuthorizationFilterContext context)
        //{
        //    context.HttpContext.Request.Headers.Add("Authorization", $"Bearer {_token}");
        //}

    }
}
