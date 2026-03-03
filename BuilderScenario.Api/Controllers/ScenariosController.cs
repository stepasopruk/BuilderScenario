using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using BuilderScenario.Core.Entities;
using BuilderScenario.Infrastructure.Data;
using BuilderScenario.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using BuilderScenario.Infrastructure.Services;

namespace BuilderScenario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScenariosController : ControllerBase
    {
        private readonly ScenarioDbContext _context;
        private readonly IMapper _mapper;
        private readonly ExportApiClient _exportClient;

        public ScenariosController(ScenarioDbContext context, IMapper mapper, ExportApiClient exportClient)
        {
            _context = context;
            _mapper = mapper;
            _exportClient = exportClient;
        }

        // GET: api/scenarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScenarioDto>>> GetScenarios()
        {
            var scenarios = await _context.Scenarios
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Steps)
                        .ThenInclude(s => s.Actions)
                .ToListAsync();

            return Ok(_mapper.Map<List<ScenarioDto>>(scenarios));
        }

        // GET: api/scenarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ScenarioDto>> GetScenario(int id)
        {
            var scenario = await _context.Scenarios
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Steps)
                        .ThenInclude(s => s.Actions)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (scenario == null)
                return NotFound();

            return Ok(_mapper.Map<ScenarioDto>(scenario));
        }

        // POST: api/scenarios
        [HttpPost]
        public async Task<ActionResult<ScenarioDto>> CreateScenario(CreateScenarioDto createDto)
        {
            // Маппим DTO в Entity
            var scenario = _mapper.Map<Scenario>(createDto);

            _context.Scenarios.Add(scenario);
            await _context.SaveChangesAsync();

            //await _context.Scenarios.AddAsync(scenario);

            // Маппим обратно в DTO для ответа
            var scenarioDto = _mapper.Map<ScenarioDto>(scenario);

            return CreatedAtAction(nameof(GetScenario), new { id = scenarioDto.Id }, scenarioDto);
        }

        // PUT: api/scenarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateScenario(int id, CreateScenarioDto updateDto)
        {
            var scenario = await _context.Scenarios
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Steps)
                        .ThenInclude(s => s.Actions)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (scenario == null)
                return NotFound();

            // Обновляем существующую сущность из DTO
            _mapper.Map(updateDto, scenario);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Scenarios.Any(s => s.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/scenarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScenario(int id)
        {
            var scenario = await _context.Scenarios.FindAsync(id);
            if (scenario == null)
                return NotFound();

            _context.Scenarios.Remove(scenario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/export")]
        public async Task<IActionResult> Export(int id)
        {
            var scenario = await _context.Scenarios.FindAsync(id);

            if (scenario == null)
                return NotFound();

            var json = await _exportClient.ExportToJsonAsync(scenario);

            return Ok(json);
        }
    }
}