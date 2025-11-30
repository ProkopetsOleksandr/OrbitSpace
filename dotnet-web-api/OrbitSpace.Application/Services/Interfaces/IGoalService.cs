using OrbitSpace.Application.Dtos.Goal;

namespace OrbitSpace.Application.Services.Interfaces
{
    public interface IGoalService
    {
        Task<List<GoalDto>> GetAllAsync(string userId);
    }
}
