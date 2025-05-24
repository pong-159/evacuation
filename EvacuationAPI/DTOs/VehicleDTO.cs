using EvacuationAPI.Models;

namespace EvacuationAPI.DTOs;

public record VehicleDto(string Id, LocationCoordinate LocationCoordinates, int Capacity, string Type, double Speed);