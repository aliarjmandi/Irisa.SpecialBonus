using System;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Domain.Entities;

namespace Irisa.SpecialBonus.Domain.Interfaces.Services
{
    /// <summary>
    /// سرویس محاسبه پاداش ویژه برای دوره‌ها.
    /// مسئول محاسبه TechnicalScore, FinalCoefficient, EvaluatedMembers و RewardAmount
    /// و ثبت نتیجه در جدول GroupRewardResults است.
    /// </summary>
    public interface IRewardCalculationService
    {
        /// <summary>
        /// محاسبه پاداش ویژه برای همه گروه‌های یک دوره
        /// و ذخیره نتیجه در GroupRewardResults.
        /// خروجی: آرایه‌ای از نتایج محاسبه شده.
        /// </summary>
        Task<GroupRewardResult[]> CalculateAndSaveForPeriodAsync(Guid periodId);
    }
}
