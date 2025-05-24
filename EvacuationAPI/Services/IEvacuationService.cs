using EvacuationAPI.DTOs;

namespace EvacuationAPI;

public interface IEvacuationService
{
    Task<bool> addEvacuationZone(EvacuationZonesDTO evacuationZonesDto);
    Task<IEnumerable<EvacuationStatusDTO>> ListEvacuationZone();

    Task clearZonesAsync();
}