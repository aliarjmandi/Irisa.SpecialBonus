using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Irisa.SpecialBonus.Persistence.Identity
{
    /// <summary>
    /// سرویس دسترسی به کاربران سیستم (AspNetUsers) بر اساس Identity
    /// فقط برای خواندن لیست کاربران، جستجو و در صورت نیاز حذف کاربر.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// دریافت تمام کاربران سیستم
        /// </summary>
        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        /// <summary>
        /// دریافت کاربر بر اساس Id (Guid)
        /// </summary>
        public async Task<ApplicationUser?> GetByIdAsync(Guid id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        /// <summary>
        /// جستجوی کاربر بر اساس نام (جستجو در UserName و Email به صورت تقریبی)
        /// </summary>
        public async Task<ApplicationUser?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var normalized = name.Trim().ToUpperInvariant();

            return await _userManager.Users
                .Where(u =>
                    (u.UserName != null && u.UserName.ToUpper().Contains(normalized)) ||
                    (u.Email != null && u.Email.ToUpper().Contains(normalized)))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// دریافت کاربر بر اساس UserName (دقیق)
        /// </summary>
        public async Task<ApplicationUser?> GetByUserNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return await _userManager.FindByNameAsync(name);
        }

        /// <summary>
        /// دریافت یک کاربر (اولین کاربر) بر اساس نقش
        /// در صورت نیاز می‌توانی در IUserService آن را به لیست کاربران تغییر دهی.
        /// </summary>
        public async Task<ApplicationUser?> GetByRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return null;

            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            return usersInRole.FirstOrDefault();
        }

        /// <summary>
        /// حذف کاربر بر اساس Id.
        /// اگر در کنترلر متد Delete(id) داری، این متد را صدا بزن.
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return;

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error deleting user {id}: {errors}");
            }
        }

        // اگر جایی در پروژه متد بدون پارامتر استفاده نشده، می‌توانی این متد را
        // از IUserService و این کلاس حذف کنی. فعلاً برای سازگاری خالی گذاشتم.
        public void DeleteAsync()
        {
            // not used
        }
    }
}
