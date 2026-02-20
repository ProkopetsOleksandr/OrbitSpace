using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface IActivityRepository
    {
        Task<Activity?> FindByIdAsync(Guid id, Guid userId);
        Task<List<Activity>> GetAllAsync(Guid userId);
        void Add(Activity activity);
        void Update(Activity activity);
        Task<int> DeleteAsync(Guid id, Guid userId);
    }
}
