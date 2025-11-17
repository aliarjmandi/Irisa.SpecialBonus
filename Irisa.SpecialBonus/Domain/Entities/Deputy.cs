using System;

namespace Irisa.SpecialBonus.Domain.Entities
{
    public class Deputy
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = null!;   // کد معاونت (مثلاً ISD)
        public string Name { get; set; } = null!;   // نام معاونت

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
