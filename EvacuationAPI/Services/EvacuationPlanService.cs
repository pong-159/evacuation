using EvacuationAPI.DTOs;
using EvacuationAPI.Entities.Enums;
using EvacuationAPI.Models;
using EvacuationAPI.Utils;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EvacuationAPI;

public class EvacuationPlanService
{
    private readonly AppDbContext.AppDbContext _context;
    private readonly ILogger _logger;

    public EvacuationPlanService(AppDbContext.AppDbContext context, ILogger<EvacuationPlanService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task createPlanAsync()
    {
        try
        {
            var evacuationZones = await _context.EvacuationZones
                .OrderByDescending(z => z.UrgencyLevel)
                .ToListAsync();

            foreach (var evacuationZone in evacuationZones) 
                _logger.LogDebug("Evacuation Zones {Zone}", evacuationZone);

            var vehicleInUsed = new HashSet<string>();
            foreach (var evacuationZone in evacuationZones)
            {
                var numberOfPeople = evacuationZone.NumberOfPeople;
                while (numberOfPeople > 0)
                {
                    _logger.LogDebug("Looking for Vehicles");
                    _logger.LogDebug("Vehicles Size {Count}", await _context.Vehicles.CountAsync());
                    
                    var vehicles = await _context.Vehicles
                        .Where(v => v.IsAvailable && !vehicleInUsed.Contains(v.Id))
                        .OrderByDescending(v => Math.Abs(v.Capacity - numberOfPeople))
                        .FirstOrDefaultAsync();

                    _logger.LogDebug("Vehicles value {Vehicle}", vehicles);
                    if (vehicles == null) break;
                    vehicleInUsed.Add(vehicles.Id);

                    var numberOfPeopleToEvacuate = Math.Min(vehicles.Capacity, numberOfPeople);
                    var vehicleSpeed = CalculatorUtils.ConvertKphToMps(vehicles.Speed);
                    var distance = CalculatorUtils.GetDistance(evacuationZone.Latitude, vehicles.Latitude,
                        evacuationZone.Longitude, vehicles.Longitude);
                    var time = distance / vehicleSpeed;
                    var timeToEvacuateInMinute = time * 60;
                    
                    _logger.LogDebug(
                        "Vehicle speed: {Speed}m/s, Distance: {Distance}m, Time: {Time}min",
                        vehicleSpeed, distance, timeToEvacuateInMinute);
                        
                    var evacuationPlan = new EvacuationPlan(evacuationZone.Id, vehicles.Id,
                        numberOfPeopleToEvacuate, timeToEvacuateInMinute);
                    numberOfPeople -= numberOfPeopleToEvacuate;

                    _logger.LogDebug("Number of people {Count}", numberOfPeople);

                    await _context.EvacuationPlans.AddAsync(evacuationPlan);

                    vehicles.IsAvailable = false;
                    _context.Vehicles.Update(vehicles);
                }

                // evacuationZone.NumberOfEvacuatedPeople = evacuationZone.NumberOfPeople - numberOfPeople;
                //
                // _context.EvacuationZones.Update(evacuationZone);
            }

            await _context.SaveChangesAsync();
            _logger.LogDebug("createPlanAsync Done");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in createPlanAsync");
            throw;
        }
    }

    public async Task<EvacuationPlan> updatePlanAsync(string vehicleId, int numberOfPeople)
    {
        try
        {
            var evacuationPlan = await _context.EvacuationPlans.FirstAsync(p =>
                p.EvacuationStatus == EvacuationStatusEnum.InProgress
                && p.VehicleId == vehicleId);

            evacuationPlan.EvacuationStatus = EvacuationStatusEnum.Completed;
            evacuationPlan.NumberOfPeople = numberOfPeople;

            _context.EvacuationPlans.Update(evacuationPlan);

            var evacuationZone = await _context.EvacuationZones.FirstAsync(z => z.Id == evacuationPlan.ZoneId);
            evacuationZone.NumberOfEvacuatedPeople += numberOfPeople;

            if (evacuationZone.NumberOfEvacuatedPeople >= evacuationZone.NumberOfPeople)
            {
                // evacuationZone.EvacuationStatus = EvacuationStatusEnum.Completed;
            }

            _context.EvacuationZones.Update(evacuationZone);

            await _context.SaveChangesAsync();
            return evacuationPlan;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in updatePlanAsync");
            throw;
        }
    }

    public async Task<IEnumerable<EvacuationPlansDTO>> listPlanAsync()
    {
        return await _context.EvacuationPlans.Select(ep =>
            new EvacuationPlansDTO(
                ep.ZoneId,
                ep.VehicleId,
                ep.getETAWithUnit(),
                ep.NumberOfPeople
            )
        ).ToListAsync();
    }

    public async Task clearAllAsync()
    {
        try
        {
            await _context.EvacuationPlans.ExecuteDeleteAsync();

            var vehicles = await _context.Vehicles.ToListAsync();
            foreach (var vehicle in vehicles)
            {
                vehicle.IsAvailable = true;
            }

            _context.Vehicles.UpdateRange(vehicles);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in clearAllAsync");
            throw;
        }
    }
}