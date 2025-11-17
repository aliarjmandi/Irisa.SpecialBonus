using System.Threading.Tasks;

namespace Irisa.SpecialBonus.Domain.Interfaces.Services
{
    /// <summary>
    /// سرویس عمومی CRUD که از IReadOnlyService ارث می‌برد.
    /// TReadModel: برای خواندن
    /// TWriteModel: برای ایجاد/ویرایش (می‌تواند همان مدل یا مدل سبک‌تر باشد)
    /// </summary>
    public interface IGenericService<TReadModel, TWriteModel, TKey>
        : IReadOnlyService<TReadModel, TKey>
    {
        /// <summary>
        /// ایجاد رکورد جدید و برگرداندن کلید آن.
        /// </summary>
        Task<TKey> CreateAsync(TWriteModel model);

        /// <summary>
        /// به‌روزرسانی رکورد با شناسه مشخص. مقدار برگشتی تعداد ردیف‌های تغییر یافته است.
        /// </summary>
        Task<int> UpdateAsync(TKey id, TWriteModel model);

        /// <summary>
        /// حذف رکورد با شناسه مشخص. مقدار برگشتی تعداد ردیف‌های حذف شده است.
        /// </summary>
        Task<int> DeleteAsync(TKey id);
    }
}
