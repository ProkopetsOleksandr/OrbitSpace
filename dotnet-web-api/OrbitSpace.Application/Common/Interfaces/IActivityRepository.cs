using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface IActivityRepository
    {
        Task<List<Activity>> GetAllAsync(string userId);
        Task<Activity?> GetByIdAsync(string id);
        Task<Activity> CreateAsync(Activity activity);
        Task<bool> UpdateAsync(Activity activity);
        Task<bool> DeleteAsync(string id, string userId);
    }
}
