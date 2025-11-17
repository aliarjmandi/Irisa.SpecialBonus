using System;

namespace Irisa.SpecialBonus.Domain.Entities
{
    public class SpecialtyGroup
    {
        public Guid Id { get; set; }

        public Guid DeputyId { get; set; }          // زیر کدام معاونت

        public string? ErpGroupCode { get; set; }   // کد گروه در ERP (در صورت وجود)

        public string Name { get; set; } = null!;   // نام گروه تخصصی

        public Guid? ManagerUserId { get; set; }    // UserId مدیر گروه (از Identity)

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
