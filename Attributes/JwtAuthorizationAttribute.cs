using System;
using System.Net;
using jwt_security_token_handler_asymmetric.Extensions;
using jwt_security_token_handler_asymmetric.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace jwt_security_token_handler_asymmetric.Attributes
{
  public class JwtAuthorizationAttribute : ActionFilterAttribute, IAuthorizationFilter
  {
    private readonly ILogger<JwtAuthorizationAttribute> _logger;
    private readonly IConfiguration _configuration;
    public JwtAuthorizationAttribute(ILogger<JwtAuthorizationAttribute> logger,
        IConfiguration configuration)
    {
      _logger = logger;
      _configuration = configuration;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
      var jwt = string.Empty;
      try
      {
        if (context.IsAnonymousAllowed()) return;

        if (context.TryGetBearerToken(out jwt) == false)
            throw new ArgumentException("Beare not be null.");

        JwtValidation.ValidateToken(jwt, _configuration.GetValue<string>("PUBLICK_KEY"));
        context.HttpContext.Response.Headers["WWW-Authenticate"] = "Bearer";
      }
      catch (Exception e)
      {
          SetUnauthorizedResult(context, jwt, e.Message);
      }
    }

    private void SetUnauthorizedResult(AuthorizationFilterContext context, string jwt, string message)
    {
      _logger.LogInformation($"Token: {jwt} Unauthorized: {message}");
      var result = new ObjectResult(new { message = message })
      {
        StatusCode = (int)HttpStatusCode.Unauthorized
      };
      context.Result = result;
    }
  }
}