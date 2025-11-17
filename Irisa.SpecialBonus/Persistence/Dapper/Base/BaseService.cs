using System;
using System.Threading.Tasks;
using Dapper;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Irisa.SpecialBonus.Persistence.Dapper.Base
{
    /// <summary>
    /// پیاده‌سازی پایه برای سرویس‌های CRUD مبتنی بر Dapper.
    /// از BaseReadOnlyService ارث‌بری می‌کند و عملیات Insert/Update/Delete را اضافه می‌کند.
    /// </summary>
    public abstract class BaseService<TReadModel, TWriteModel, TKey>
        : BaseReadOnlyService<TReadModel, TKey>,
          IGenericService<TReadModel, TWriteModel, TKey>
    {
        /// <summary>
        /// کوئری Insert مثلاً:
        /// INSERT INTO Table (Col1, Col2, ...) VALUES (@Col1, @Col2, ...);
        /// در صورت نیاز می‌توان خروجی Id را نیز با SCOPE_IDENTITY() برگرداند.
        /// </summary>
        protected string SqlInsert { get; }

        /// <summary>
        /// کوئری Update مثلاً:
        /// UPDATE Table SET Col1 = @Col1, ... WHERE Id = @Id;
        /// </summary>
        protected string SqlUpdate { get; }

        /// <summary>
        /// کوئری Delete مثلاً:
        /// DELETE FROM Table WHERE Id = @Id;
        /// </summary>
        protected string SqlDelete { get; }

        protected BaseService(
            IConfiguration configuration,
            string sqlSelectAll,
            string sqlSelectById,
            string sqlOrderBy,
            string sqlInsert,
            string sqlUpdate,
            string sqlDelete,
            string connectionStringName = "DefaultConnection")
            : base(configuration, sqlSelectAll, sqlSelectById, sqlOrderBy, connectionStringName)
        {
            SqlInsert = sqlInsert ?? throw new ArgumentNullException(nameof(sqlInsert));
            SqlUpdate = sqlUpdate ?? throw new ArgumentNullException(nameof(sqlUpdate));
            SqlDelete = sqlDelete ?? throw new ArgumentNullException(nameof(sqlDelete));
        }

        /// <summary>
        /// پارامترهای مورد نیاز برای Insert را از مدل می‌سازد.
        /// </summary>
        protected abstract object GetInsertParameters(TWriteModel model);

        /// <summary>
        /// پارامترهای مورد نیاز برای Update را از id و مدل می‌سازد.
        /// </summary>
        protected abstract object GetUpdateParameters(TKey id, TWriteModel model);

        /// <summary>
        /// پارامترهای مورد نیاز برای Delete را از id می‌سازد.
        /// به صورت پیش‌فرض فقط Id را برمی‌گرداند.
        /// </summary>
        protected virtual object GetDeleteParameters(TKey id)
        {
            return new { Id = id };
        }

        /// <summary>
        /// ایجاد رکورد جدید و برگرداندن کلید.
        /// پیاده‌سازی واقعی SqlInsert می‌تواند Id را در خود SQL برگرداند (بسته به طراحی).
        /// </summary>
        public virtual async Task<TKey> CreateAsync(TWriteModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            using var connection = CreateConnection();
            var parameters = GetInsertParameters(model);

            // دو سناریو:
            // ۱) SqlInsert خودش Id را برمی‌گرداند (SELECT CAST(SCOPE_IDENTITY() as uniqueidentifier) ...)
            // ۲) Id در خود مدل ساخته می‌شود و نیازی به برگشت از DB نیست.
            //
            // برای عمومی بودن، فرض می‌کنیم SqlInsert یک مقدار Id برمی‌گرداند.
            var id = await connection.ExecuteScalarAsync<TKey>(SqlInsert, parameters);
            return id;
        }

        public virtual async Task<int> UpdateAsync(TKey id, TWriteModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            using var connection = CreateConnection();
            var parameters = GetUpdateParameters(id, model);
            var affectedRows = await connection.ExecuteAsync(SqlUpdate, parameters);
            return affectedRows;
        }

        public virtual async Task<int> DeleteAsync(TKey id)
        {
            using var connection = CreateConnection();
            var parameters = GetDeleteParameters(id);
            var affectedRows = await connection.ExecuteAsync(SqlDelete, parameters);
            return affectedRows;
        }
    }
}
