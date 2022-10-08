using Domain.DTOs;

namespace HttpClients.ClientInterfaces;

public interface ITodoService
{
    Task CreateTodo(TodoCreationDto dto);
}