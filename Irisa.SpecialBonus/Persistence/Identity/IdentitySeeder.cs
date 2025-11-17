using System;
using System.Linq;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Irisa.SpecialBonus.Persistence.Identity
{
    public static class IdentitySeeder
    {
        /// <summary>
        /// ایجاد نقش‌های پایه و چند کاربر نمونه برای شروع کار سیستم.
        /// این متد را یک‌بار در شروع برنامه صدا بزن.
        /// </summary>
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1) ایجاد نقش‌ها
            await EnsureRoleAsync(roleManager, RoleNames.SystemAdmin);
            await EnsureRoleAsync(roleManager, RoleNames.Deputy);
            await EnsureRoleAsync(roleManager, RoleNames.IndicatorDataEntry);
            await EnsureRoleAsync(roleManager, RoleNames.GroupManager);

            // 2) ایجاد کاربران نمونه (موقت / برای تست)
            // TODO: بعداً ایمیل‌ها و پسوردها را مطابق سیاست سازمان تغییر بده.

            // کاربر مدیریت سیستم
            var adminUser = await EnsureUserAsync(
                userManager,
                userName: "admin",
                email: "admin@irisa.local",
                password: "Admin@12345");

            await EnsureUserInRoleAsync(userManager, adminUser, RoleNames.SystemAdmin);

            // کاربر معاونت سیستم‌های اطلاعاتی
            var deputyUser = await EnsureUserAsync(
                userManager,
                userName: "deputy.is",
                email: "deputy.is@irisa.local",
                password: "Deputy@12345");

            await EnsureUserInRoleAsync(userManager, deputyUser, RoleNames.Deputy);

            // یک کاربر واردکننده ضرایب فنی (مثلاً برای شاخص APEX)
            var indicatorUser = await EnsureUserAsync(
                userManager,
                userName: "indicator.apex",
                email: "indicator.apex@irisa.local",
                password: "Indicator@12345");

            await EnsureUserInRoleAsync(userManager, indicatorUser, RoleNames.IndicatorDataEntry);

            // یک مدیر گروه تخصصی (مثلاً گروه سامانه‌های هوشمند)
            var groupManagerUser = await EnsureUserAsync(
                userManager,
                userName: "gm.intelligent",
                email: "gm.intelligent@irisa.local",
                password: "GroupMgr@12345");

            await EnsureUserInRoleAsync(userManager, groupManagerUser, RoleNames.GroupManager);
        }

        private static async Task EnsureRoleAsync(
            RoleManager<ApplicationRole> roleManager,
            string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                };

                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Error creating role '{roleName}': {errors}");
                }
            }
        }

        private static async Task<ApplicationUser> EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            string userName,
            string email,
            string password)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                return user;
            }

            user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error creating user '{userName}': {errors}");
            }

            return user;
        }

        private static async Task EnsureUserInRoleAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string roleName)
        {
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                var result = await userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Error adding user '{user.UserName}' to role '{roleName}': {errors}");
                }
            }
        }
    }
}
