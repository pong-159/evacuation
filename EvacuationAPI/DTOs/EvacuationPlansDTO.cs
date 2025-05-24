using EvacuationAPI.Models;

namespace EvacuationAPI.DTOs;

public record EvacuationPlansDTO(
    string ZoneID,
    string VehicleID,
    string ETA,
    long NumberOfPeople);
