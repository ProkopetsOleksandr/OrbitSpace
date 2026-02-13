using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.Goal;

namespace OrbitSpace.Application.Services.Interfaces
{
    public interface IGoalService
    {
        Task<List<GoalDto>> GetAllAsync(Guid userId);
        Task<GoalDto?> GetGoalDetailsAsync(Guid id, Guid userId);
        Task<OperationResult<GoalDto>> CreateAsync(CreateGoalRequest request, Guid userId);
        Task<OperationResult<GoalDto>> UpdateAsync(UpdateGoalRequest request, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}
