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
    /// کنترلر مقادیر شاخص‌ها (IndicatorValues)
    /// این کنترلر برای ثبت، ویرایش و مشاهده مقادیر شاخص‌های هر گروه در هر دوره است.
    /// نقش IndicatorDataEntry می‌تواند داده وارد کند.
    /// نقش Deputy و SystemAdmin کنترل کامل دارند.
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "SystemAdmin,Deputy,IndicatorDataEntry")]
    public class IndicatorValuesController : ControllerBase
    {
        private readonly IIndicatorValueService _service;

        public IndicatorValuesController(IIndicatorValueService service)
        {
            _service = service;
        }

        // GET: api/admin/IndicatorValues
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        // GET: api/admin/IndicatorValues/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // POST: api/admin/IndicatorValues
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IndicatorValue model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdStr, out var enteredBy))
                model.EnteredById = enteredBy;

            if (model.EnteredAt == default)
                model.EnteredAt = DateTime.UtcNow;

            var id = await _service.CreateAsync(model);
            var created = await _service.GetByIdAsync(id);

            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        // PUT: api/admin/IndicatorValues/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] IndicatorValue model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            model.Id = id;
            model.ModifiedAt = DateTime.UtcNow;

            var affected = await _service.UpdateAsync(id, model);
            if (affected == 0)
                return StatusCode(500, "خطا در بروزرسانی مقدار شاخص.");

            var updated = await _service.GetByIdAsync(id);
            return Ok(updated);
        }

        // DELETE: api/admin/IndicatorValues/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "SystemAdmin,Deputy")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var affected = await _service.DeleteAsync(id);
            if (affected == 0)
                return StatusCode(500, "خطا در حذف مقدار شاخص.");

            return NoContent();
        }
    }
}
