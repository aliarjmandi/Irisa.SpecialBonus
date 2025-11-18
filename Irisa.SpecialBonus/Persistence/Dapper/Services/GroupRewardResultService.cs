using System;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Irisa.SpecialBonus.Persistence.Dapper.Base;
using Microsoft.Extensions.Configuration;

namespace Irisa.SpecialBonus.Persistence.Dapper.Services
{
    public class GroupRewardResultService
        : BaseService<GroupRewardResult, GroupRewardResult, Guid>, IGroupRewardResultService
    {
        private const string SelectAllSql = @"
SELECT
    Id,
    PeriodId,
    GroupId,
    TechnicalScore,
    FinalCoefficient,
    EvaluatedMembers,
    RewardAmount,
    PerCapitaReward,
    CalculatedAt
FROM GroupRewardResults";

        private const string SelectByIdSql = SelectAllSql + @"
 WHERE Id = @Id";

        private const string OrderBySql = @"
ORDER BY CalculatedAt DESC";

        private const string InsertSql = @"
INSERT INTO GroupRewardResults
(
    Id,
    PeriodId,
    GroupId,
    TechnicalScore,
    FinalCoefficient,
    EvaluatedMembers,
    RewardAmount,
    PerCapitaReward,
    CalculatedAt
)
VALUES
(
    @Id,
    @PeriodId,
    @GroupId,
    @TechnicalScore,
    @FinalCoefficient,
    @EvaluatedMembers,
    @RewardAmount,
    @PerCapitaReward,
    @CalculatedAt
);

SELECT @Id;
";

        private const string UpdateSql = @"
UPDATE GroupRewardResults
SET
    PeriodId        = @PeriodId,
    GroupId         = @GroupId,
    TechnicalScore  = @TechnicalScore,
    FinalCoefficient= @FinalCoefficient,
    EvaluatedMembers= @EvaluatedMembers,
    RewardAmount    = @RewardAmount,
    PerCapitaReward = @PerCapitaReward,
    CalculatedAt    = @CalculatedAt
WHERE Id = @Id;
";

        private const string DeleteSql = @"
DELETE FROM GroupRewardResults
WHERE Id = @Id;
";

        public GroupRewardResultService(IConfiguration configuration)
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

        protected override object GetInsertParameters(GroupRewardResult model)
        {
            if (model.Id == Guid.Empty)
                model.Id = Guid.NewGuid();

            if (model.CalculatedAt == default)
                model.CalculatedAt = DateTime.UtcNow;

            return new
            {
                model.Id,
                model.PeriodId,
                model.GroupId,
                model.TechnicalScore,
                model.FinalCoefficient,
                model.EvaluatedMembers,
                model.RewardAmount,
                model.PerCapitaReward,
                model.CalculatedAt
            };
        }

        protected override object GetUpdateParameters(Guid id, GroupRewardResult model)
        {
            return new
            {
                Id = id,
                model.PeriodId,
                model.GroupId,
                model.TechnicalScore,
                model.FinalCoefficient,
                model.EvaluatedMembers,
                model.RewardAmount,
                model.PerCapitaReward,
                model.CalculatedAt
            };
        }
    }
}
