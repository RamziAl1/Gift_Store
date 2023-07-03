using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Gift_Store
{
    public class LoginCheckFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (actionDescriptor != null)
            {
                var controllerName = actionDescriptor.ControllerName;
                var actionName = actionDescriptor.ActionName;

                // Exclude specific actions or controllers from the filter
                if (controllerName == "Auth" && actionName == "Login" 
                    || controllerName == "Auth" && actionName == "Register" 
                    || controllerName == "Auth" && actionName == "LoginAsGuest")
                {
                    return; // Skip authentication check for Login action in HomeController
                }
            }
            // Check if the user is logged in by validating the session
            if (context.HttpContext.Session.GetInt32("RoleId") == null)
            {
                // User is not logged in, redirect to the login page
                context.Result = new RedirectResult("/Auth/Login");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action needed after the action is executed
        }
    }
}
