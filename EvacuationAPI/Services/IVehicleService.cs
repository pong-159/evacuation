using EvacuationAPI.DTOs;
using EvacuationAPI.Models;

namespace EvacuationAPI;

public interface IVehicleService
{
    Task<bool> addVehicle(VehicleDto vehicleDto);
    Task<IEnumerable<Vehicles>> ListVehicle();
    void initData();
    Task clearVehiclesAsync();
}
