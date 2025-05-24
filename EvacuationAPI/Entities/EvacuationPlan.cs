using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EvacuationAPI.Entities.Enums;

namespace EvacuationAPI.Models;

public class EvacuationPlan
{
    private static readonly string ETA_UNIT = "minutes";

    public EvacuationPlan()
    {
    }

    public EvacuationPlan(string zoneId, string vehicleId, long numberOfPeople, double ETA)
    {
        Id = Guid.NewGuid().ToString();
        ZoneId = zoneId;
        VehicleId = vehicleId;
        NumberOfPeople = numberOfPeople;
        this.ETA = ETA;
        EvacuationStatus = EvacuationStatusEnum.InProgress;
    }

    public EvacuationPlan(string zoneId, string vehicleId, long numberOfPeople)
    {
        Id = Guid.NewGuid().ToString();    
        ZoneId = zoneId;
        VehicleId = vehicleId;
        NumberOfPeople = numberOfPeople;
        EvacuationStatus = EvacuationStatusEnum.InProgress;
    }

    [Key]
    public string Id { get; set; }
    public string ZoneId { get; set; }

    public string VehicleId { get; set; }

    public double ETA { get; set; }

    public long NumberOfPeople { get; set; }
    
    public EvacuationStatusEnum EvacuationStatus { get; set; } 
    
    public string getETAWithUnit()
    {
        return $"{ETA} {ETA_UNIT}";
    }
}