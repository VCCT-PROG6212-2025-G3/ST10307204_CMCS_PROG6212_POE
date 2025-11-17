// Filters/AuthorizeRoleAttribute.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CMCS_PROG6212_POE.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public AuthorizeRoleAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");

            // Not logged in → go to login
            if (string.IsNullOrEmpty(role))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // Logged in but wrong role → forbidden
            if (!_allowedRoles.Contains(role))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
        }
    }
}