using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TaskManagement.Tests;

/// <summary>
/// Стартовый образец: каждый тест получает собственный API и чистое хранилище.
/// Добавлены тесты по тест-кейсам TC-01, TC-03, TC-05, TC-06, TC-12, TC-15, TC-17, TC-18.
/// </summary>
public sealed class TaskApiTests
{
    [Fact]
    public async Task GetAll_WhenNoTasks_ReturnsEmptyList()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(JsonValueKind.Array, document.RootElement.ValueKind);
        Assert.Equal(0, document.RootElement.GetArrayLength());
    }

    [Fact]
    public async Task CreateTask_WithValidTitle_ReturnsCreated()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/tasks",
            new { title = "Отчёт", description = "Практика" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("New", document.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateTask_WithTitleOfExactlyThreeChars_ReturnsCreated()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync("/api/tasks", new { title = "abc" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateTask_WithTitleOf101Chars_ReturnsBadRequest()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync("/api/tasks", new { title = new string('a', 101) });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTask_WithWhitespaceOnlyTitle_ReturnsBadRequest()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.PostAsJsonAsync("/api/tasks", new { title = "     " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();

        var response = await client.GetAsync("/api/tasks/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ChangeStatus_WithUnknownStatus_ReturnsBadRequest()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();
        var id = await CreateTaskAsync(client, "Задача");

        var response = await client.PatchAsJsonAsync($"/api/tasks/{id}/status", new { status = "Unknown" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangeStatus_FromDoneToNew_ReturnsConflict()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();
        var id = await CreateTaskAsync(client, "Задача");
        await client.PatchAsJsonAsync($"/api/tasks/{id}/status", new { status = "Done" });

        var response = await client.PatchAsJsonAsync($"/api/tasks/{id}/status", new { status = "New" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Delete_TaskWithDoneStatus_ReturnsConflict()
    {
        using var app = new WebApplicationFactory<Program>();
        using var client = app.CreateClient();
        var id = await CreateTaskAsync(client, "Задача");
        await client.PatchAsJsonAsync($"/api/tasks/{id}/status", new { status = "Done" });

        var response = await client.DeleteAsync($"/api/tasks/{id}");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    private static async Task<int> CreateTaskAsync(HttpClient client, string title)
    {
        var response = await client.PostAsJsonAsync("/api/tasks", new { title });
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.GetProperty("id").GetInt32();
    }

}
