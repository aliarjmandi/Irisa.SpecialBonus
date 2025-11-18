using System;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Irisa.SpecialBonus.Persistence.Dapper.Base;
using Microsoft.Extensions.Configuration;

namespace Irisa.SpecialBonus.Persistence.Dapper.Services
{
    public class IndicatorService
        : BaseService<Indicator, Indicator, Guid>, IIndicatorService
    {
        private const string SelectAllSql = @"
SELECT
    Id,
    DeputyId,
    Code,
    Name,
    Description,
    Weight,
    MinValue,
    MaxValue,
    DataEntryUserId,
    IsActive,
    SortOrder,
    CreatedAt,
    CreatedById
FROM Indicators";

        private const string SelectByIdSql = SelectAllSql + @"
 WHERE Id = @Id";

        private const string OrderBySql = @"
ORDER BY IsActive DESC, SortOrder ASC, Name ASC";

        private const string InsertSql = @"
INSERT INTO Indicators
(
    Id,
    DeputyId,
    Code,
    Name,
    Description,
    Weight,
    MinValue,
    MaxValue,
    DataEntryUserId,
    IsActive,
    SortOrder,
    CreatedAt,
    CreatedById
)
VALUES
(
    @Id,
    @DeputyId,
    @Code,
    @Name,
    @Description,
    @Weight,
    @MinValue,
    @MaxValue,
    @DataEntryUserId,
    @IsActive,
    @SortOrder,
    @CreatedAt,
    @CreatedById
);

SELECT @Id;
";

        private const string UpdateSql = @"
UPDATE Indicators
SET
    DeputyId       = @DeputyId,
    Code           = @Code,
    Name           = @Name,
    Description    = @Description,
    Weight         = @Weight,
    MinValue       = @MinValue,
    MaxValue       = @MaxValue,
    DataEntryUserId= @DataEntryUserId,
    IsActive       = @IsActive,
    SortOrder      = @SortOrder
WHERE Id = @Id;
";

        private const string DeleteSql = @"
DELETE FROM Indicators
WHERE Id = @Id;
";

        public IndicatorService(IConfiguration configuration)
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

        protected override object GetInsertParameters(Indicator model)
        {
            if (model.Id == Guid.Empty)
                model.Id = Guid.NewGuid();

            if (model.CreatedAt == default)
                model.CreatedAt = DateTime.UtcNow;

            return new
            {
                model.Id,
                model.DeputyId,
                model.Code,
                model.Name,
                model.Description,
                model.Weight,
                model.MinValue,
                model.MaxValue,
                model.DataEntryUserId,
                model.IsActive,
                model.SortOrder,
                model.CreatedAt,
                model.CreatedById
            };
        }

        protected override object GetUpdateParameters(Guid id, Indicator model)
        {
            return new
            {
                Id = id,
                model.DeputyId,
                model.Code,
                model.Name,
                model.Description,
                model.Weight,
                model.MinValue,
                model.MaxValue,
                model.DataEntryUserId,
                model.IsActive,
                model.SortOrder
            };
        }
    }
}
