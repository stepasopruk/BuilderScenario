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
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Steps)
                        .ThenInclude(st => st.Actions)
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

        public async Task AddAsync(Scenario scenario)
        {
            // Важно: сбрасываем ID, чтобы БД сама назначила новые
            ResetIds(scenario);

            await _context.Scenarios.AddAsync(scenario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Scenario scenario)
        {
            var existing = await _context.Scenarios
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Steps)
                        .ThenInclude(st => st.Actions)
                .FirstOrDefaultAsync(s => s.Id == scenario.Id);

            if (existing == null)
                throw new Exception($"Scenario with id {scenario.Id} not found");

            // Обновляем основные поля
            existing.Name = scenario.Name;

            // Обновляем группы (удаляем старые, добавляем новые)
            UpdateGroups(existing, scenario);

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

        private void ResetIds(Scenario scenario)
        {
            scenario.Id = 0;

            foreach (var group in scenario.Groups)
            {
                group.Id = 0;
                group.ScenarioId = 0;

                foreach (var step in group.Steps)
                {
                    step.Id = 0;
                    step.ActionGroupId = 0;

                    foreach (var action in step.Actions)
                    {
                        action.Id = 0;
                        action.StepItemId = 0;
                    }
                }
            }
        }

        private void UpdateGroups(Scenario existing, Scenario updated)
        {
            // Удаляем группы, которых нет в updated
            var groupsToRemove = existing.Groups
                .Where(eg => !updated.Groups.Any(ug => ug.Id == eg.Id && eg.Id > 0))
                .ToList();

            foreach (var group in groupsToRemove)
            {
                _context.ActionGroups.Remove(group);
            }

            // Обновляем существующие и добавляем новые группы
            foreach (var updatedGroup in updated.Groups)
            {
                var existingGroup = existing.Groups
                    .FirstOrDefault(eg => eg.Id == updatedGroup.Id && eg.Id > 0);

                if (existingGroup != null)
                {
                    // Обновляем существующую группу
                    existingGroup.Name = updatedGroup.Name;
                    existingGroup.Order = updatedGroup.Order;

                    UpdateSteps(existingGroup, updatedGroup);
                }
                else
                {
                    // Добавляем новую группу
                    updatedGroup.Id = 0;
                    updatedGroup.ScenarioId = existing.Id;

                    foreach (var step in updatedGroup.Steps)
                    {
                        step.Id = 0;
                        step.ActionGroupId = 0;

                        foreach (var action in step.Actions)
                        {
                            action.Id = 0;
                            action.StepItemId = 0;
                        }
                    }

                    existing.Groups.Add(updatedGroup);
                }
            }
        }

        private void UpdateSteps(ActionGroup existingGroup, ActionGroup updatedGroup)
        {
            // Удаляем шаги, которых нет в updatedGroup
            var stepsToRemove = existingGroup.Steps
                .Where(es => !updatedGroup.Steps.Any(us => us.Id == es.Id && es.Id > 0))
                .ToList();

            foreach (var step in stepsToRemove)
            {
                _context.Steps.Remove(step);
            }

            // Обновляем существующие и добавляем новые шаги
            foreach (var updatedStep in updatedGroup.Steps)
            {
                var existingStep = existingGroup.Steps
                    .FirstOrDefault(es => es.Id == updatedStep.Id && es.Id > 0);

                if (existingStep != null)
                {
                    // Обновляем существующий шаг
                    existingStep.Name = updatedStep.Name;
                    existingStep.Order = updatedStep.Order;

                    UpdateActions(existingStep, updatedStep);
                }
                else
                {
                    // Добавляем новый шаг
                    updatedStep.Id = 0;
                    updatedStep.ActionGroupId = existingGroup.Id;

                    foreach (var action in updatedStep.Actions)
                    {
                        action.Id = 0;
                        action.StepItemId = 0;
                    }

                    existingGroup.Steps.Add(updatedStep);
                }
            }
        }

        private void UpdateActions(StepItem existingStep, StepItem updatedStep)
        {
            // Удаляем действия, которых нет в updatedStep
            var actionsToRemove = existingStep.Actions
                .Where(ea => !updatedStep.Actions.Any(ua => ua.Id == ea.Id && ea.Id > 0))
                .ToList();

            foreach (var action in actionsToRemove)
            {
                _context.Actions.Remove(action);
            }

            // Обновляем существующие и добавляем новые действия
            foreach (var updatedAction in updatedStep.Actions)
            {
                var existingAction = existingStep.Actions
                    .FirstOrDefault(ea => ea.Id == updatedAction.Id && ea.Id > 0);

                if (existingAction != null)
                {
                    // Обновляем существующее действие
                    existingAction.Name = updatedAction.Name;
                    existingAction.Order = updatedAction.Order;
                }
                else
                {
                    // Добавляем новое действие
                    updatedAction.Id = 0;
                    updatedAction.StepItemId = existingStep.Id;
                    existingStep.Actions.Add(updatedAction);
                }
            }
        }
    }
}