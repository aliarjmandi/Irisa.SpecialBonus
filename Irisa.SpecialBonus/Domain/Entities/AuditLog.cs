using System;

namespace Irisa.SpecialBonus.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }         // کاربری که عمل را انجام داده

        public string ActionType { get; set; } = null!;   // نوع عمل (مثلاً "UpdateIndicatorValue")
        public string? EntityName { get; set; }           // نام موجودیت (مثلاً "IndicatorValue")
        public Guid? EntityId { get; set; }               // شناسه‌ی رکورد

        public string? Details { get; set; }              // JSON / توضیحات بیشتر

        public DateTime CreatedAt { get; set; }
    }
}
