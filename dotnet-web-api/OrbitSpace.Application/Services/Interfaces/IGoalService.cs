using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.Goal;

namespace OrbitSpace.Application.Services.Interfaces
{
    public interface IGoalService
    {
        Task<List<GoalDto>> GetAllAsync(string userId);
        Task<GoalDto?> GetGoalDetailsAsync(string id, string userId);
        Task<OperationResult<GoalDto>> CreateAsync(CreateGoalRequest request, string userId);
        Task<OperationResult<GoalDto>> UpdateAsync(UpdateGoalRequest request, string userId);
        Task<bool> DeleteAsync(string id, string userId);
    }
}
