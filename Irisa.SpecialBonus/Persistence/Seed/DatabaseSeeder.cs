using System.Data;
using Dapper;
using Irisa.SpecialBonus.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Irisa.SpecialBonus.Persistence.Seed
{
    /// <summary>
    /// سیدر تستی برای:
    /// - نقش‌ها و کاربران Identity (با UserManager/RoleManager)
    /// - داده‌های دامنه‌ای (معاونت، گروه‌ها، دوره، شاخص‌ها، ضرایب و مقادیر) با Dapper
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<DatabaseSeeder> _logger;

        private string ConnectionString => _configuration.GetConnectionString("DefaultConnection")!;

        public DatabaseSeeder(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<DatabaseSeeder> logger)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("DatabaseSeeder started...");

            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedDomainDataAsync();

            _logger.LogInformation("DatabaseSeeder finished.");
        }

        private IDbConnection CreateConnection()
            => new SqlConnection(ConnectionString);

        #region Identity (Roles & Users)

        private async Task SeedRolesAsync()
        {
            string[] roles =
            {
                "SystemAdmin",
                "Deputy",
                "IndicatorDataEntry",
                "GroupManager"
            };

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var result = await _roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpperInvariant()
                    });

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                        throw new Exception($"Error creating role {roleName}: {errors}");
                    }
                }
            }
        }

        private ApplicationUser _admin = null!;
        private ApplicationUser _deputy = null!;
        private ApplicationUser _indParticip = null!;
        private ApplicationUser _indSupport = null!;
        private ApplicationUser _indDev = null!;
        private ApplicationUser _indApex = null!;
        private ApplicationUser _gmAcc = null!;
        private ApplicationUser _gmProd = null!;
        private ApplicationUser _gmComm = null!;
        private ApplicationUser _gmIt = null!;

        private async Task SeedUsersAsync()
        {
            _admin = await EnsureUserAsync("admin", "admin@irisa.local", "SystemAdmin");
            _deputy = await EnsureUserAsync("deputy.is", "deputy.is@irisa.local", "Deputy");

            _indParticip = await EnsureUserAsync("indicator.participation", "indicator.participation@irisa.local", "IndicatorDataEntry");
            _indSupport = await EnsureUserAsync("indicator.support", "indicator.support@irisa.local", "IndicatorDataEntry");
            _indDev = await EnsureUserAsync("indicator.dev", "indicator.dev@irisa.local", "IndicatorDataEntry");
            _indApex = await EnsureUserAsync("indicator.apex", "indicator.apex@irisa.local", "IndicatorDataEntry");

            _gmAcc = await EnsureUserAsync("gm.accounting", "gm.accounting@irisa.local", "GroupManager");
            _gmProd = await EnsureUserAsync("gm.production", "gm.production@irisa.local", "GroupManager");
            _gmComm = await EnsureUserAsync("gm.commerce", "gm.commerce@irisa.local", "GroupManager");
            _gmIt = await EnsureUserAsync("gm.it", "gm.it@irisa.local", "GroupManager");
        }

        private async Task<ApplicationUser> EnsureUserAsync(string userName, string email, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true
                };

                // پسورد تستی مشترک
                var createResult = await _userManager.CreateAsync(user, "Test@12345");
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Error creating user {userName}: {errors}");
                }
            }

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, roleName);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(" | ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Error adding user {userName} to role {roleName}: {errors}");
                }
            }

            return user;
        }

        #endregion

        #region Domain data with Dapper

        private async Task SeedDomainDataAsync()
        {
            // ترتیب: Deputy → Groups → Period → Indicators → Snapshots → ManagerialCoeff → IndicatorValues
            var deputyId = await EnsureDeputyAsync();
            var groupIds = await EnsureGroupsAsync(deputyId);
            var periodId = await EnsureRewardPeriodAsync(deputyId);
            var indicatorIds = await EnsureIndicatorsAsync(deputyId);
            await EnsureSnapshotsAsync(periodId, groupIds);
            await EnsureManagerialCoefficientsAsync(periodId, groupIds);
            await EnsureIndicatorValuesAsync(periodId, groupIds, indicatorIds);
        }

        private async Task<Guid> EnsureDeputyAsync()
        {
            using var conn = CreateConnection();

            const string selectSql = @"SELECT TOP 1 Id FROM Deputies WHERE Code = @Code";
            var existing = await conn.QueryFirstOrDefaultAsync<Guid?>(selectSql, new { Code = "ISD" });

            if (existing.HasValue && existing.Value != Guid.Empty)
                return existing.Value;

            var id = Guid.NewGuid();

            const string insertSql = @"
INSERT INTO Deputies (Id, Code, Name, IsActive, CreatedAt)
VALUES (@Id, @Code, @Name, @IsActive, @CreatedAt);";

            await conn.ExecuteAsync(insertSql, new
            {
                Id = id,
                Code = "ISD",
                Name = "معاونت سیستم‌های اطلاعاتی",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            return id;
        }

        private async Task<Dictionary<string, Guid>> EnsureGroupsAsync(Guid deputyId)
        {
            using var conn = CreateConnection();

            // key: ErpGroupCode, value: Id
            var result = new Dictionary<string, Guid>();

            async Task<Guid> EnsureGroup(string code, string name, Guid managerUserId)
            {
                const string selectSql = @"SELECT TOP 1 Id FROM SpecialtyGroups WHERE ErpGroupCode = @Code";
                var existing = await conn.QueryFirstOrDefaultAsync<Guid?>(selectSql, new { Code = code });

                if (existing.HasValue && existing.Value != Guid.Empty)
                    return existing.Value;

                var id = Guid.NewGuid();

                const string insertSql = @"
INSERT INTO SpecialtyGroups
    (Id, DeputyId, ErpGroupCode, Name, ManagerUserId, IsActive, CreatedAt)
VALUES
    (@Id, @DeputyId, @ErpGroupCode, @Name, @ManagerUserId, @IsActive, @CreatedAt);";

                await conn.ExecuteAsync(insertSql, new
                {
                    Id = id,
                    DeputyId = deputyId,
                    ErpGroupCode = code,
                    Name = name,
                    ManagerUserId = managerUserId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });

                return id;
            }

            result["ACCT"] = await EnsureGroup("ACCT", "گروه حسابداری", _gmAcc.Id);
            result["PROD"] = await EnsureGroup("PROD", "گروه تولید", _gmProd.Id);
            result["COMM"] = await EnsureGroup("COMM", "گروه بازرگانی", _gmComm.Id);
            result["IT"] = await EnsureGroup("IT", "گروه آی‌تی", _gmIt.Id);

            return result;
        }

        private async Task<Guid> EnsureRewardPeriodAsync(Guid deputyId)
        {
            using var conn = CreateConnection();

            const string selectSql = @"
SELECT TOP 1 Id
FROM RewardPeriods
WHERE DeputyId = @DeputyId AND [Year] = @Year AND [Month] = @Month;";

            var existing = await conn.QueryFirstOrDefaultAsync<Guid?>(selectSql, new
            {
                DeputyId = deputyId,
                Year = 1404,
                Month = 6
            });

            if (existing.HasValue && existing.Value != Guid.Empty)
                return existing.Value;

            var id = Guid.NewGuid();

            const string insertSql = @"
INSERT INTO RewardPeriods
    (Id, DeputyId, [Year], [Month], Title, BudgetAmount, IsLocked, CreatedAt, CreatedById)
VALUES
    (@Id, @DeputyId, @Year, @Month, @Title, @BudgetAmount, @IsLocked, @CreatedAt, @CreatedById);";

            await conn.ExecuteAsync(insertSql, new
            {
                Id = id,
                DeputyId = deputyId,
                Year = 1404,
                Month = 6,
                Title = "شهریور 1404",
                BudgetAmount = 1000000000m,
                IsLocked = false,
                CreatedAt = DateTime.UtcNow,
                CreatedById = _deputy.Id
            });

            return id;
        }

        private async Task<Dictionary<string, Guid>> EnsureIndicatorsAsync(Guid deputyId)
        {
            using var conn = CreateConnection();

            var dict = new Dictionary<string, Guid>();

            async Task<Guid> EnsureIndicator(
                string code,
                string name,
                decimal weight,
                Guid dataEntryUserId,
                int sortOrder)
            {
                const string selectSql = @"SELECT TOP 1 Id FROM Indicators WHERE DeputyId = @DeputyId AND Code = @Code";
                var existing = await conn.QueryFirstOrDefaultAsync<Guid?>(selectSql, new { DeputyId = deputyId, Code = code });

                if (existing.HasValue && existing.Value != Guid.Empty)
                    return existing.Value;

                var id = Guid.NewGuid();

                const string insertSql = @"
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
);";

                // توجه: MinValue/MaxValue و Valueها نرمال‌شده بین 0 و 1 هستند
                await conn.ExecuteAsync(insertSql, new
                {
                    Id = id,
                    DeputyId = deputyId,
                    Code = code,
                    Name = name,
                    Description = name,
                    Weight = weight,
                    MinValue = 0m,   // ✅ بازه 0 تا 1 تا با decimal(5,4) سازگار شود
                    MaxValue = 1m,   // ✅
                    DataEntryUserId = dataEntryUserId,
                    IsActive = true,
                    SortOrder = sortOrder,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = _admin.Id
                });

                return id;
            }

            dict["PART"] = await EnsureIndicator("PART", "مشارکت در ارتقای شاخص‌های تابلو اهداف معاونت", 0.30m, _indParticip.Id, 1);
            dict["SUPP"] = await EnsureIndicator("SUPP", "بهبود شاخص‌های درخواست‌های پشتیبانی پروژه‌ها", 0.25m, _indSupport.Id, 2);
            dict["DEV"] = await EnsureIndicator("DEV", "بهبود شاخص‌های درخواست‌های توسعه پروژه‌ها", 0.25m, _indDev.Id, 3);
            dict["APEX"] = await EnsureIndicator("APEX", "پروژه بازطراحی IS Suite با تکنولوژی APEX", 0.20m, _indApex.Id, 4);

            return dict;
        }

        private async Task EnsureSnapshotsAsync(Guid periodId, Dictionary<string, Guid> groups)
        {
            using var conn = CreateConnection();

            async Task EnsureSnapshot(string groupCode, int memberCount)
            {
                var groupId = groups[groupCode];

                const string selectSql = @"
SELECT TOP 1 Id
FROM PeriodGroupSnapshots
WHERE PeriodId = @PeriodId AND GroupId = @GroupId;";

                var existing = await conn.QueryFirstOrDefaultAsync<Guid?>(selectSql, new { PeriodId = periodId, GroupId = groupId });
                if (existing.HasValue && existing.Value != Guid.Empty)
                    return;

                const string insertSql = @"
INSERT INTO PeriodGroupSnapshots
    (Id, PeriodId, GroupId, MemberCount, VerifiedById, VerifiedAt)
VALUES
    (@Id, @PeriodId, @GroupId, @MemberCount, @VerifiedById, @VerifiedAt);";

                await conn.ExecuteAsync(insertSql, new
                {
                    Id = Guid.NewGuid(),
                    PeriodId = periodId,
                    GroupId = groupId,
                    MemberCount = memberCount,
                    VerifiedById = _deputy.Id,
                    VerifiedAt = DateTime.UtcNow
                });
            }

            await EnsureSnapshot("ACCT", 5);
            await EnsureSnapshot("PROD", 7);
            await EnsureSnapshot("COMM", 4);
            await EnsureSnapshot("IT", 6);
        }

        private async Task EnsureManagerialCoefficientsAsync(Guid periodId, Dictionary<string, Guid> groups)
        {
            using var conn = CreateConnection();

            async Task EnsureCoeff(string groupCode, decimal value)
            {
                var groupId = groups[groupCode];

                const string selectSql = @"
SELECT TOP 1 Id
FROM ManagerialCoefficients
WHERE PeriodId = @PeriodId AND GroupId = @GroupId;";

                var existing = await conn.QueryFirstOrDefaultAsync<Guid?>(selectSql, new { PeriodId = periodId, GroupId = groupId });
                if (existing.HasValue && existing.Value != Guid.Empty)
                    return;

                const string insertSql = @"
INSERT INTO ManagerialCoefficients
    (Id, PeriodId, GroupId, Value, EnteredById, EnteredAt, ModifiedAt)
VALUES
    (@Id, @PeriodId, @GroupId, @Value, @EnteredById, @EnteredAt, @ModifiedAt);";

                await conn.ExecuteAsync(insertSql, new
                {
                    Id = Guid.NewGuid(),
                    PeriodId = periodId,
                    GroupId = groupId,
                    Value = value,
                    EnteredById = _deputy.Id,
                    EnteredAt = DateTime.UtcNow,
                    ModifiedAt = (DateTime?)null
                });
            }

            // ضرایب مدیریتی نمونه (۱ یعنی بدون تغییر)
            await EnsureCoeff("ACCT", 1.05m);
            await EnsureCoeff("PROD", 1.10m);
            await EnsureCoeff("COMM", 0.95m);
            await EnsureCoeff("IT", 1.00m);
        }

        private async Task EnsureIndicatorValuesAsync(
            Guid periodId,
            Dictionary<string, Guid> groups,
            Dictionary<string, Guid> indicators)
        {
            using var conn = CreateConnection();

            async Task EnsureValue(string groupCode, string indCode, decimal value)
            {
                var groupId = groups[groupCode];
                var indicatorId = indicators[indCode];

                const string selectSql = @"
SELECT TOP 1 Id
FROM IndicatorValues
WHERE PeriodId = @PeriodId AND GroupId = @GroupId AND IndicatorId = @IndicatorId;";

                var existing = await conn.QueryFirstOrDefaultAsync<Guid?>(selectSql, new
                {
                    PeriodId = periodId,
                    GroupId = groupId,
                    IndicatorId = indicatorId
                });

                if (existing.HasValue && existing.Value != Guid.Empty)
                    return;

                const string insertSql = @"
INSERT INTO IndicatorValues
    (Id, PeriodId, GroupId, IndicatorId, Value, EnteredById, EnteredAt, ModifiedAt)
VALUES
    (@Id, @PeriodId, @GroupId, @IndicatorId, @Value, @EnteredById, @EnteredAt, @ModifiedAt);";

                await conn.ExecuteAsync(insertSql, new
                {
                    Id = Guid.NewGuid(),
                    PeriodId = periodId,
                    GroupId = groupId,
                    IndicatorId = indicatorId,
                    Value = value,          // مقدار نرمال‌شده بین 0 و 1
                    EnteredById = _admin.Id,
                    EnteredAt = DateTime.UtcNow,
                    ModifiedAt = (DateTime?)null
                });
            }

            // مقادیر نرمال‌شده: 80% => 0.80m
            // Accounting
            await EnsureValue("ACCT", "PART", 0.80m);
            await EnsureValue("ACCT", "SUPP", 0.75m);
            await EnsureValue("ACCT", "DEV", 0.70m);
            await EnsureValue("ACCT", "APEX", 0.60m);

            // Production
            await EnsureValue("PROD", "PART", 0.90m);
            await EnsureValue("PROD", "SUPP", 0.85m);
            await EnsureValue("PROD", "DEV", 0.88m);
            await EnsureValue("PROD", "APEX", 0.70m);

            // Commerce
            await EnsureValue("COMM", "PART", 0.75m);
            await EnsureValue("COMM", "SUPP", 0.65m);
            await EnsureValue("COMM", "DEV", 0.60m);
            await EnsureValue("COMM", "APEX", 0.55m);

            // IT
            await EnsureValue("IT", "PART", 0.85m);
            await EnsureValue("IT", "SUPP", 0.90m);
            await EnsureValue("IT", "DEV", 0.92m);
            await EnsureValue("IT", "APEX", 0.80m);
        }

        #endregion
    }
}
