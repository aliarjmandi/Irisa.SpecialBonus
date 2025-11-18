using System;
using Irisa.SpecialBonus.Domain.Entities;

namespace Irisa.SpecialBonus.Domain.Interfaces.Services
{
    public interface IIndicatorValueService
        : IGenericService<IndicatorValue, IndicatorValue, Guid>
    {
    }
}
