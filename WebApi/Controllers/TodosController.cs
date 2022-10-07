using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoLogic _todoLogic;

    public TodosController(ITodoLogic todoLogic)
    {
        _todoLogic = todoLogic;
    }

    [HttpPost]
    public async Task<ActionResult<Todo>> CreateAsync(TodoCreationDto dto)
    {
        try
        {
            Todo todo = await _todoLogic.CreateAsync(dto);
            return Created($"/todos/{todo.Id}", todo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetAsync([FromQuery] string? userName, [FromQuery] int? userId,
        [FromQuery] bool? completedStatus, [FromQuery] string? titleContains)
    {
        try
        {
            SearchTodoParametersDto parameters = new(userName, userId, completedStatus, titleContains);
            var todos = await _todoLogic.GetAsync(parameters);
            return Ok(todos);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPatch]
    public async Task<ActionResult> UpdateTodo([FromBody] TodoUpdateDto dto)
    {
        try
        {
            await _todoLogic.UpdateAsync(dto);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id)
    {
        try
        {
            await _todoLogic.DeleteAsync(id);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Todo>> GetById([FromRoute] int id)
    {
        try
        {
            Todo todo = await _todoLogic.GetByIdAsync(id);
            return Ok(todo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}