using System;

namespace Irisa.SpecialBonus.Domain.Entities
{
    public class IndicatorValue
    {
        public Guid Id { get; set; }

        public Guid PeriodId { get; set; }          // دوره
        public Guid GroupId { get; set; }           // گروه تخصصی
        public Guid IndicatorId { get; set; }       // شاخص فنی

        public decimal Value { get; set; }          // مقدار شاخص (مثلاً 0.89)

        public Guid EnteredById { get; set; }       // UserId واردکننده
        public DateTime EnteredAt { get; set; }     // زمان ثبت

        public DateTime? ModifiedAt { get; set; }   // آخرین ویرایش (در صورت وجود)
    }
}
