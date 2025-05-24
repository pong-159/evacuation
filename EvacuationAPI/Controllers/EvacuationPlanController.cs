using EvacuationAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EvacuationAPI.Controllers;

[Route("api/evacuations/")]
[ApiController]
public class EvacuationPlanController : ControllerBase
{
    private readonly EvacuationPlanService _evacuationPlanService;
    private readonly EvacuationService _evacuationService;
    private readonly ILogger _logger;

    public EvacuationPlanController(EvacuationService evacuationService, EvacuationPlanService evacuationPlanService, ILogger<EvacuationPlanController> logger)
    {
        _evacuationService = evacuationService;
        _evacuationPlanService = evacuationPlanService;
        _logger = logger;
    }

    [HttpPost]
    [Route("plan")]
    public async Task<IActionResult> CreateEvacuationPlan()
    {
        try
        {
            _logger.LogInformation("Generating Evacuation Plan");
            await _evacuationPlanService.createPlanAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("status")]
    public async Task<IActionResult> GetEvacuationStatus()
    {
        try
        {
            _logger.LogInformation("Getting Evacuation Status");
            var result = await _evacuationService.ListEvacuationZone();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> UpdateEvacuationPlan([FromBody] UpdateEvacuationPlanDto updateEvacuationPlanDto)
    {
        try
        {
            _logger.LogInformation("Updating Evacuation Plan " + updateEvacuationPlanDto);
            var evacuationPlan = await _evacuationPlanService.updatePlanAsync(updateEvacuationPlanDto.vehicleId, updateEvacuationPlanDto.numberOfPeople);
            return Ok(evacuationPlan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    [HttpDelete]
    [Route("clear")]
    public async Task<IActionResult> ClearEvacuationPlan()
    {
        try
        {
            _logger.LogInformation("Clearing Evacuation Plan and Reset Everthing");
            await _evacuationPlanService.clearAllAsync();

            return Ok("Cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetEvacuationPlan()
    {
        try
        {
            _logger.LogInformation("Getting Evacuation Status");
            var result = await _evacuationPlanService.listPlanAsync();
            _logger.LogInformation("Evacuation Plan " + result.Count());
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}