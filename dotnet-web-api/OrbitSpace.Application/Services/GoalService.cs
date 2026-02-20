using MapsterMapper;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.Goal;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.Domain.Entities;
using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Services
{
    public class GoalService(IUnitOfWork unitOfWork, IGoalRepository goalRepository, IMapper mapper) : IGoalService
    {
        public async Task<OperationResult<GoalDto>> GetByIdAsync(Guid id, Guid userId)
        {
            var goal = await goalRepository.FindByIdAsync(id, userId);
            
            return goal == null ? OperationResultError.NotFound() : mapper.Map<GoalDto>(goal);
        }
        
        public async Task<List<GoalDto>> GetAllAsync(Guid userId)
        {
            var data = await goalRepository.GetAllAsync(userId);
            return mapper.Map<List<GoalDto>>(data);
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

            goalRepository.Add(newGoal);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<GoalDto>(newGoal);
        }

        public async Task<OperationResult<GoalDto>> UpdateAsync(UpdateGoalRequest request, Guid userId)
        {
            if (request is { Status: GoalStatus.Active,  DueDate: null })
            {
                return OperationResultError.Validation("Due date is required for active goals");
            }

            var goal = await goalRepository.FindByIdAsync(request.Id, userId);
            if (goal == null)
            {
                return OperationResultError.NotFound();
            }

            goal.Title = request.Title;
            goal.LifeArea = request.LifeArea;
            goal.Status = request.Status;
            goal.DueAtUtc = request.DueDate;
            goal.IsSmartGoal = request.IsSmartGoal;
            goal.Description = request.Description;
            goal.Metrics = request.Metrics;
            goal.AchievabilityRationale =  request.AchievabilityRationale;
            goal.Motivation = request.Motivation;

            var now = DateTime.UtcNow;
            if (request.Status == GoalStatus.Completed)
            {
                goal.CompletedAtUtc = now;
            }

            if (goal.Status != GoalStatus.Active
                && request.Status == GoalStatus.Active)
            {
                goal.StartedAtUtc = now;
            }

            if (goal.Status != GoalStatus.Canceled
                && request.Status == GoalStatus.Canceled)
            {
                goal.CanceledAtUtc = now;
            }

            goalRepository.Update(goal);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<GoalDto>(goal);
        }

        public async Task<OperationResult> DeleteAsync(Guid id, Guid userId)
        {
            var affected = await goalRepository.DeleteAsync(id, userId);
            return affected == 0 ? OperationResultError.NotFound() : OperationResult.Success();
        }
    }
}
