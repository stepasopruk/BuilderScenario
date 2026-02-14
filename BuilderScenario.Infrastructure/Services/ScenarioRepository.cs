using BuilderScenario.Core.Entities;
using BuilderScenario.Infrastructure.Data;

namespace BuilderScenario.Infrastructure.Services
{
    public class ScenarioRepository
    {
        private readonly ScenarioDbContext _context;

        public ScenarioRepository(ScenarioDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(Scenario scenario)
        {
            _context.Scenarios.Add(scenario);
            await _context.SaveChangesAsync();
        }
    }
}
