namespace TaskManagement.Api.Models;

public sealed record TaskItem(int Id, string Title, string? Description, TaskStatus Status);
