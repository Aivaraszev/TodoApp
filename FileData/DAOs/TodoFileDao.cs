using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Models;

namespace FileData.DAOs;

public class TodoFileDao : ITodoDao
{
    private readonly FileContext _context;

    public TodoFileDao(FileContext context)
    {
        _context = context;
    }

    public Task<Todo> CreateAsync(Todo todo)
    {
        int id = 1;
        if (_context.Todos.Any())
        {
            id = _context.Todos.Max(t => t.Id);
            id++;
        }

        todo.Id = id;

        _context.Todos.Add(todo);
        _context.SaveChanges();

        return Task.FromResult(todo);
    }

    public Task<IEnumerable<Todo>> GetAsync(SearchTodoParametersDto searchParameters)
    {
        IEnumerable<Todo> todos = _context.Todos.AsEnumerable();
        if (!string.IsNullOrEmpty(searchParameters.Username))
        {
            todos = _context.Todos.Where(t =>
                t.Owner.UserName.Contains(searchParameters.Username, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(searchParameters.TitleContains))
        {
            todos = todos.Where(t =>
                t.Title.Contains(searchParameters.TitleContains, StringComparison.OrdinalIgnoreCase));
        }

        if (searchParameters.UserId != null)
        {
            todos = todos.Where(t =>
                t.Owner.Id == searchParameters.UserId);
        }

        if (searchParameters.CompletedStatus != null)
        {
            todos = todos.Where(t =>
                t.IsCompleted == searchParameters.CompletedStatus
            );
        }

        return Task.FromResult(todos);
    }

    public Task UpdateAsync(Todo todo)
    {
        Todo? existing = _context.Todos.FirstOrDefault(t => t.Id == todo.Id);

        _context.Todos.Remove(existing ?? throw new Exception($"Todo with id {todo.Id} does not exist"));

        _context.Todos.Add(todo);
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    public Task<Todo?> GetByIdAsync(int id)
    {
        Todo? existing = _context.Todos.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(existing);
    }

    public Task DeleteAsync(int id)
    {
        Todo? existing = _context.Todos.FirstOrDefault(t => t.Id == id);
        _context.Todos.Remove(existing ?? throw new Exception($"Todo with id {id} does not exist"));
        _context.SaveChanges();
        return Task.CompletedTask;
    }
}