using System.Text;
using Irisa.SpecialBonus.Api.Configuration;
using Irisa.SpecialBonus.Application.Services;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Irisa.SpecialBonus.Api.Configuration
{
    public static class AuthenticationConfig
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind Jwt settings
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            var jwtSettings = new JwtSettings();
            configuration.GetSection("Jwt").Bind(jwtSettings);

            var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),

                        ValidateLifetime = true,
                        ClockSkew = System.TimeSpan.FromMinutes(1)
                    };
                });

            // ثبت سرویس تولید توکن
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            return services;
        }
    }
}
