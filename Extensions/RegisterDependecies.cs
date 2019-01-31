using System;
using jwt_security_token_handler_asymmetric.Abstraction;
using jwt_security_token_handler_asymmetric.Models;
using jwt_security_token_handler_asymmetric.Repositores;
using Microsoft.Extensions.DependencyInjection;

namespace jwt_security_token_handler_asymmetric.Extensions
{
    public static class RegisterDependecies
    {
        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Client, Guid>, ClientRepository>();
            //services.AddScoped<IRepository<Sale, Guid>, RepositoryBase<Sale, Guid>>();
        }
    }
}