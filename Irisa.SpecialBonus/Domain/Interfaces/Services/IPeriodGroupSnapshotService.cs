using System;
using Irisa.SpecialBonus.Domain.Entities;

namespace Irisa.SpecialBonus.Domain.Interfaces.Services
{
    /// <summary>
    /// سرویس مربوط به Snapshot تعداد نفرات گروه‌ها در هر دوره.
    /// </summary>
    public interface IPeriodGroupSnapshotService
        : IGenericService<PeriodGroupSnapshot, PeriodGroupSnapshot, Guid>
    {
    }
}
