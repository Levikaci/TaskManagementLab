using System.Collections.Concurrent;
using TaskManagement.Api.Models;
using TaskStatus = TaskManagement.Api.Models.TaskStatus;

namespace TaskManagement.Api.Services;

public sealed class TaskStore
{
    private readonly ConcurrentDictionary<int, TaskItem> _tasks = new();
    private int _nextId;

    public IEnumerable<TaskItem> GetAll() => _tasks.Values.OrderBy(t => t.Id);
    public bool TryGet(int id, out TaskItem? item) => _tasks.TryGetValue(id, out item);

    public TaskItem Create(string title, string? description)
    {
        var item = new TaskItem(Interlocked.Increment(ref _nextId), title, description, TaskStatus.New);
        _tasks[item.Id] = item;
        return item;
    }

    public bool TryChangeStatus(int id, TaskStatus status, out TaskItem? updated)
    {
        updated = null;
        while (_tasks.TryGetValue(id, out var current))
        {
            var next = current with { Status = status };
            if (_tasks.TryUpdate(id, next, current))
            {
                updated = next;
                return true;
            }
        }
        return false;
    }

    public bool Delete(int id) => _tasks.TryRemove(id, out _);
}
