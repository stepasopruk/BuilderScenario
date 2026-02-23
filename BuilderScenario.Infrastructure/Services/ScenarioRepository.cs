using BuilderScenario.Core.Entities;
using BuilderScenario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BuilderScenario.Infrastructure.Services
{
    public class ScenarioRepository
    {
        private readonly ScenarioDbContext _context;

        public ScenarioRepository(ScenarioDbContext context)
        {
            _context = context;
        }

        public async Task<List<Scenario>> GetAllAsync()
        {
            return await _context.Scenarios
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Scenario?> GetByIdAsync(int id)
        {
            var scenario = await _context.Scenarios
        .Include(s => s.Groups)
            .ThenInclude(g => g.Steps)
                .ThenInclude(st => st.Actions)
        .FirstOrDefaultAsync(s => s.Id == id);

            if (scenario == null)
                return null;

            // Сортировка по Order
            scenario.Groups = scenario.Groups
                .OrderBy(g => g.Order)
                .Select(g =>
                {
                    g.Steps = g.Steps
                        .OrderBy(s => s.Order)
                        .Select(s =>
                        {
                            s.Actions = s.Actions
                                .OrderBy(a => a.Order)
                                .ToList();
                            return s;
                        })
                        .ToList();

                    return g;
                })
                .ToList();

            return scenario;
        }

        public async Task SaveAsync(Scenario scenario)
        {
            var existing = await _context.Scenarios
                .Include(s => s.Groups)
                .ThenInclude(g => g.Steps)
                .ThenInclude(st => st.Actions)
                .FirstOrDefaultAsync(s => s.Id == scenario.Id);

            if (existing == null)
            {
                _context.Scenarios.Add(scenario);
            }
            else
            {
                existing.Name = scenario.Name;

                // обновляем порядок групп
                for (int i = 0; i < scenario.Groups.Count; i++)
                {
                    var incomingGroup = scenario.Groups[i];
                    var existingGroup = existing.Groups
                        .First(g => g.Id == incomingGroup.Id);

                    existingGroup.Order = i;

                    // шаги
                    for (int j = 0; j < incomingGroup.Steps.Count; j++)
                    {
                        var incomingStep = incomingGroup.Steps[j];
                        var existingStep = existingGroup.Steps
                            .First(s => s.Id == incomingStep.Id);

                        existingStep.Order = j;

                        // действия
                        for (int k = 0; k < incomingStep.Actions.Count; k++)
                        {
                            var incomingAction = incomingStep.Actions[k];
                            var existingAction = existingStep.Actions
                                .First(a => a.Id == incomingAction.Id);

                            existingAction.Order = k;
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(Scenario scenario)
        {
            _context.Scenarios.Add(scenario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Scenario scenario)
        {
            _context.Scenarios.Update(scenario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var scenario = await _context.Scenarios.FindAsync(id);
            if (scenario == null)
                return;

            _context.Scenarios.Remove(scenario);
            await _context.SaveChangesAsync();
        }
    }
}
