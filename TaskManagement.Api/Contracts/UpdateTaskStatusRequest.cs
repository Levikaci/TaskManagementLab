namespace TaskManagement.Api.Contracts;

public sealed class UpdateTaskStatusRequest
{
    public string? Status { get; init; }
}
