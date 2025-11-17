using System;

namespace Irisa.SpecialBonus.Domain.Entities
{
    public class RewardPeriod
    {
        public Guid Id { get; set; }

        public Guid DeputyId { get; set; }          // FK به Deputy

        public int Year { get; set; }               // سال شمسی (مثلاً 1404)
        public byte Month { get; set; }             // ماه 1..12
        public string Title { get; set; } = null!;  // مثلاً "شهریور 1404"

        public decimal BudgetAmount { get; set; }   // بودجه کل پاداش ویژه معاونت در این ماه

        public bool IsLocked { get; set; }          // آیا دوره قفل شده است یا نه

        public DateTime CreatedAt { get; set; }
        public Guid? CreatedById { get; set; }      // کاربر ایجادکننده (UserId)
    }
}
