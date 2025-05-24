using System.ComponentModel.DataAnnotations;
using EvacuationAPI.DTOs;
using EvacuationAPI.Entities.Enums;

namespace EvacuationAPI.Models;

public class EvacuationZones
{
    public EvacuationZones()
    {
        
    }
    
    public EvacuationZones(string id, LocationCoordinate location, long numberOfPeople, int urgencyLevel)
    {
        Id = id;
        Latitude = location.Latitude;
        Longitude = location.Longitude;
        NumberOfPeople = numberOfPeople;
        UrgencyLevel = urgencyLevel;
        NumberOfEvacuatedPeople = 0;
    }

    public EvacuationZones(EvacuationZonesDTO dto)
    {
        Id = dto.ZoneId;
        Latitude = dto.LocationCoordinates.Latitude;
        Longitude = dto.LocationCoordinates.Longitude;
        NumberOfPeople = dto.NumberOfPeople;
        UrgencyLevel = dto.UrgencyLevel;
        NumberOfEvacuatedPeople = 0;
    }

    [Key] 
    public string Id { get; set; }

    public double Latitude { get; set; }
    
    public double Longitude { get; set; }

    public long NumberOfPeople { get; set; }

    public int UrgencyLevel { get; set; }
    
    public long NumberOfEvacuatedPeople { get; set; }
    
    public long NumberOfRemainingPeople()
    {
        return NumberOfPeople - NumberOfEvacuatedPeople;
    }
}