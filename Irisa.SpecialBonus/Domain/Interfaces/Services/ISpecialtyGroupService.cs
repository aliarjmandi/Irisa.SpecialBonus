using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Domain.Entities;

namespace Irisa.SpecialBonus.Domain.Interfaces.Services
{
    /// <summary>
    /// سرویس مدیریت گروه‌های تخصصی (SpecialtyGroups)
    /// </summary>
    public interface ISpecialtyGroupService
        : IGenericService<SpecialtyGroup, SpecialtyGroup, Guid>
    {
        /// <summary>
        /// دریافت همه گروه‌های یک معاونت بر اساس DeputyId
        /// </summary>
        Task<IEnumerable<SpecialtyGroup>> GetByDeputyIdAsync(Guid deputyId);
    }
}
