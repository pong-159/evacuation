using EvacuationAPI.Models;

namespace EvacuationAPI.DTOs;

public record EvacuationZonesDTO(
    string ZoneId,
    LocationCoordinate LocationCoordinates,
    long NumberOfPeople,
    int UrgencyLevel);
