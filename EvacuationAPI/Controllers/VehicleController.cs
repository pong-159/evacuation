using EvacuationAPI.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EvacuationAPI.Controllers;

[Route("api/vehicles")]
[ApiController]
public class VehicleController
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger _logger;

    public VehicleController(IVehicleService vehicleService, ILogger<VehicleController> logger)
    {
        _vehicleService = vehicleService;
        _logger = logger;
    }
    [HttpPost]
    public async Task<IActionResult> addVehicle([FromBody] VehicleDto vehicleDto)
    {
        try
        {
     
            _logger.LogInformation("Adding vehicle " + vehicleDto);
            var isDataSave = await _vehicleService.addVehicle(vehicleDto);
            if (!isDataSave)
            {
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
        


        return new OkObjectResult(vehicleDto);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetVehicleStatus()
    {
        try
        {
            _logger.LogInformation("Getting Vehicles List");
            var result = await _vehicleService.ListVehicle();
            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting vehicles list: {ex.Message}");
            return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

}