using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Irisa.SpecialBonus.Api.Models.Auth;
using Irisa.SpecialBonus.Domain.Interfaces.Services;
using Irisa.SpecialBonus.Persistence.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Irisa.SpecialBonus.Api.Configuration;

namespace Irisa.SpecialBonus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService,
            IOptions<JwtSettings> jwtOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _jwtSettings = jwtOptions.Value;
        }

        /// <summary>
        /// ورود کاربر و دریافت توکن JWT
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // جستجو بر اساس UserName یا Email
            ApplicationUser? user = await _userManager.FindByNameAsync(request.UserNameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(request.UserNameOrEmail);
            }

            if (user == null)
                return Unauthorized("نام کاربری/ایمیل یا کلمه عبور نادرست است.");

            // بررسی کلمه عبور
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
                return Unauthorized("نام کاربری/ایمیل یا کلمه عبور نادرست است.");

            // نقش‌های کاربر
            var roles = await _userManager.GetRolesAsync(user);

            // تولید توکن
            var token = _jwtTokenService.GenerateToken(user.Id, user.UserName!, roles);

            var response = new LoginResponse
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                UserName = user.UserName!,
                Email = user.Email,
                Roles = roles
            };

            return Ok(response);
        }

        /// <summary>
        /// نمونه‌ی ساده برای بررسی توکن سمت کلاینت
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity?.Name;

            return Ok(new
            {
                UserId = userId,
                UserName = userName,
                Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray()
            });
        }
    }
}
