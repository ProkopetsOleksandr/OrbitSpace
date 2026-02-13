using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface IActivityRepository
    {
        Task<List<Activity>> GetAllAsync(Guid userId);
        Task<Activity?> GetByIdAsync(Guid id);
        Task<Activity> CreateAsync(Activity activity);
        Task<bool> UpdateAsync(Activity activity);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}
