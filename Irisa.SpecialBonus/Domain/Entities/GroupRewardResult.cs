using System;

namespace Irisa.SpecialBonus.Domain.Entities
{
    public class GroupRewardResult
    {
        public Guid Id { get; set; }

        public Guid PeriodId { get; set; }          // دوره
        public Guid GroupId { get; set; }           // گروه تخصصی

        public decimal TechnicalScore { get; set; }   // ارزیابی فنی ترکیبی (ماده 47)
        public decimal FinalCoefficient { get; set; } // ضریب نهایی (فنی + مدیریتی)
        public decimal EvaluatedMembers { get; set; } // تعداد نفرات با اعمال ضریب

        public decimal RewardAmount { get; set; }     // مبلغ پاداش ویژه گروه
        public decimal? PerCapitaReward { get; set; } // سرانه (اختیاری)

        public DateTime CalculatedAt { get; set; }    // زمان محاسبه
    }
}
