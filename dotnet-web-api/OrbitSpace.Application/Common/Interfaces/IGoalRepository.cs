using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface IGoalRepository
    {
        Task<List<Goal>> GetAllAsync(string userId);
    }
}
