using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jwt_security_token_handler_asymmetric.Contexts;
using jwt_security_token_handler_asymmetric.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace jwt_security_token_handler_asymmetric
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddJwtCustomAthorization();
            services.AddScoped<DbContext, ApplicationDbContext>();
            services.AddDbContext<ApplicationDbContext>(opt => 
                opt.UseInMemoryDatabase(Guid.NewGuid().ToString()), 
                ServiceLifetime.Singleton);

            services.RegisterRepositories();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new Info
                {
                    Title = "Validation JWT Handler",
                    Version = "V1",
                    Description = "Teste of validation jwt handler",
                    Contact = new Contact
                    {
                        Name = "Paulo Henrique Sousa da Silva",
                        Email = "pauloofmeta@gmail.com"
                    }
                });

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "Header",
                    Description = @"Please report the token jwt in the format 'Bearer {token}'",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] {}}
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = string.Empty;
                c.SwaggerEndpoint("/swagger/V1/swagger.json", "Validation JWT Handler");
            });
        }
        
    }
}
