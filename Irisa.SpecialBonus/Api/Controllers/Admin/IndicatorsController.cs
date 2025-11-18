using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Irisa.SpecialBonus.Api.Controllers.Admin
{
    /// <summary>
    /// کنترلر شاخص‌ها (Indicators)
    /// این کنترلر برای مدیریت شاخص‌ها توسط نقش‌های SystemAdmin و Deputy استفاده می‌شود.
    /// نقش IndicatorDataEntry فقط مجاز به مشاهده شاخص‌ها است.
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "SystemAdmin,Deputy,IndicatorDataEntry")]
    public class IndicatorsController : ControllerBase
    {
        private readonly IIndicatorService _indicatorService;

        public IndicatorsController(IIndicatorService indicatorService)
        {
            _indicatorService = indicatorService;
        }

        // GET: api/admin/Indicators
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _indicatorService.GetAllAsync();
            return Ok(items);
        }

        // GET: api/admin/Indicators/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _indicatorService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // POST: api/admin/Indicators
        // فقط معاون و ادمین می‌توانند شاخص ایجاد کنند
        [HttpPost]
        [Authorize(Roles = "SystemAdmin,Deputy")]
        public async Task<IActionResult> Create([FromBody] Indicator model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdStr, out var userId))
                model.CreatedById = userId;

            var id = await _indicatorService.CreateAsync(model);
            var created = await _indicatorService.GetByIdAsync(id);

            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        // PUT: api/admin/Indicators/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "SystemAdmin,Deputy")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Indicator model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _indicatorService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            model.Id = id;

            var affected = await _indicatorService.UpdateAsync(id, model);
            if (affected == 0)
                return StatusCode(500, "خطا در به‌روزرسانی شاخص.");

            var updated = await _indicatorService.GetByIdAsync(id);
            return Ok(updated);
        }

        // DELETE: api/admin/Indicators/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "SystemAdmin,Deputy")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _indicatorService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var affected = await _indicatorService.DeleteAsync(id);
            if (affected == 0)
                return StatusCode(500, "خطا در حذف شاخص.");

            return NoContent();
        }
    }
}
