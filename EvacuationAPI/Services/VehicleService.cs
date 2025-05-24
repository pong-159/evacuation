using EvacuationAPI.DTOs;
using EvacuationAPI.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EvacuationAPI;


public class VehicleService : IVehicleService
{
    private readonly AppDbContext.AppDbContext _context;

    public VehicleService(AppDbContext.AppDbContext context)
    {
        _context = context;
 
    }

    public async Task<bool> addVehicle(VehicleDto vehicleDto)
    {
        try
        {
            var vehicle = new Vehicles(vehicleDto);
            _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    
    public async Task<IEnumerable<Vehicles>> ListVehicle()
    {
        return await _context.Vehicles.ToListAsync();
        
    }


    
    public async Task clearVehiclesAsync()
    {
        try
        {
            _context.Vehicles.RemoveRange(_context.Vehicles);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.Error("clearVehiclesAsync Error " + ex.Message);
        }
    }
}