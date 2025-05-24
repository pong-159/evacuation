using EvacuationAPI.Models;

namespace EvacuationAPI.DTOs;

public record EvacuationStatusDTO(
    string ZoneId,
    LocationCoordinate LocationCoordinates,
    long NumberOfPeople,
    long TotalEvacuatedPeople,
    long RemainingPeople,
    int UrgencyLevel);
