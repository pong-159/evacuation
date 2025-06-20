﻿using EvacuationAPI.Caching;
using EvacuationAPI.DTOs;
using EvacuationAPI.Entities.Enums;
using EvacuationAPI.Models;
using EvacuationAPI.Utils;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EvacuationAPI;



public class EvacuationPlanService : IEvacuationPlanService
{
    private readonly AppDbContext.AppDbContext _context;
    private readonly ILogger _logger;
    private readonly ICacheService _cacheService;
    private double _abs;

    public EvacuationPlanService(AppDbContext.AppDbContext context, ILogger<EvacuationPlanService> logger, ICacheService cacheService)
    {
        _context = context;
        _logger = logger;
        _cacheService = cacheService;
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
                    // _logger.LogDebug("Vehicles Size {Count}", await _context.Vehicles.CountAsync());
                    
                    var vehiclesList = await _context.Vehicles.ToListAsync();
                    var vehicles =   vehiclesList.Where(v => v.IsAvailable && !vehicleInUsed.Contains(v.Id))
                        .OrderBy(v => Math.Abs(CalculatorUtils.GetDistance(v,evacuationZone)))
                        .ThenBy(v => Math.Abs(v.Capacity - numberOfPeople))
                        .FirstOrDefault();

                    _logger.LogDebug("Vehicles value {Vehicle}", vehicles);
                    if (vehicles == null) break;
                    vehicleInUsed.Add(vehicles.Id);

                    var numberOfPeopleToEvacuate = Math.Min(vehicles.Capacity, numberOfPeople);
                    var vehicleSpeed = CalculatorUtils.ConvertKphToMps(vehicles.Speed);
                    var distance = CalculatorUtils.GetDistance(vehicles, evacuationZone);
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
            var evacuationPlan = await _context.EvacuationPlans.FirstOrDefaultAsync(p =>
                p.EvacuationStatus == EvacuationStatusEnum.InProgress
                && p.VehicleId == vehicleId);

            if (evacuationPlan == null)
            {
                throw new Exception("No Evacuation Plan Found");
            }
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
            
            var cachedData = _cacheService.Get<IEnumerable<EvacuationZones>>();
            var updatedData =  cachedData?.Select(c =>
            {
                _logger.LogDebug("Processing evacuation zone: " +  c.Id + ", with evacuationZone: " + evacuationZone.Id );
                
                if (c.Id == evacuationZone.Id)
                {
                    return evacuationZone;
                }

                return c;
            }).ToList();
            
            _logger.LogInformation("Updated evacuation zone data: " + updatedData);
            if(updatedData != null) _cacheService.Update(updatedData);

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
        var result =  await _context.EvacuationPlans.Select(ep =>
            new EvacuationPlansDTO(
                ep.ZoneId,
                ep.VehicleId,
                ep.getETAWithUnit(),
                ep.NumberOfPeople
            )
        ).ToListAsync();
        
    
        
        result.ForEach(r => _logger.LogDebug("Evacuation Plan {Plan}", r));
        return result;
    }

    public async Task clearAllAsync()
    {
        try
        {
            _context.EvacuationPlans.RemoveRange(_context.EvacuationPlans);

            var vehicles = await _context.Vehicles.ToListAsync();
            foreach (var vehicle in vehicles)
            {
                vehicle.IsAvailable = true;
            }

            _context.Vehicles.UpdateRange(vehicles);
            await _context.SaveChangesAsync();

            _cacheService.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in clearAllAsync");
            throw;
        }
    }
}