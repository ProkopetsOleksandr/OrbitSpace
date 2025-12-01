using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface IGoalRepository
    {
        Task<List<Goal>> GetAllAsync(string userId);
        Task<Goal?> GetByIdAsync(string id);
        Task<Goal> CreateAsync(Goal goal);
        Task<bool> UpdateAsync(Goal goal);
        Task<bool> DeleteAsync(string id, string userId);
    }
}
