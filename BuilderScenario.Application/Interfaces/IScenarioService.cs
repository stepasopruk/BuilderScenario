using BuilderScenario.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuilderScenario.Application.Interfaces
{
    public interface IScenarioService
    {
        Task SaveAsync(Scenario scenario);
        Task<List<Scenario>> GetAllAsync();
        Task<Scenario> GetByIdAsync(int id);
    }
}
