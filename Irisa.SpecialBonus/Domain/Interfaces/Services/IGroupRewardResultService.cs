using System;
using Irisa.SpecialBonus.Domain.Entities;

namespace Irisa.SpecialBonus.Domain.Interfaces.Services
{
    public interface IGroupRewardResultService
        : IGenericService<GroupRewardResult, GroupRewardResult, Guid>
    {
    }
}
