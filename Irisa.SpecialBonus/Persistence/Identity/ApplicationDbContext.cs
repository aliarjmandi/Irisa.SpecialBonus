using Irisa.SpecialBonus.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace Irisa.SpecialBonus.Persistence.Identity
{
    /// <summary>
    /// کانتکست EF فقط برای Identity (کاربران و نقش‌ها)
    /// </summary>
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // اگر خواستی جدول‌ها در اسکیمای خاصی باشند، اینجا تنظیم کن
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // مثال: تغییر نام جدول‌ها (اختیاری)
            // builder.Entity<ApplicationUser>().ToTable("Users");
            // builder.Entity<ApplicationRole>().ToTable("Roles");
        }
    }
}
