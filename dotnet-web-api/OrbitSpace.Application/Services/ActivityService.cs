using MapsterMapper;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.Activity;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Services
{
    public class ActivityService(IUnitOfWork unitOfWork, IActivityRepository activityRepository, IMapper mapper) : IActivityService
    {
        public async Task<OperationResult<ActivityDto>> GetByIdAsync(Guid id, Guid userId)
        {
            var activity = await activityRepository.FindByIdAsync(id, userId);
            if (activity == null)
            {
                return OperationResultError.NotFound();
            }
            
            return mapper.Map<ActivityDto>(activity);
        }
        
        public async Task<List<ActivityDto>> GetAllAsync(Guid userId)
        {
            var data = await activityRepository.GetAllAsync(userId);
            return mapper.Map<List<ActivityDto>>(data);
        }
        
        public async Task<OperationResult<ActivityDto>> CreateAsync(CreateActivityRequest request, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return OperationResultError.Validation("Name is required");
            }

            if (string.IsNullOrWhiteSpace(request.Code) || request.Code.Length > 5)
            {
                return OperationResultError.Validation("Code must be between 1 and 5 characters");
            }

            var currentDateTime = DateTime.UtcNow;
            var newActivity = new Activity
            {
                Id = Guid.CreateVersion7(),
                UserId = userId,
                Name = request.Name,
                Code = request.Code,
                CreatedAtUtc = currentDateTime,
                UpdatedAtUtc = currentDateTime
            };

            activityRepository.Add(newActivity);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<ActivityDto>(newActivity);
        }

        public async Task<OperationResult<ActivityDto>> UpdateAsync(UpdateActivityRequest request, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return OperationResultError.Validation("Name is required");
            }

            if (string.IsNullOrWhiteSpace(request.Code) || request.Code.Length > 5)
            {
                return OperationResultError.Validation("Code must be between 1 and 5 characters");
            }

            var activity = await activityRepository.FindByIdAsync(request.Id, userId);
            if (activity == null)
            {
                return OperationResultError.NotFound();
            }

            activity.Name = request.Name;
            activity.Code = request.Code;
            activity.UpdatedAtUtc = DateTime.UtcNow;

            activityRepository.Update(activity);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<ActivityDto>(activity);
        }

        public async Task<OperationResult> DeleteAsync(Guid id, Guid userId)
        {
            var affected = await activityRepository.DeleteAsync(id, userId);
            return affected == 0 ? OperationResultError.NotFound() : OperationResult.Success();
        }
    }
}
