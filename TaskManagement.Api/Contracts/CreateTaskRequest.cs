namespace TaskManagement.Api.Contracts;

public sealed class CreateTaskRequest
{
    public string? Title { get; init; }
    public string? Description { get; init; }
}
