using System;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Irisa.SpecialBonus.Api.Controllers.Admin
{
    /// <summary>
    /// مدیریت مشاهده کاربران سیستم (AspNetUsers)
    /// فقط برای نقش‌های مدیریت (SystemAdmin, Deputy) و فقط خواندنی
    /// </summary>
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "SystemAdmin,Deputy")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/admin/Users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // GET: api/admin/Users/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // GET: api/admin/Users/by-username/{userName}
        [HttpGet("by-username/{userName}")]
        public async Task<IActionResult> GetByUserName(string userName)
        {
            var user = await _userService.GetByUserNameAsync(userName);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // GET: api/admin/Users/by-role/{roleName}
        [HttpGet("by-role/{roleName}")]
        public async Task<IActionResult> GetByRole(string roleName)
        {
            var users = await _userService.GetByRoleAsync(roleName);
            return Ok(users);
        }
    }
}
