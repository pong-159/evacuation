using EvacuationAPI.DTOs;
using EvacuationAPI.Models;

namespace EvacuationAPI;

public interface IEvacuationService
{
    Task<bool> addEvacuationZone(EvacuationZonesDTO evacuationZonesDto);
    Task<IEnumerable<EvacuationStatusDTO>> ListEvacuationZone();

    Task clearZonesAsync();

    EvacuationStatusDTO ConvertZoneToEvacuationStatusDto(EvacuationZones evacuationZones);
}