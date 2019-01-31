using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace jwt_security_token_handler_asymmetric.Extensions
{
    public static class AuthorizationFilterContextExtension
    {
        public static bool IsAnonymousAllowed(this ActionContext context)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var allowClassAnonymous = descriptor?.ControllerTypeInfo.GetCustomAttribute<AllowAnonymousAttribute>();
            var allowMethodAnonymous = descriptor?.MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>();
            return (allowMethodAnonymous != null) || (allowClassAnonymous != null);
        }

        public static bool TryGetBearerToken(this AuthorizationFilterContext context, out string jwt)
        {
            const string authorization = "Authorization";
            jwt = string.Empty;
            if (context.HttpContext.Request.Headers.TryGetValue(authorization, out var token) == false)
            {
                context.Result = new UnauthorizedResult();
                return false;
            }

            jwt = token.SingleOrDefault()?.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Skip(1)
                .SingleOrDefault();

            if (string.IsNullOrWhiteSpace(jwt))
            {
                context.Result = new UnauthorizedResult();
                return false;
            }

            return true;
        }
    }
}