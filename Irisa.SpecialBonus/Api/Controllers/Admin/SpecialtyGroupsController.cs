using System;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Irisa.SpecialBonus.Api.Controllers.Admin
{
    /// <summary>
    /// مدیریت گروه‌های تخصصی (SpecialtyGroups)
    /// فقط برای نقش‌های SystemAdmin و Deputy
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "SystemAdmin,Deputy")]
    public class SpecialtyGroupsController : ControllerBase
    {
        private readonly ISpecialtyGroupService _specialtyGroupService;

        public SpecialtyGroupsController(ISpecialtyGroupService specialtyGroupService)
        {
            _specialtyGroupService = specialtyGroupService;
        }

        // GET: api/admin/SpecialtyGroups
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _specialtyGroupService.GetAllAsync();
            return Ok(items);
        }

        // GET: api/admin/SpecialtyGroups/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _specialtyGroupService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // GET: api/admin/SpecialtyGroups/by-deputy/{deputyId}
        [HttpGet("by-deputy/{deputyId:guid}")]
        public async Task<IActionResult> GetByDeputy(Guid deputyId)
        {
            var items = await _specialtyGroupService.GetByDeputyIdAsync(deputyId);
            return Ok(items);
        }

        // POST: api/admin/SpecialtyGroups
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SpecialtyGroup model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _specialtyGroupService.CreateAsync(model);
            var created = await _specialtyGroupService.GetByIdAsync(id);

            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        // PUT: api/admin/SpecialtyGroups/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SpecialtyGroup model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _specialtyGroupService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            model.Id = id;

            var affected = await _specialtyGroupService.UpdateAsync(id, model);
            if (affected == 0)
                return StatusCode(500, "خطا در به‌روزرسانی گروه.");

            var updated = await _specialtyGroupService.GetByIdAsync(id);
            return Ok(updated);
        }

        // DELETE: api/admin/SpecialtyGroups/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _specialtyGroupService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var affected = await _specialtyGroupService.DeleteAsync(id);
            if (affected == 0)
                return StatusCode(500, "خطا در حذف گروه.");

            return NoContent();
        }
    }
}
