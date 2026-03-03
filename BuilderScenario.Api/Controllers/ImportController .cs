using AutoMapper;
using BuilderScenario.Api.Dtos;
using BuilderScenario.Core.Entities;
using BuilderScenario.Infrastructure.Services;
using BuilderScenario.Infrastructure.Services.Import;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuilderScenario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly ImportService _importService;
        private readonly ScenarioRepository _scenarioRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ImportController> _logger;

        public ImportController(
            ImportService importService,
            ScenarioRepository scenarioRepository,
            IMapper mapper,
            ILogger<ImportController> logger)
        {
            _importService = importService;
            _scenarioRepository = scenarioRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ImportResultDto>> Import(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Файл не выбран или пуст");

                _logger.LogInformation($"Начало импорта файла: {file.FileName}, размер: {file.Length}");

                var tempPath = Path.GetTempFileName();
                try
                {
                    using (var stream = System.IO.File.Create(tempPath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    _logger.LogInformation($"Файл сохранен временно: {tempPath}");

                    var scenario = _importService.Import(tempPath, file.FileName);

                    _logger.LogInformation($"Сценарий импортирован: {scenario.Name}, групп: {scenario.Groups.Count}");

                    // Если имя пустое, ставим заглушку
                    if (string.IsNullOrWhiteSpace(scenario.Name))
                    {
                        scenario.Name = $"Импорт от {DateTime.Now:yyyy-MM-dd HH:mm}";
                    }

                    await _scenarioRepository.AddAsync(scenario);

                    _logger.LogInformation($"Сценарий сохранен в БД с ID: {scenario.Id}");

                    return Ok(_mapper.Map<ImportResultDto>(scenario));
                }
                finally
                {
                    if (System.IO.File.Exists(tempPath))
                        System.IO.File.Delete(tempPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте");
                return StatusCode(500, $"Ошибка при импорте: {ex.Message}");
            }
        }
    }
}