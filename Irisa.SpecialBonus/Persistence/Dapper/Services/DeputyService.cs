using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Irisa.SpecialBonus.Persistence.Dapper.Base;
using Microsoft.Extensions.Configuration;

namespace Irisa.SpecialBonus.Persistence.Dapper.Services
{
    /// <summary>
    /// سرویس مدیریت داده‌های معاونت‌ها (Deputies) با استفاده از Dapper
    /// </summary>
    public class DeputyService
        : BaseService<Deputy, Deputy, Guid>, IDeputyService
    {
        // تمام ستون‌هایی که فعلاً نیاز داریم
        private const string SelectAllSql = @"
SELECT
    Id,
    Code,
    Name,
    IsActive,
    CreatedAt
FROM Deputies";

        private const string SelectByIdSql = SelectAllSql + @"
 WHERE Id = @Id";

        private const string OrderBySql = @"
ORDER BY Name ASC";

        private const string InsertSql = @"
INSERT INTO Deputies
(
    Id,
    Code,
    Name,
    IsActive,
    CreatedAt
)
VALUES
(
    @Id,
    @Code,
    @Name,
    @IsActive,
    @CreatedAt
);

SELECT @Id;
";

        private const string UpdateSql = @"
UPDATE Deputies
SET
    Code     = @Code,
    Name     = @Name,
    IsActive = @IsActive
WHERE Id = @Id;
";

        private const string DeleteSql = @"
DELETE FROM Deputies
WHERE Id = @Id;
";

        public DeputyService(IConfiguration configuration)
            : base(
                  configuration,
                  sqlSelectAll: SelectAllSql,
                  sqlSelectById: SelectByIdSql,
                  sqlOrderBy: OrderBySql,
                  sqlInsert: InsertSql,
                  sqlUpdate: UpdateSql,
                  sqlDelete: DeleteSql,
                  connectionStringName: "DefaultConnection")
        {
        }

        /// <summary>
        /// تنظیم پارامترهای مورد نیاز برای INSERT
        /// </summary>
        protected override object GetInsertParameters(Deputy model)
        {
            if (model.Id == Guid.Empty)
                model.Id = Guid.NewGuid();

            if (model.CreatedAt == default)
                model.CreatedAt = DateTime.UtcNow;

            // اگر از بیرون مقدار IsActive نیامده، پیش‌فرض فعال در نظر بگیر
            if (model.IsActive == default)
                model.IsActive = true;

            return new
            {
                model.Id,
                model.Code,
                model.Name,
                model.IsActive,
                model.CreatedAt
            };
        }

        /// <summary>
        /// تنظیم پارامترهای مورد نیاز برای UPDATE
        /// </summary>
        protected override object GetUpdateParameters(Guid id, Deputy model)
        {
            return new
            {
                Id = id,
                model.Code,
                model.Name,
                model.IsActive
            };
        }

        /// <summary>
        /// دریافت یک معاونت بر اساس کُد (مثلاً ISD)
        /// </summary>
        public async Task<Deputy?> GetByCodeAsync(string code)
        {
            const string sql = SelectAllSql + @"
 WHERE Code = @Code";

            using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Deputy>(sql, new { Code = code });
        }

        /// <summary>
        /// دریافت همه معاونت‌های فعال
        /// </summary>
        public async Task<IEnumerable<Deputy>> GetActiveAsync()
        {
            const string sql = SelectAllSql + @"
 WHERE IsActive = 1" + OrderBySql;

            using var conn = CreateConnection();
            return await conn.QueryAsync<Deputy>(sql);
        }
    }
}
