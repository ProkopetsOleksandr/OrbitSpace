using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface IGoalRepository
    {
        Task<List<Goal>> GetAllAsync(Guid userId);
        Task<Goal?> GetByIdAsync(Guid id);
        Task<Goal> CreateAsync(Goal goal);
        Task<bool> UpdateAsync(Goal goal);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}
