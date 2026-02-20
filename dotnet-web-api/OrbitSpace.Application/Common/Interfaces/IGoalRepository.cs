using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface IGoalRepository
    {
        Task<Goal?> FindByIdAsync(Guid id, Guid userId);
        Task<List<Goal>> GetAllAsync(Guid userId);
        void Add(Goal goal);
        void Update(Goal goal);
        Task<int> DeleteAsync(Guid id, Guid userId);
    }
}
