using EvacuationAPI.DTOs;
using EvacuationAPI.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EvacuationAPI;

public class EvacuationService
{
    private readonly AppDbContext.AppDbContext _context;

    public EvacuationService(AppDbContext.AppDbContext context)
    {
        _context = context;
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
            
            return await _context.EvacuationZones
                .Select(ez => 
                    new EvacuationStatusDTO(ez.Id, new LocationCoordinate(ez.Latitude, ez.Longitude), ez.NumberOfPeople,ez.NumberOfEvacuatedPeople, ez.NumberOfRemainingPeople() ,ez.UrgencyLevel)).ToListAsync();
        
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