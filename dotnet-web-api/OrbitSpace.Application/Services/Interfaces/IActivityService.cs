using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.Activity;

namespace OrbitSpace.Application.Services.Interfaces
{
    public interface IActivityService
    {
        Task<List<ActivityDto>> GetAllAsync(string userId);
        Task<ActivityDto?> GetByIdAsync(string id, string userId);
        Task<OperationResult<ActivityDto>> CreateAsync(CreateActivityRequest request, string userId);
        Task<OperationResult<ActivityDto>> UpdateAsync(UpdateActivityRequest request, string userId);
        Task<bool> DeleteAsync(string id, string userId);
    }
}
