using System;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Irisa.SpecialBonus.Api.Controllers.Admin
{
    /// <summary>
    /// وب‌سرویس مدیریت معاونت‌ها (Deputies) در ناحیه ادمین
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "SystemAdmin,Deputy")]
    public class DeputiesController : ControllerBase
    {
        private readonly IDeputyService _deputyService;

        public DeputiesController(IDeputyService deputyService)
        {
            _deputyService = deputyService;
        }

        /// <summary>
        /// دریافت لیست تمام معاونت‌ها
        /// </summary>
        // GET: api/admin/Deputies
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _deputyService.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// دریافت همه معاونت‌های فعال
        /// </summary>
        // GET: api/admin/Deputies/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var items = await _deputyService.GetActiveAsync();
            return Ok(items);
        }

        /// <summary>
        /// دریافت معاونت بر اساس Id
        /// </summary>
        // GET: api/admin/Deputies/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _deputyService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        /// <summary>
        /// دریافت معاونت بر اساس کُد (مثلاً ISD)
        /// </summary>
        // GET: api/admin/Deputies/by-code/{code}
        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var item = await _deputyService.GetByCodeAsync(code);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        /// <summary>
        /// ایجاد معاونت جدید
        /// </summary>
        // POST: api/admin/Deputies
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Deputy model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _deputyService.CreateAsync(model);
            var created = await _deputyService.GetByIdAsync(id);

            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        /// <summary>
        /// ویرایش اطلاعات یک معاونت
        /// </summary>
        // PUT: api/admin/Deputies/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Deputy model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _deputyService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            model.Id = id;

            var affected = await _deputyService.UpdateAsync(id, model);
            if (affected == 0)
                return StatusCode(500, "خطا در به‌روزرسانی معاونت.");

            var updated = await _deputyService.GetByIdAsync(id);
            return Ok(updated);
        }

        /// <summary>
        /// حذف یک معاونت
        /// </summary>
        // DELETE: api/admin/Deputies/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _deputyService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var affected = await _deputyService.DeleteAsync(id);
            if (affected == 0)
                return StatusCode(500, "خطا در حذف معاونت.");

            return NoContent();
        }
    }
}
