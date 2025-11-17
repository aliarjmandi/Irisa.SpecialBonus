using System.Collections.Generic;
using System.Threading.Tasks;

namespace Irisa.SpecialBonus.Domain.Interfaces.Services
{
    /// <summary>
    /// سرویس عمومی فقط-خواندنی برای موجودیت‌ها
    /// </summary>
    /// <typeparam name="TModel">مدل خروجی (معمولاً Entity)</typeparam>
    /// <typeparam name="TKey">نوع کلید اصلی (مثلاً Guid)</typeparam>
    public interface IReadOnlyService<TModel, TKey>
    {
        Task<IEnumerable<TModel>> GetAllAsync();

        Task<TModel?> GetByIdAsync(TKey id);

        /// <summary>
        /// برگرداندن صفحه‌ای از داده‌ها به همراه تعداد کل رکوردها
        /// </summary>
        /// <param name="pageNumber">شماره صفحه (۱ به بالا)</param>
        /// <param name="pageSize">تعداد رکورد در صفحه</param>
        Task<(IEnumerable<TModel> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    }
}
