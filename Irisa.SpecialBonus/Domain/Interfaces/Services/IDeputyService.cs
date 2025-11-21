using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Domain.Entities;

namespace Irisa.SpecialBonus.Domain.Interfaces.Services
{
    /// <summary>
    /// سرویس مدیریت معاونت‌ها (Deputies)
    /// </summary>
    public interface IDeputyService
        : IGenericService<Deputy, Deputy, Guid>
    {
        /// <summary>
        /// دریافت معاونت بر اساس کُد (مثلاً ISD)
        /// </summary>
        /// 
        Task<Deputy?> GetByCodeAsync(string code);

        /// <summary>
        /// دریافت همه معاونت‌های فعال
        /// </summary>
        Task<IEnumerable<Deputy>> GetActiveAsync();
    }
}
