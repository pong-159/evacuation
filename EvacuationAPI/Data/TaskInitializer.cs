using EvacuationAPI.Models;

namespace EvacuationAPI.AppDbContext;

public static class TaskInitializer
{
    public static WebApplication SeedData(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            try
            {
                context.Database.EnsureCreated();
                
                initVehiclesData(context);
                initEvacuationData(context);
            }catch (Exception ex)
            {
                throw;
            }


        }
        return app;
    }
    
    public static void initVehiclesData(AppDbContext _context)
    {
        if(_context.Vehicles.Any()) return;
        for (int i = 1; i <= 10; i++)
        {
            var vehicle = new Vehicles(
                "v" +  i,
                10 +i,
                "Car",
                new LocationCoordinate(10 * i, 10 * i),
                10 +i);
            _context.Vehicles.Add(vehicle);
        }
        _context.SaveChanges();
        
        
    }
    
    public static void initEvacuationData(AppDbContext _context){
        
        if(_context.EvacuationZones.Any()) return;
        for (int i = 1; i <= 10; i++)
        {
            var zone = new EvacuationZones("z" + i, new LocationCoordinate(12.39 * i, 21.684 * i), 10 * i, (i % 5) + 1 );
            _context.EvacuationZones.Add(zone);
        }
        _context.SaveChanges();
        
    }
}