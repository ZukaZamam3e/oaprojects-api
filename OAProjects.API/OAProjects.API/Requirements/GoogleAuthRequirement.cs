using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Text;

namespace OAProjects.API.Requirements;

public partial class AuthorizationRequirements
{
    public const string GoogleAuthRequirement = "GoogleAuthRequirement";
}

public class GoogleAuthRequirement : IAuthorizationRequirement
{
    public GoogleAuthRequirement()
    {

    }
}

class GoogleAuthRequirementHandler : AuthorizationHandler<GoogleAuthRequirement>
{
    public GoogleAuthRequirementHandler()
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GoogleAuthRequirement requirement)
    {
        DefaultHttpContext httpContext = (DefaultHttpContext)context.Resource;

        // Check that token is currently valid.
        // Store user token in memory and in db
        // if user token is not in memory, pull it from db
        // if not in db, add it to db and memory
        // check google for valid token
        // if token is not valid, set back invalid token

        string token = httpContext.Request.Headers.FirstOrDefault(m => m.Key == "Authorization").Value.ToString() ?? "";

        if(!string.IsNullOrEmpty(token) && false)
        {
            context.Succeed(requirement);
        }
        else
        {
            //var mvcContext = context.Resource as AuthorizationFilterContext;
            //mvcContext.Result = new RedirectToActionResult("Unauth", "Auth", null);
            //httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            httpContext.Response.Redirect("~/Auth/Unauth"); 
            //context.Result.
            //filterContext.HttpContext.Response.StatusCode = 403;
            //filterContext.Result = new RedirectResult("/Unauthorized/");
        }


        return Task.CompletedTask;
    }
}
