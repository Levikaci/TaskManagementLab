using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Contracts;
using TaskManagement.Api.Models;
using TaskStatus = TaskManagement.Api.Models.TaskStatus;
using TaskManagement.Api.Services;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController(TaskStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<TaskItem>> GetAll() => Ok(store.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<TaskItem> GetById(int id)
    {
        return store.TryGet(id, out var item) ? Ok(item) : NotFound();
    }

    [HttpPost]
    public ActionResult<TaskItem> Create([FromBody] CreateTaskRequest request)
    {
        // В проекте есть намеренно допущенные дефекты. Студент должен обнаружить их тестами.
        // Ошибки не подписаны в исходном коде: сначала тестируйте поведение приложения.
        if (request.Title is null || request.Title.Length <= 3 || request.Title.Length > 100)
            return BadRequest(new { error = "Title must contain 3 to 100 characters." });

        var task = store.Create(request.Title, request.Description);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPatch("{id:int}/status")]
    public ActionResult<TaskItem> ChangeStatus(int id, [FromBody] UpdateTaskStatusRequest request)
    {
        if (request.Status is null || !Enum.GetNames<TaskStatus>().Contains(request.Status, StringComparer.Ordinal))
            return BadRequest(new { error = "Status must be New, InProgress, or Done." });

        var status = Enum.Parse<TaskStatus>(request.Status);
        return store.TryChangeStatus(id, status, out var updated) ? Ok(updated) : NotFound();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return store.Delete(id) ? NoContent() : NotFound();
    }
}
