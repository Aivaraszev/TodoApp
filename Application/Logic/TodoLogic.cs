using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Models;

namespace Application.Logic;

public class TodoLogic : ITodoLogic
{
    private readonly ITodoDao _todoDao;
    private readonly IUserDao _userDao;

    public TodoLogic(ITodoDao todoDao, IUserDao userDao)
    {
        _todoDao = todoDao;
        _userDao = userDao;
    }

    public async Task<Todo> CreateAsync(TodoCreationDto dto)
    {
        User? user = await _userDao.getByIdAsync(dto.OwnerId);
        if (user == null)
        {
            throw new Exception($"User with id {dto.OwnerId} was not found.");
        }

        Todo todo = new Todo(user, dto.Title);
        ValidateTodo(todo);
        Todo created = await _todoDao.CreateAsync(todo);
        return created;
    }

    public Task<IEnumerable<Todo>> GetAsync(SearchTodoParametersDto searchParameters)
    {
        return _todoDao.GetAsync(searchParameters);
    }

    public async Task UpdateAsync(TodoUpdateDto dto)
    {
        Todo? existing = await _todoDao.GetByIdAsync(dto.Id);
        if (existing == null)
        {
            throw new Exception($"Todo with id {dto.Id} was not found.");
        }

        User? user = null;
        if (dto.OwnerId != null)
        {
            user = await _userDao.getByIdAsync((int) dto.OwnerId);
            if (user == null)
            {
                throw new Exception($"User with id {dto.OwnerId} was not found.");
            }
        }

        if (dto.IsCompleted != null && existing.IsCompleted && !(bool) dto.IsCompleted)
        {
            throw new Exception("Todo cannot be un-completed");
        }

        User userToUse = user ?? existing.Owner;
        string titleToUse = dto.Title ?? existing.Title;
        bool completedToUse = dto.IsCompleted ?? existing.IsCompleted;

        Todo updated = new(userToUse, titleToUse)
        {
            IsCompleted = completedToUse,
            Id = existing.Id,
        };

        ValidateTodo(updated);

        await _todoDao.UpdateAsync(updated);
    }

    public async Task DeleteAsync(int id)
    {
        Todo? todo = await _todoDao.GetByIdAsync(id);
        if (todo == null)
        {
            throw new Exception($"Todo with id {id} was not found");
        }

        if (!todo.IsCompleted)
        {
            throw new Exception($"Cannot delete not completed todo");
        }

        await _todoDao.DeleteAsync(id);
    }

    public async Task<Todo> GetByIdAsync(int id)
    {
        return await _todoDao.GetByIdAsync(id) ?? throw new Exception($"Todo with id {id} was not found");
    }

    private void ValidateTodo(Todo dto)
    {
        if (string.IsNullOrEmpty(dto.Title)) throw new Exception("Title cannot be empty.");
    }
}