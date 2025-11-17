using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Irisa.SpecialBonus.Persistence.Dapper.Base
{
    /// <summary>
    /// پیاده‌سازی پایه برای سرویس‌های فقط-خواندنی مبتنی بر Dapper.
    /// </summary>
    public abstract class BaseReadOnlyService<TModel, TKey> : IReadOnlyService<TModel, TKey>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionStringName;

        /// <summary>
        /// کوئری انتخاب همه رکوردها. (SELECT ... FROM Table ...)
        /// </summary>
        protected string SqlSelectAll { get; }

        /// <summary>
        /// کوئری انتخاب یک رکورد بر اساس Id. (SELECT ... FROM Table WHERE Id = @Id)
        /// </summary>
        protected string SqlSelectById { get; }

        /// <summary>
        /// بخش ORDER BY برای Paging (مثلاً "ORDER BY CreatedAt DESC").
        /// اگر خالی باشد، در GetPagedAsync از SqlSelectAll بدون Paging استفاده نمی‌شود.
        /// </summary>
        protected string SqlOrderBy { get; }

        protected BaseReadOnlyService(
            IConfiguration configuration,
            string sqlSelectAll,
            string sqlSelectById,
            string sqlOrderBy,
            string connectionStringName = "DefaultConnection")
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            SqlSelectAll = sqlSelectAll ?? throw new ArgumentNullException(nameof(sqlSelectAll));
            SqlSelectById = sqlSelectById ?? throw new ArgumentNullException(nameof(sqlSelectById));
            SqlOrderBy = sqlOrderBy ?? string.Empty;
            _connectionStringName = connectionStringName;
        }

        protected IDbConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString(_connectionStringName);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"Connection string '{_connectionStringName}' is not configured.");
            }

            return new SqlConnection(connectionString);
        }

        public virtual async Task<IEnumerable<TModel>> GetAllAsync()
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<TModel>(SqlSelectAll);
        }

        public virtual async Task<TModel?> GetByIdAsync(TKey id)
        {
            using var connection = CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<TModel>(
                SqlSelectById,
                new { Id = id });

            return result;
        }

        public virtual async Task<(IEnumerable<TModel> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize)
        {
            if (pageNumber <= 0) throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
            if (string.IsNullOrWhiteSpace(SqlOrderBy))
            {
                // اگر ORDER BY نداشته باشیم، نمی‌توانیم paging امن انجام دهیم.
                // در این حالت، به سادگی همه را برمی‌گردانیم.
                var allItems = await GetAllAsync();
                var list = allItems is List<TModel> typed ? typed : new List<TModel>(allItems);
                return (list, list.Count);
            }

            var offset = (pageNumber - 1) * pageSize;

            var pagedSql = $@"
{SqlSelectAll}
{SqlOrderBy}
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

SELECT COUNT(1) FROM ({SqlSelectAll}) AS CountQuery;
";

            using var connection = CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                pagedSql,
                new
                {
                    Offset = offset,
                    PageSize = pageSize
                });

            var items = await multi.ReadAsync<TModel>();
            var totalCount = await multi.ReadFirstAsync<int>();

            return (items, totalCount);
        }
    }
}
