using BuilderScenario.Application.Interfaces;
using BuilderScenario.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuilderScenario.Infrastructure.Services
{
    public class ScenarioService : IScenarioService
    {
        private static readonly List<Scenario> _storage = new();

        public Task SaveAsync(Scenario scenario)
        {
            var existing = _storage.FirstOrDefault(x => x.Id == scenario.Id);

            if (existing == null)
                _storage.Add(scenario);
            else
                _storage[_storage.IndexOf(existing)] = scenario;

            return Task.CompletedTask;
        }

        public Task<List<Scenario>> GetAllAsync()
        {
            return Task.FromResult(_storage.ToList());
        }

        public Task<Scenario> GetByIdAsync(int id)
        {
            return Task.FromResult(_storage.FirstOrDefault(x => x.Id == id));
        }
    }
}
