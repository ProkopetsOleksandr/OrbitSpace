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
        public async Task<List<GoalDto>> GetAllAsync(string userId)
        {
            var data = await goalRepository.GetAllAsync(userId);
            return mapper.Map<List<GoalDto>>(data);
        }

        public async Task<GoalDto?> GetGoalDetailsAsync(string id, string userId)
        {
            var item = await GetByIdForUserAsync(id, userId);
            
            return item == null ? null : mapper.Map<GoalDto>(item);
        }

        public async Task<OperationResult<GoalDto>> CreateAsync(CreateGoalRequest request, string userId)
        {
            if (request is { IsActive: true, DueDate: null })
            {
                return OperationResultError.Validation("Due date is required for active goals");
            }

            var currentDateTime = DateTime.UtcNow;
            var newGoal = new Goal
            {
                UserId = userId,
                Title = request.Title,
                LifeArea = request.LifeArea,
                Status = request.IsActive ? GoalStatus.Active : GoalStatus.NotStarted,
                CreatedAt = currentDateTime,
                UpdatedAt = currentDateTime,
                StartDate = request.IsActive ? currentDateTime : null,
                DueDate = request.DueDate
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

        public async Task<OperationResult<GoalDto>> UpdateAsync(UpdateGoalRequest request, string userId)
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
            entityInDb.DueDate = request.DueDate;
            entityInDb.IsSmartGoal = request.IsSmartGoal;
            entityInDb.Description = request.Description;
            entityInDb.Metrics = request.Metrics;
            entityInDb.AchievabilityRationale =  request.AchievabilityRationale;
            entityInDb.Motivation = request.Motivation;
            
            var currentDateTime = DateTime.UtcNow;
            if (request.Status == GoalStatus.Completed)
            {
                entityInDb.CompletedDate = currentDateTime;
            }

            if (entityInDb.Status != GoalStatus.Active
                && request.Status == GoalStatus.Active)
            {
                entityInDb.StartDate = currentDateTime;
            }

            if (entityInDb.Status != GoalStatus.Cancelled
                && request.Status == GoalStatus.Cancelled)
            {
                entityInDb.CancelledDate = currentDateTime;
            }

            await goalRepository.UpdateAsync(entityInDb);
            
            return mapper.Map<GoalDto>(entityInDb);
        }

        public async Task<bool> DeleteAsync(string id, string userId)
        {
            return await goalRepository.DeleteAsync(id, userId);
        }

        private async Task<Goal?> GetByIdForUserAsync(string id, string userId)
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
