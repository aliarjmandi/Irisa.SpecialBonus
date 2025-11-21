using Irisa.SpecialBonus.Domain.Entities;
using Irisa.SpecialBonus.Persistence.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Irisa.SpecialBonus.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser?> GetByIdAsync(Guid id);

        Task<ApplicationUser?> GetByNameAsync(string name);
        Task<ApplicationUser?> GetByUserNameAsync(string name);

        Task<ApplicationUser?> GetByRoleAsync(string roleName);
    }
}
