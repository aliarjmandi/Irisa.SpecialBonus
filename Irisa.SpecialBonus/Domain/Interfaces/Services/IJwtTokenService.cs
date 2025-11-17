using System;
using System.Collections.Generic;

namespace Irisa.SpecialBonus.Domain.Interfaces.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(Guid userId, string userName, IEnumerable<string> roles);
    }
}
