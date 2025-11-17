using System;

namespace Irisa.SpecialBonus.Domain.Entities
{
    public class ManagerialCoefficient
    {
        public Guid Id { get; set; }

        public Guid PeriodId { get; set; }          // دوره
        public Guid GroupId { get; set; }           // گروه تخصصی

        public decimal Value { get; set; }          // ضریب مدیریتی (مثلاً 1.02)

        public Guid EnteredById { get; set; }       // UserId معاون
        public DateTime EnteredAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
