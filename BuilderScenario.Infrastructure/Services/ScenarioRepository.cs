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

        private void SyncGroups(Scenario existing, Scenario updated)
        {
            // Удалённые группы
            foreach (var existingGroup in existing.Groups.ToList())
            {
                if (!updated.Groups.Any(g => g.Id == existingGroup.Id))
                    _context.ActionGroups.Remove(existingGroup);
            }

            foreach (var group in updated.Groups)
            {
                var existingGroup = existing.Groups
                    .FirstOrDefault(g => g.Id == group.Id);

                if (existingGroup == null)
                {
                    existing.Groups.Add(group);
                }
                else
                {
                    _context.Entry(existingGroup)
                        .CurrentValues.SetValues(group);

                    SyncSteps(existingGroup, group);
                }
            }
        }

        private void SyncSteps(ActionGroup existing, ActionGroup updated)
        {
            foreach (var existingStep in existing.Steps.ToList())
            {
                if (!updated.Steps.Any(s => s.Id == existingStep.Id))
                    _context.Steps.Remove(existingStep);
            }

            foreach (var step in updated.Steps)
            {
                var existingStep = existing.Steps
                    .FirstOrDefault(s => s.Id == step.Id);

                if (existingStep == null)
                {
                    existing.Steps.Add(step);
                }
                else
                {
                    _context.Entry(existingStep)
                        .CurrentValues.SetValues(step);

                    SyncActions(existingStep, step);
                }
            }
        }

        private void SyncActions(StepItem existing, StepItem updated)
        {
            foreach (var existingAction in existing.Actions.ToList())
            {
                if (!updated.Actions.Any(a => a.Id == existingAction.Id))
                    _context.Actions.Remove(existingAction);
            }

            foreach (var action in updated.Actions)
            {
                var existingAction = existing.Actions
                    .FirstOrDefault(a => a.Id == action.Id);

                if (existingAction == null)
                {
                    existing.Actions.Add(action);
                }
                else
                {
                    _context.Entry(existingAction)
                        .CurrentValues.SetValues(action);
                }
            }
        }
    }
}
