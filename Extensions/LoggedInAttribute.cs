using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Wedding_Planner.Extensions
{
    public class LoggedInAttribute : IActionFilter
    {
        //  Called after the action executes, before the action result.
        public void OnActionExecuted(ActionExecutedContext context){}
        // Called before the action executes, after model binding is complete.
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.HttpContext.Session.GetInt32("User") == null)
                context.Result = new RedirectResult("/");
        }
    }
}