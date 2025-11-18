using System;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Irisa.SpecialBonus.Persistence.Dapper.Base;
using Microsoft.Extensions.Configuration;

namespace Irisa.SpecialBonus.Persistence.Dapper.Services
{
    public class PeriodGroupSnapshotService
        : BaseService<PeriodGroupSnapshot, PeriodGroupSnapshot, Guid>, IPeriodGroupSnapshotService
    {
        private const string SelectAllSql = @"
SELECT
    Id,
    PeriodId,
    GroupId,
    MemberCount,
    VerifiedById,
    VerifiedAt
FROM PeriodGroupSnapshots";

        private const string SelectByIdSql = SelectAllSql + @"
 WHERE Id = @Id";

        private const string OrderBySql = @"
ORDER BY PeriodId DESC";

        private const string InsertSql = @"
INSERT INTO PeriodGroupSnapshots
(
    Id,
    PeriodId,
    GroupId,
    MemberCount,
    VerifiedById,
    VerifiedAt
)
VALUES
(
    @Id,
    @PeriodId,
    @GroupId,
    @MemberCount,
    @VerifiedById,
    @VerifiedAt
);

SELECT @Id;
";

        private const string UpdateSql = @"
UPDATE PeriodGroupSnapshots
SET
    PeriodId    = @PeriodId,
    GroupId     = @GroupId,
    MemberCount = @MemberCount,
    VerifiedById= @VerifiedById,
    VerifiedAt  = @VerifiedAt
WHERE Id = @Id;
";

        private const string DeleteSql = @"
DELETE FROM PeriodGroupSnapshots
WHERE Id = @Id;
";

        public PeriodGroupSnapshotService(IConfiguration configuration)
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

        protected override object GetInsertParameters(PeriodGroupSnapshot model)
        {
            if (model.Id == Guid.Empty)
                model.Id = Guid.NewGuid();

            return new
            {
                model.Id,
                model.PeriodId,
                model.GroupId,
                model.MemberCount,
                model.VerifiedById,
                model.VerifiedAt
            };
        }

        protected override object GetUpdateParameters(Guid id, PeriodGroupSnapshot model)
        {
            return new
            {
                Id = id,
                model.PeriodId,
                model.GroupId,
                model.MemberCount,
                model.VerifiedById,
                model.VerifiedAt
            };
        }
    }
}
