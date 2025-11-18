using System;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Irisa.SpecialBonus.Api.Controllers.Admin
{
    /// <summary>
    /// کنترلر نتایج پاداش ویژه گروه‌ها.
    /// این کنترلر نتیجه نهایی محاسبات را برمی‌گرداند.
    /// GroupManager فقط مشاهده دارد.
    /// Deputy و SystemAdmin دسترسی کامل دارند.
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "SystemAdmin,Deputy,GroupManager")]
    public class GroupRewardResultsController : ControllerBase
    {
        private readonly IGroupRewardResultService _service;

        public GroupRewardResultsController(IGroupRewardResultService service)
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
        [Authorize(Roles = "SystemAdmin,Deputy")]
        public async Task<IActionResult> Create([FromBody] GroupRewardResult model)
        {
            var id = await _service.CreateAsync(model);
            var created = await _service.GetByIdAsync(id);
            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "SystemAdmin,Deputy")]
        public async Task<IActionResult> Update(Guid id, [FromBody] GroupRewardResult model)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            model.Id = id;

            var affected = await _service.UpdateAsync(id, model);
            if (affected == 0)
                return StatusCode(500, "خطا در بروزرسانی.");

            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "SystemAdmin")]
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
