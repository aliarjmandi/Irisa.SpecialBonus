using System;

namespace Irisa.SpecialBonus.Domain.Entities
{
    public class Indicator
    {
        public Guid Id { get; set; }

        public Guid DeputyId { get; set; }          // شاخص مربوط به کدام معاونت است

        public string Code { get; set; } = null!;   // کد کوتاه (APEX, DEV_REQ, ...)
        public string Name { get; set; } = null!;   // عنوان شاخص
        public string? Description { get; set; }

        public decimal Weight { get; set; }         // ضریب اثر (مثلاً 0.35)

        public decimal MinValue { get; set; }       // حداقل مجاز برای مقدار شاخص
        public decimal MaxValue { get; set; }       // حداکثر مجاز

        public Guid DataEntryUserId { get; set; }   // کاربر مسئول ثبت مقدار شاخص

        public bool IsActive { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid? CreatedById { get; set; }
    }
}
