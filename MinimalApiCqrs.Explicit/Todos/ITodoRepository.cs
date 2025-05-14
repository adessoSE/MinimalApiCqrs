namespace MinimalApiCqrs.Simple.Todos;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ITodoRepository
{
    Task<IEnumerable<Todo>> GetTodosAsync(CancellationToken cancellationToken);
    Task<Todo?> GetTodoByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddTodoAsync(Todo todo, CancellationToken cancellationToken);
    Task UpdateTodoAsync(Todo todo, CancellationToken cancellationToken);
    Task DeleteTodoAsync(Guid id, CancellationToken cancellationToken);
}
