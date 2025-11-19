using Mapster;
using OrbitSpace.Application.Dtos.TodoItem;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Mapping
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<TodoItem, TodoItemDto>().MapWith(m => new TodoItemDto(m.Id, m.Title, m.CreatedAt, m.UpdatedAt, m.Status));
        }
    }
}
