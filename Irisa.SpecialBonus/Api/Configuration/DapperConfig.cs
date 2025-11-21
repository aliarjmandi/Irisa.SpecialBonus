using Irisa.SpecialBonus.Application.Services;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Irisa.SpecialBonus.Persistence.Dapper.Services;
using Irisa.SpecialBonus.Persistence.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Irisa.SpecialBonus.Api.Configuration
{
    public static class DapperConfig
    {
        public static IServiceCollection AddDapperServices(this IServiceCollection services)
        {
            // سرویس دوره‌ها (RewardPeriods)
            services.AddScoped<IRewardPeriodService, RewardPeriodService>();
            services.AddScoped<IIndicatorService, IndicatorService>();
            services.AddScoped<IIndicatorValueService, IndicatorValueService>();
            services.AddScoped<IManagerialCoefficientService, ManagerialCoefficientService>();
            services.AddScoped<IGroupRewardResultService, GroupRewardResultService>();
            services.AddScoped<IPeriodGroupSnapshotService, PeriodGroupSnapshotService>();
            services.AddScoped<IRewardCalculationService, RewardCalculationService>();
            services.AddScoped<IDeputyService, DeputyService>();
            services.AddScoped<ISpecialtyGroupService, SpecialtyGroupService>();
            services.AddScoped<IUserService, UserService>();


            return services;
        }
    }
}
