using System;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Irisa.SpecialBonus.Persistence.Dapper.Base;
using Microsoft.Extensions.Configuration;

namespace Irisa.SpecialBonus.Persistence.Dapper.Services
{
    public class ManagerialCoefficientService
        : BaseService<ManagerialCoefficient, ManagerialCoefficient, Guid>, IManagerialCoefficientService
    {
        private const string SelectAllSql = @"
SELECT
    Id,
    PeriodId,
    GroupId,
    Value,
    EnteredById,
    EnteredAt,
    ModifiedAt
FROM ManagerialCoefficients";

        private const string SelectByIdSql = SelectAllSql + @"
 WHERE Id = @Id";

        private const string OrderBySql = @"
ORDER BY EnteredAt DESC";

        private const string InsertSql = @"
INSERT INTO ManagerialCoefficients
(
    Id,
    PeriodId,
    GroupId,
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
    @Value,
    @EnteredById,
    @EnteredAt,
    @ModifiedAt
);

SELECT @Id;
";

        private const string UpdateSql = @"
UPDATE ManagerialCoefficients
SET
    PeriodId   = @PeriodId,
    GroupId    = @GroupId,
    Value      = @Value,
    EnteredById= @EnteredById,
    EnteredAt  = @EnteredAt,
    ModifiedAt = @ModifiedAt
WHERE Id = @Id;
";

        private const string DeleteSql = @"
DELETE FROM ManagerialCoefficients
WHERE Id = @Id;
";

        public ManagerialCoefficientService(IConfiguration configuration)
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

        protected override object GetInsertParameters(ManagerialCoefficient model)
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
                model.Value,
                model.EnteredById,
                model.EnteredAt,
                model.ModifiedAt
            };
        }

        protected override object GetUpdateParameters(Guid id, ManagerialCoefficient model)
        {
            if (model.ModifiedAt == null || model.ModifiedAt == default)
                model.ModifiedAt = DateTime.UtcNow;

            return new
            {
                Id = id,
                model.PeriodId,
                model.GroupId,
                model.Value,
                model.EnteredById,
                model.EnteredAt,
                model.ModifiedAt
            };
        }
    }
}
