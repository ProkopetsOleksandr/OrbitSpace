using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.Activity;

namespace OrbitSpace.Application.Services.Interfaces
{
    public interface IActivityService
    {
        Task<List<ActivityDto>> GetAllAsync(Guid userId);
        Task<ActivityDto?> GetByIdAsync(Guid id, Guid userId);
        Task<OperationResult<ActivityDto>> CreateAsync(CreateActivityRequest request, Guid userId);
        Task<OperationResult<ActivityDto>> UpdateAsync(UpdateActivityRequest request, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}
