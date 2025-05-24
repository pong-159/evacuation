using EvacuationAPI.DTOs;
using EvacuationAPI.Models;

namespace EvacuationAPI;

public interface IEvacuationPlanService
{
    Task createPlanAsync();
    Task<EvacuationPlan> updatePlanAsync(string vehicleId, int numberOfPeople);
    Task<IEnumerable<EvacuationPlansDTO>> listPlanAsync();
    Task clearAllAsync();
}