using MapsterMapper;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.Goal;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.Domain.Entities;
using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Services
{
    public class GoalService(IGoalRepository goalRepository, IMapper mapper) : IGoalService
    {
        public async Task<List<GoalDto>> GetAllAsync(Guid userId)
        {
            var data = await goalRepository.GetAllAsync(userId);
            return mapper.Map<List<GoalDto>>(data);
        }

        public async Task<GoalDto?> GetGoalDetailsAsync(Guid id, Guid userId)
        {
            var item = await GetByIdForUserAsync(id, userId);

            return item == null ? null : mapper.Map<GoalDto>(item);
        }

        public async Task<OperationResult<GoalDto>> CreateAsync(CreateGoalRequest request, Guid userId)
        {
            if (request is { IsActive: true, DueAtUtc: null })
            {
                return OperationResultError.Validation("Due date is required for active goals");
            }

            var currentDateTime = DateTime.UtcNow;
            var newGoal = new Goal
            {
                Id = Guid.CreateVersion7(),
                UserId = userId,
                Title = request.Title,
                LifeArea = request.LifeArea,
                Status = request.IsActive ? GoalStatus.Active : GoalStatus.NotStarted,
                CreatedAtUtc = currentDateTime,
                UpdatedAtUtc = currentDateTime,
                StartedAtUtc = request.IsActive ? currentDateTime : null,
                DueAtUtc = request.DueAtUtc
            };

            if (request.IsSmartGoal)
            {
                newGoal.IsSmartGoal = true;
                newGoal.Description = request.Description;
                newGoal.Metrics = request.Metrics;
                newGoal.AchievabilityRationale = request.AchievabilityRationale;
                newGoal.Motivation = request.Motivation;
            }

            var createdItem = await goalRepository.CreateAsync(newGoal);

            return mapper.Map<GoalDto>(createdItem);
        }

        public async Task<OperationResult<GoalDto>> UpdateAsync(UpdateGoalRequest request, Guid userId)
        {
            if (request is { Status: GoalStatus.Active,  DueDate: null })
            {
                return OperationResultError.Validation("Due date is required for active goals");
            }

            var entityInDb = await GetByIdForUserAsync(request.Id, userId);
            if (entityInDb == null)
            {
                return OperationResultError.NotFound();
            }

            entityInDb.Title = request.Title;
            entityInDb.LifeArea = request.LifeArea;
            entityInDb.Status = request.Status;
            entityInDb.DueAtUtc = request.DueDate;
            entityInDb.IsSmartGoal = request.IsSmartGoal;
            entityInDb.Description = request.Description;
            entityInDb.Metrics = request.Metrics;
            entityInDb.AchievabilityRationale =  request.AchievabilityRationale;
            entityInDb.Motivation = request.Motivation;

            var currentDateTime = DateTime.UtcNow;
            if (request.Status == GoalStatus.Completed)
            {
                entityInDb.CompletedAtUtc = currentDateTime;
            }

            if (entityInDb.Status != GoalStatus.Active
                && request.Status == GoalStatus.Active)
            {
                entityInDb.StartedAtUtc = currentDateTime;
            }

            if (entityInDb.Status != GoalStatus.Cancelled
                && request.Status == GoalStatus.Cancelled)
            {
                entityInDb.CancelledAtUtc = currentDateTime;
            }

            await goalRepository.UpdateAsync(entityInDb);

            return mapper.Map<GoalDto>(entityInDb);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            return await goalRepository.DeleteAsync(id, userId);
        }

        private async Task<Goal?> GetByIdForUserAsync(Guid id, Guid userId)
        {
            var entityInDb = await goalRepository.GetByIdAsync(id);
            if (entityInDb == null || entityInDb.UserId != userId)
            {
                return null;
            }

            return entityInDb;
        }
    }
}
