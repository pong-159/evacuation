using EvacuationAPI.Caching;
using EvacuationAPI.DTOs;
using EvacuationAPI.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvacuationAPI;




public class EvacuationService : IEvacuationService
{
    private readonly AppDbContext.AppDbContext _context;
    private readonly ICacheService _cache;

    public EvacuationService(AppDbContext.AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
        initData();
    }

    public async Task<bool> addEvacuationZone(EvacuationZonesDTO evacuationZonesDto)
    {
        try
        {
            var evacuationZone = new EvacuationZones(evacuationZonesDto);
             _context.EvacuationZones.Add(evacuationZone);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    
    public async Task<IEnumerable<EvacuationStatusDTO>> ListEvacuationZone()
    {
            var resultZone =  await _context.EvacuationZones
                .ToListAsync();
            var result = resultZone.Select(ConvertZoneToEvacuationStatusDto).ToList();
            
            _cache.Update(resultZone);
            
            
            return result;
    }

    public static EvacuationStatusDTO ConvertZoneToEvacuationStatusDto(EvacuationZones ez)
    {
        return new EvacuationStatusDTO(ez.Id, new LocationCoordinate(ez.Latitude, ez.Longitude), ez.NumberOfPeople,ez.NumberOfEvacuatedPeople, ez.NumberOfRemainingPeople() ,ez.UrgencyLevel);
    }


    public void initData(){
        
        if(_context.EvacuationZones.Any()) return;
        for (int i = 1; i <= 10; i++)
        {
            var zone = new EvacuationZones("z" + i, new LocationCoordinate(12.39 * i, 21.684 * i), 10 * i, (i % 5) + 1 );
            _context.EvacuationZones.Add(zone);
        }
        _context.SaveChanges();
        
    }
    
    public async Task clearZonesAsync()
    {
        try
        {
            _context.EvacuationZones.RemoveRange(_context.EvacuationZones);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.Error("clearZonesAsync Error " + ex.Message);
            throw;
        }
    }
}