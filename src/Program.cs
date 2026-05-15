using TaskTrackerAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ── Register services (Dependency Injection) ──────────────────────────────────

// Singleton: same in-memory store across all requests
builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "Task Tracker API",
        Version     = "v1",
        Description = "Midterm Assignment – Task Management Microservice"
    });
});

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────────

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
