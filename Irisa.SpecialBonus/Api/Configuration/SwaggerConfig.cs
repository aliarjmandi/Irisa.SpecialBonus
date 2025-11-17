using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace Irisa.SpecialBonus.Api.Configuration
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Irisa Special Bonus API",
                    Version = "v1"
                });

                // تعریف امنیت Bearer برای JWT
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "توکن JWT را با فرمت 'Bearer {token}' وارد کنید",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                };

                c.AddSecurityDefinition("Bearer", securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                };

                c.AddSecurityRequirement(securityRequirement);
            });

            return services;
        }
    }
}
