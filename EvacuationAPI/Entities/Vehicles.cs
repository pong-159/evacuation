using System.ComponentModel.DataAnnotations;
using EvacuationAPI.DTOs;

namespace EvacuationAPI.Models;

public class Vehicles
{
    public Vehicles()
    {
    }

    public Vehicles(string id, int capacity, string type, LocationCoordinate location, double speed)
    {
        Id = id;
        Capacity = capacity;
        Type = type;
        Latitude = location.Latitude;
        Longitude = location.Longitude;
        Speed = speed;
        IsAvailable = true;
    }

    public Vehicles(VehicleDto vehicleDto)
    {
        Id = vehicleDto.Id;
        Capacity = vehicleDto.Capacity;
        Type = vehicleDto.Type;
        Latitude = vehicleDto.LocationCoordinates.Latitude;
        Longitude = vehicleDto.LocationCoordinates.Longitude;
        Speed = vehicleDto.Speed;
        IsAvailable = true;
    }

    [Key] public string Id { get; set; }

    public int Capacity { get; set; }

    public string Type { get; set; }

    public double Latitude { get; set; }
    
    public double Longitude { get; set; }

    public double Speed { get; set; }
    
    public bool IsAvailable { get; set; }
}