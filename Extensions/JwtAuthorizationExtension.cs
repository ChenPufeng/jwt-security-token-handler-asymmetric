using jwt_security_token_handler_asymmetric.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace jwt_security_token_handler_asymmetric.Extensions
{
    public static class JwtAuthorizationExtension
    {
        public static void AddJwtCustomAthorization(this IServiceCollection service)
        {
            service.AddMvc(opt => opt.Filters.Add(typeof(JwtAuthorizationAttribute)));
        }
    }
}