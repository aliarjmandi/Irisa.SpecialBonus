using System;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Irisa.SpecialBonus.Persistence.Dapper.Base;
using Microsoft.Extensions.Configuration;

namespace Irisa.SpecialBonus.Persistence.Dapper.Services
{
    public class IndicatorValueService
        : BaseService<IndicatorValue, IndicatorValue, Guid>, IIndicatorValueService
    {
        private const string SelectAllSql = @"
SELECT
    Id,
    PeriodId,
    GroupId,
    IndicatorId,
    Value,
    EnteredById,
    EnteredAt,
    ModifiedAt
FROM IndicatorValues";

        private const string SelectByIdSql = SelectAllSql + @"
 WHERE Id = @Id";

        private const string OrderBySql = @"
ORDER BY EnteredAt DESC";

        private const string InsertSql = @"
INSERT INTO IndicatorValues
(
    Id,
    PeriodId,
    GroupId,
    IndicatorId,
    Value,
    EnteredById,
    EnteredAt,
    ModifiedAt
)
VALUES
(
    @Id,
    @PeriodId,
    @GroupId,
    @IndicatorId,
    @Value,
    @EnteredById,
    @EnteredAt,
    @ModifiedAt
);

SELECT @Id;
";

        private const string UpdateSql = @"
UPDATE IndicatorValues
SET
    PeriodId    = @PeriodId,
    GroupId     = @GroupId,
    IndicatorId = @IndicatorId,
    Value       = @Value,
    EnteredById = @EnteredById,
    EnteredAt   = @EnteredAt,
    ModifiedAt  = @ModifiedAt
WHERE Id = @Id;
";

        private const string DeleteSql = @"
DELETE FROM IndicatorValues
WHERE Id = @Id;
";

        public IndicatorValueService(IConfiguration configuration)
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

        protected override object GetInsertParameters(IndicatorValue model)
        {
            if (model.Id == Guid.Empty)
                model.Id = Guid.NewGuid();

            if (model.EnteredAt == default)
                model.EnteredAt = DateTime.UtcNow;

            return new
            {
                model.Id,
                model.PeriodId,
                model.GroupId,
                model.IndicatorId,
                model.Value,
                model.EnteredById,
                model.EnteredAt,
                model.ModifiedAt
            };
        }

        protected override object GetUpdateParameters(Guid id, IndicatorValue model)
        {
            if (model.ModifiedAt == null || model.ModifiedAt == default)
                model.ModifiedAt = DateTime.UtcNow;

            return new
            {
                Id = id,
                model.PeriodId,
                model.GroupId,
                model.IndicatorId,
                model.Value,
                model.EnteredById,
                model.EnteredAt,
                model.ModifiedAt
            };
        }
    }
}
