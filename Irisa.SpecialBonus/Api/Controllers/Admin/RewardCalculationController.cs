using System;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Irisa.SpecialBonus.Api.Controllers.Admin
{
    /// <summary>
    /// کنترلر محاسبه پاداش ویژه برای یک دوره.
    /// فقط SystemAdmin و Deputy می‌توانند محاسبه را اجرا کنند.
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "SystemAdmin,Deputy")]
    public class RewardCalculationController : ControllerBase
    {
        private readonly IRewardCalculationService _calculationService;

        public RewardCalculationController(IRewardCalculationService calculationService)
        {
            _calculationService = calculationService;
        }

        /// <summary>
        /// اجرای محاسبه پاداش ویژه برای یک دوره مشخص.
        /// </summary>
        /// <param name="periodId">شناسه دوره</param>
        [HttpPost("{periodId:guid}/run")]
        public async Task<IActionResult> Run(Guid periodId)
        {
            var results = await _calculationService.CalculateAndSaveForPeriodAsync(periodId);
            return Ok(results);
        }
    }
}
