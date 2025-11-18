using System;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Irisa.SpecialBonus.Persistence.Dapper.Base;
using Microsoft.Extensions.Configuration;

namespace Irisa.SpecialBonus.Persistence.Dapper.Services
{
    public class RewardPeriodService
        : BaseService<RewardPeriod, RewardPeriod, Guid>, IRewardPeriodService
    {
        private const string SelectAllSql = @"
SELECT
    Id,
    DeputyId,
    [Year],
    [Month],
    Title,
    BudgetAmount,
    IsLocked,
    CreatedAt,
    CreatedById
FROM RewardPeriods";

        private const string SelectByIdSql = SelectAllSql + @"
 WHERE Id = @Id";

        private const string OrderBySql = @"
ORDER BY [Year] DESC, [Month] DESC";

        private const string InsertSql = @"
INSERT INTO RewardPeriods
(
    Id,
    DeputyId,
    [Year],
    [Month],
    Title,
    BudgetAmount,
    IsLocked,
    CreatedAt,
    CreatedById
)
VALUES
(
    @Id,
    @DeputyId,
    @Year,
    @Month,
    @Title,
    @BudgetAmount,
    @IsLocked,
    @CreatedAt,
    @CreatedById
);

SELECT @Id;
";

        private const string UpdateSql = @"
UPDATE RewardPeriods
SET
    DeputyId    = @DeputyId,
    [Year]      = @Year,
    [Month]     = @Month,
    Title       = @Title,
    BudgetAmount= @BudgetAmount,
    IsLocked    = @IsLocked
WHERE Id = @Id;
";

        private const string DeleteSql = @"
DELETE FROM RewardPeriods
WHERE Id = @Id;
";

        public RewardPeriodService(IConfiguration configuration)
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

        protected override object GetInsertParameters(RewardPeriod model)
        {
            // Id را اینجا تولید می‌کنیم و در DB همان را Insert می‌کنیم
            if (model.Id == Guid.Empty)
                model.Id = Guid.NewGuid();

            if (model.CreatedAt == default)
                model.CreatedAt = DateTime.UtcNow;

            return new
            {
                model.Id,
                model.DeputyId,
                model.Year,
                model.Month,
                model.Title,
                model.BudgetAmount,
                model.IsLocked,
                model.CreatedAt,
                model.CreatedById
            };
        }

        protected override object GetUpdateParameters(Guid id, RewardPeriod model)
        {
            return new
            {
                Id = id,
                model.DeputyId,
                model.Year,
                model.Month,
                model.Title,
                model.BudgetAmount,
                model.IsLocked
            };
        }
    }
}
