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
    /// پیاده‌سازی Dapper برای جدول SpecialtyGroups
    /// </summary>
    public class SpecialtyGroupService
        : BaseService<SpecialtyGroup, SpecialtyGroup, Guid>, ISpecialtyGroupService
    {
        private const string SelectAllSql = @"
SELECT
    Id,
    DeputyId,
    ErpGroupCode,
    Name,
    ManagerUserId,
    IsActive,
    CreatedAt
FROM SpecialtyGroups";

        private const string SelectByIdSql = SelectAllSql + @"
 WHERE Id = @Id";

        private const string OrderBySql = @"
ORDER BY Name ASC";

        private const string InsertSql = @"
INSERT INTO SpecialtyGroups
(
    Id,
    DeputyId,
    ErpGroupCode,
    Name,
    ManagerUserId,
    IsActive,
    CreatedAt
)
VALUES
(
    @Id,
    @DeputyId,
    @ErpGroupCode,
    @Name,
    @ManagerUserId,
    @IsActive,
    @CreatedAt
);

SELECT @Id;
";

        private const string UpdateSql = @"
UPDATE SpecialtyGroups
SET
    DeputyId      = @DeputyId,
    ErpGroupCode  = @ErpGroupCode,
    Name          = @Name,
    ManagerUserId = @ManagerUserId,
    IsActive      = @IsActive
WHERE Id = @Id;
";

        private const string DeleteSql = @"
DELETE FROM SpecialtyGroups
WHERE Id = @Id;
";

        private const string SelectByDeputySql = SelectAllSql + @"
 WHERE DeputyId = @DeputyId
" + OrderBySql + ";";

        public SpecialtyGroupService(IConfiguration configuration)
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

        protected override object GetInsertParameters(SpecialtyGroup model)
        {
            if (model.Id == Guid.Empty)
                model.Id = Guid.NewGuid();

            if (model.CreatedAt == default)
                model.CreatedAt = DateTime.UtcNow;

            return new
            {
                model.Id,
                model.DeputyId,
                model.ErpGroupCode,
                model.Name,
                model.ManagerUserId,
                model.IsActive,
                model.CreatedAt
            };
        }

        protected override object GetUpdateParameters(Guid id, SpecialtyGroup model)
        {
            return new
            {
                Id = id,
                model.DeputyId,
                model.ErpGroupCode,
                model.Name,
                model.ManagerUserId,
                model.IsActive
            };
        }

        public async Task<IEnumerable<SpecialtyGroup>> GetByDeputyIdAsync(Guid deputyId)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<SpecialtyGroup>(
                SelectByDeputySql,
                new { DeputyId = deputyId });
        }
    }
}
