using MapsterMapper;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.Activity;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Services
{
    public class ActivityService(IActivityRepository activityRepository, IMapper mapper) : IActivityService
    {
        public async Task<List<ActivityDto>> GetAllAsync(Guid userId)
        {
            var data = await activityRepository.GetAllAsync(userId);
            return mapper.Map<List<ActivityDto>>(data);
        }

        public async Task<ActivityDto?> GetByIdAsync(Guid id, Guid userId)
        {
            var item = await GetByIdForUserAsync(id, userId);
            return item == null ? null : mapper.Map<ActivityDto>(item);
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

            var createdItem = await activityRepository.CreateAsync(newActivity);

            return mapper.Map<ActivityDto>(createdItem);
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

            var entityInDb = await GetByIdForUserAsync(request.Id, userId);
            if (entityInDb == null)
            {
                return OperationResultError.NotFound();
            }

            entityInDb.Name = request.Name;
            entityInDb.Code = request.Code;
            entityInDb.UpdatedAtUtc = DateTime.UtcNow;

            await activityRepository.UpdateAsync(entityInDb);

            return mapper.Map<ActivityDto>(entityInDb);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            return await activityRepository.DeleteAsync(id, userId);
        }

        private async Task<Activity?> GetByIdForUserAsync(Guid id, Guid userId)
        {
            var entityInDb = await activityRepository.GetByIdAsync(id);
            if (entityInDb == null || entityInDb.UserId != userId)
            {
                return null;
            }

            return entityInDb;
        }
    }
}
