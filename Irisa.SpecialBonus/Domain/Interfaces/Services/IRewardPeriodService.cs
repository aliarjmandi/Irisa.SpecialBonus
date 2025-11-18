using System;
using Irisa.SpecialBonus.Domain.Entities;

namespace Irisa.SpecialBonus.Domain.Interfaces.Services
{
    public interface IRewardPeriodService
        : IGenericService<RewardPeriod, RewardPeriod, Guid>
    {
        // در صورت نیاز متدهای اختصاصی‌تر هم می‌توانی اینجا اضافه کنی
    }
}
