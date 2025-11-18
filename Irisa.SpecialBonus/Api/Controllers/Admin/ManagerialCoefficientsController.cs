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
    /// کنترلر ضرایب مدیریتی (Managerial Coefficients)
    /// این ضرایب توسط معاون (Deputy) ثبت و ویرایش می‌شود.
    /// فقط SystemAdmin و Deputy اجازه ثبت/ویرایش دارند.
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "SystemAdmin,Deputy")]
    public class ManagerialCoefficientsController : ControllerBase
    {
        private readonly IManagerialCoefficientService _service;

        public ManagerialCoefficientsController(IManagerialCoefficientService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ManagerialCoefficient model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdStr, out var uid))
                model.EnteredById = uid;

            model.EnteredAt = DateTime.UtcNow;

            var id = await _service.CreateAsync(model);
            var created = await _service.GetByIdAsync(id);

            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ManagerialCoefficient model)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            model.Id = id;
            model.ModifiedAt = DateTime.UtcNow;

            var affected = await _service.UpdateAsync(id, model);
            if (affected == 0)
                return StatusCode(500, "خطا در بروزرسانی.");

            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var affected = await _service.DeleteAsync(id);
            if (affected == 0)
                return StatusCode(500, "خطا در حذف.");

            return NoContent();
        }
    }
}
