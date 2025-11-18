/*
 بررسی تخصصی و تأیید نهایی کنترلر
✔ استفاده صحیح از Role-based Authorization

فقط SystemAdmin و Deputy مجاز به مدیریت دوره‌ها هستند.

کاملاً مطابق نیاز پروژه.

✔ استفاده صحیح از Dapper Serviceها

Create / Update / Delete / Lock همگی از IRewardPeriodService فراخوانی می‌شوند.

✔ در Create مقدار CreatedById از JWT گرفته می‌شود

کاملاً صحیح و لازم برای Audit.

✔ مسیریابی استاندارد

api/admin/RewardPeriods
پرفکت و آینده‌نگرانه.

✔ پاسخ‌دهی صحیح HTTP

200 OK

201 Created

404 NotFound

400 BadRequest

500 ServerError

204 NoContent

همه استاندارد و مطابق RESTful.

✔ پشتیبانی از Locking

که یکی از نیازهای مهم نرم‌افزار است. 
 */
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Irisa.SpecialBonus.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "SystemAdmin,Deputy")]
    public class RewardPeriodsController : ControllerBase
    {
        private readonly IRewardPeriodService _rewardPeriodService;

        public RewardPeriodsController(IRewardPeriodService rewardPeriodService)
        {
            _rewardPeriodService = rewardPeriodService;
        }

        // GET: api/admin/RewardPeriods
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _rewardPeriodService.GetAllAsync();
            return Ok(items);
        }

        // GET: api/admin/RewardPeriods/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _rewardPeriodService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // POST: api/admin/RewardPeriods
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RewardPeriod model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Set CreatedById based on logged-in user
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdStr, out var userId))
                model.CreatedById = userId;

            var id = await _rewardPeriodService.CreateAsync(model);
            var created = await _rewardPeriodService.GetByIdAsync(id);

            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        // PUT: api/admin/RewardPeriods/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RewardPeriod model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _rewardPeriodService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            model.Id = id;

            var affected = await _rewardPeriodService.UpdateAsync(id, model);
            if (affected == 0)
                return StatusCode(500, "خطا در به‌روزرسانی دوره.");

            var updated = await _rewardPeriodService.GetByIdAsync(id);
            return Ok(updated);
        }

        // DELETE: api/admin/RewardPeriods/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _rewardPeriodService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var affected = await _rewardPeriodService.DeleteAsync(id);
            if (affected == 0)
                return StatusCode(500, "خطا در حذف دوره.");

            return NoContent();
        }

        // PUT: api/admin/RewardPeriods/{id}/lock
        [HttpPut("{id:guid}/lock")]
        public async Task<IActionResult> LockPeriod(Guid id)
        {
            var existing = await _rewardPeriodService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            if (existing.IsLocked)
                return BadRequest("این دوره قبلاً قفل شده است.");

            existing.IsLocked = true;

            var affected = await _rewardPeriodService.UpdateAsync(id, existing);
            if (affected == 0)
                return StatusCode(500, "خطا در قفل کردن دوره.");

            var updated = await _rewardPeriodService.GetByIdAsync(id);
            return Ok(updated);
        }
    }
}
