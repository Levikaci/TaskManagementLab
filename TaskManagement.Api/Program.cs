using System.Text.Json.Serialization;
using TaskManagement.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TaskStore>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger открыт и вне Development, чтобы лабораторную было легко выполнить локально.
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();

// Позволяет запускать API непосредственно из xUnit через WebApplicationFactory.
public partial class Program { }
