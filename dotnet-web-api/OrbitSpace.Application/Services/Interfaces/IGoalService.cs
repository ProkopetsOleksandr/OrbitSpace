using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.Goal;

namespace OrbitSpace.Application.Services.Interfaces
{
    public interface IGoalService
    {
        Task<OperationResult<GoalDto>> GetByIdAsync(Guid id, Guid userId);
        Task<List<GoalDto>> GetAllAsync(Guid userId);
        Task<OperationResult<GoalDto>> CreateAsync(CreateGoalRequest request, Guid userId);
        Task<OperationResult<GoalDto>> UpdateAsync(UpdateGoalRequest request, Guid userId);
        Task<OperationResult> DeleteAsync(Guid id, Guid userId);
    }
}
