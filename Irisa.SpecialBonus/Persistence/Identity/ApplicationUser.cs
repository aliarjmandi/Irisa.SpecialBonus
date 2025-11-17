using System;
using Microsoft.AspNetCore.Identity;

namespace Irisa.SpecialBonus.Persistence.Identity
{
    // کلید اصلی از نوع Guid
    public class ApplicationUser : IdentityUser<Guid>
    {
        // در صورت نیاز فیلدهای سفارشی اضافه کن
        // public string? DisplayName { get; set; }
    }
}
