using System;

namespace Irisa.SpecialBonus.Domain.Entities
{
    public class PeriodGroupSnapshot
    {
        public Guid Id { get; set; }

        public Guid PeriodId { get; set; }          // FK به RewardPeriod
        public Guid GroupId { get; set; }           // FK به SpecialtyGroup

        public int MemberCount { get; set; }        // تعداد نفرات گروه در این دوره

        public Guid? VerifiedById { get; set; }     // UserId راهبر که تأیید کرده
        public DateTime? VerifiedAt { get; set; }   // زمان تأیید
    }
}
