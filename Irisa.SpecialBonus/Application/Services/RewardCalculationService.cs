using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;

namespace Irisa.SpecialBonus.Application.Services
{
    /// <summary>
    /// پیاده‌سازی سرویس محاسبه پاداش ویژه برای یک دوره.
    /// منطق فعلی:
    /// - ترکیب 85% امتیاز فنی و 15% ضریب مدیریتی
    /// - استفاده از وزن شاخص‌ها (Weight) و مقدار آنها (IndicatorValue)
    /// - محاسبه سهم هر گروه از بودجه دوره بر اساس FinalCoefficient و تعداد نفرات
    /// </summary>
    public class RewardCalculationService : IRewardCalculationService
    {
        private readonly IRewardPeriodService _periodService;
        private readonly IIndicatorService _indicatorService;
        private readonly IIndicatorValueService _indicatorValueService;
        private readonly IManagerialCoefficientService _managerialCoefficientService;
        private readonly IPeriodGroupSnapshotService _snapshotService;
        private readonly IGroupRewardResultService _resultService;

        public RewardCalculationService(
            IRewardPeriodService periodService,
            IIndicatorService indicatorService,
            IIndicatorValueService indicatorValueService,
            IManagerialCoefficientService managerialCoefficientService,
            IPeriodGroupSnapshotService snapshotService,
            IGroupRewardResultService resultService)
        {
            _periodService = periodService;
            _indicatorService = indicatorService;
            _indicatorValueService = indicatorValueService;
            _managerialCoefficientService = managerialCoefficientService;
            _snapshotService = snapshotService;
            _resultService = resultService;
        }

        public async Task<GroupRewardResult[]> CalculateAndSaveForPeriodAsync(Guid periodId)
        {
            // 1) خواندن دوره
            var period = await _periodService.GetByIdAsync(periodId);
            if (period == null)
                throw new InvalidOperationException("دوره موردنظر یافت نشد.");

            // 2) خواندن همه Snapshotهای این دوره (گروه‌ها و تعداد نفراتشان)
            var allSnapshots = await _snapshotService.GetAllAsync();
            var snapshots = allSnapshots.Where(s => s.PeriodId == periodId).ToList();

            if (!snapshots.Any())
                throw new InvalidOperationException("هیچ گروهی برای این دوره تعریف نشده است.");

            // 3) خواندن شاخص‌ها، مقادیر شاخص‌ها، ضرایب مدیریتی
            var allIndicators = (await _indicatorService.GetAllAsync())
                .Where(i => i.DeputyId == period.DeputyId && i.IsActive)
                .ToList();

            var allIndicatorValues = await _indicatorValueService.GetAllAsync();
            var allManagerialCoeffs = await _managerialCoefficientService.GetAllAsync();

            var results = new List<GroupRewardResult>();

            // 4) محاسبه امتیاز فنی و ضریب نهایی برای هر گروه
            foreach (var snap in snapshots)
            {
                var groupId = snap.GroupId;
                var memberCount = snap.MemberCount;

                // 4-1) امتیاز فنی: ترکیب وزن شاخص و مقدار شاخص
                decimal technicalScore = 0m;
                decimal totalWeight = allIndicators.Sum(i => i.Weight);

                if (totalWeight <= 0)
                    totalWeight = 1; // جلوگیری از تقسیم بر صفر

                foreach (var indicator in allIndicators)
                {
                    var value = allIndicatorValues.FirstOrDefault(v =>
                        v.PeriodId == periodId &&
                        v.GroupId == groupId &&
                        v.IndicatorId == indicator.Id);

                    if (value == null)
                        continue;

                    // نرمال‌سازی مقدار شاخص بین 0 و 1 بر اساس Min و Max
                    decimal normalized = 0m;
                    var range = indicator.MaxValue - indicator.MinValue;
                    if (range > 0)
                    {
                        normalized = (value.Value - indicator.MinValue) / range;
                        if (normalized < 0) normalized = 0;
                        if (normalized > 1) normalized = 1;
                    }

                    // TODO: اگر در اکسل منطق دیگری برای محاسبه ماده 47/امتیاز فنی هست،
                    // این بخش باید مطابق همان تنظیم شود.
                    technicalScore += normalized * indicator.Weight;
                }

                // تبدیل به بازه 0..1
                if (technicalScore > 0 && totalWeight > 0)
                    technicalScore /= totalWeight;

                // 4-2) ضریب مدیریتی این گروه
                var managerial = allManagerialCoeffs.FirstOrDefault(m =>
                    m.PeriodId == periodId &&
                    m.GroupId == groupId);

                decimal managerialValue = managerial?.Value ?? 1m; // اگر نبود، 1 فرض می‌کنیم

                // نرمال‌سازی مدیریتی بین 0..1 (در صورت نیاز)
                // TODO: اگر در اکسل ضریب مدیریتی مستقیماً استفاده می‌شود، این بخش را اصلاح کن.
                decimal managerialNormalized = managerialValue;

                // 4-3) ترکیب 85% فنی و 15% مدیریتی
                const decimal technicalWeight = 0.85m;
                const decimal managerialWeight = 0.15m;

                decimal finalCoefficient =
                    technicalScore * technicalWeight +
                    managerialNormalized * managerialWeight;

                if (finalCoefficient < 0) finalCoefficient = 0;

                // 4-4) تعداد نفرات با اعمال ضریب ارزیابی
                var evaluatedMembers = memberCount * finalCoefficient;

                var result = new GroupRewardResult
                {
                    Id = Guid.NewGuid(),
                    PeriodId = periodId,
                    GroupId = groupId,
                    TechnicalScore = technicalScore,
                    FinalCoefficient = finalCoefficient,
                    EvaluatedMembers = evaluatedMembers,
                    RewardAmount = 0m,      // فعلاً 0، در مرحله بعد محاسبه می‌شود
                    PerCapitaReward = null,
                    CalculatedAt = DateTime.UtcNow
                };

                results.Add(result);
            }

            // 5) محاسبه سهم هر گروه از بودجه دوره
            var totalEvaluated = results.Sum(r => r.EvaluatedMembers);
            if (totalEvaluated <= 0)
                throw new InvalidOperationException("جمع نفرات ارزیابی‌شده صفر است؛ امکان توزیع بودجه وجود ندارد.");

            foreach (var r in results)
            {
                var share = r.EvaluatedMembers / totalEvaluated;
                var rewardAmount = period.BudgetAmount * share;

                r.RewardAmount = decimal.Round(rewardAmount, 0); // گرد کردن به ریال
                if (r.EvaluatedMembers > 0)
                {
                    r.PerCapitaReward = decimal.Round(
                        r.RewardAmount / (r.EvaluatedMembers == 0 ? 1 : r.EvaluatedMembers),
                        0);
                }
            }

            // 6) حذف نتایج قبلی این دوره و ثبت نتایج جدید
            var existingResults = await _resultService.GetAllAsync();
            var existingForPeriod = existingResults.Where(r => r.PeriodId == periodId).ToList();

            foreach (var ex in existingForPeriod)
            {
                await _resultService.DeleteAsync(ex.Id);
            }

            foreach (var r in results)
            {
                await _resultService.CreateAsync(r);
            }

            return results.ToArray();
        }
    }
}
