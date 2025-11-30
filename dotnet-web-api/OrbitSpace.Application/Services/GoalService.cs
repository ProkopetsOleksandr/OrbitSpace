using MapsterMapper;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Application.Dtos.Goal;
using OrbitSpace.Application.Services.Interfaces;

namespace OrbitSpace.Application.Services
{
    public class GoalService(IGoalRepository goalRepository, IMapper mapper) : IGoalService
    {
        public async Task<List<GoalDto>> GetAllAsync(string userId)
        {
            var data = await goalRepository.GetAllAsync(userId);
            return mapper.Map<List<GoalDto>>(data);
        }
    }
}
