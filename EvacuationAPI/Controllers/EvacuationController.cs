using EvacuationAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EvacuationAPI.Controllers;

[Route("api/evacuation-zones")]
[ApiController]
public class EvacuationController
{
    private readonly EvacuationService _evacuationService;
    
    private readonly ILogger _logger;

    public EvacuationController(EvacuationService evacuationService, ILogger<EvacuationController> logger)
    {
        _evacuationService = evacuationService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> addEvacuationZone([FromBody] EvacuationZonesDTO evacuationZonesDto)
    {
        try
        {
            _logger.LogInformation("Adding Evacuation Zone " + evacuationZonesDto);
            if(evacuationZonesDto.UrgencyLevel < 0 || evacuationZonesDto.UrgencyLevel > 5) throw new Exception("Urgency Level must be between 0 and 5");
            await _evacuationService.addEvacuationZone(evacuationZonesDto);
            return new OkObjectResult(evacuationZonesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            
            return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
        }

        
    }
}