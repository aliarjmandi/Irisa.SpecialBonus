using System;
using System.Collections.Generic;

namespace Irisa.SpecialBonus.Api.Models.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }

        public string UserName { get; set; } = null!;
        public string? Email { get; set; }

        public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    }
}
