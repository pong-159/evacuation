using EvacuationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EvacuationAPI.AppDbContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<EvacuationZones> EvacuationZones { get; set; }
    
    public DbSet<EvacuationPlan> EvacuationPlans { get; set; }

    public DbSet<Vehicles> Vehicles { get; set; }
}