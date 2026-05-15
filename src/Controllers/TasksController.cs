using Microsoft.AspNetCore.Mvc;
using TaskTrackerAPI.Models;
using TaskTrackerAPI.Repositories;
using TaskTrackerAPI.Services;

namespace TaskTrackerAPI.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    // Block 2 – DI: ITaskRepository injected via constructor
    private readonly ITaskRepository _repository;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskRepository repository, ILogger<TasksController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    // ── GET /api/tasks ────────────────────────────────────────────────────────

    /// <summary>Retrieve all tasks.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var tasks = _repository.GetAll().Select(TaskMapper.ToResponse);
        return Ok(tasks);
    }

    // ── GET /api/tasks/filtered ───────────────────────────────────────────────

    /// <summary>
    /// Returns high-severity incomplete bugs and total estimated hours for
    /// incomplete features (Block 1 LINQ queries exposed via API).
    /// </summary>
    [HttpGet("filtered")]
    [ProducesResponseType(typeof(FilteredTasksResponse), StatusCodes.Status200OK)]
    public IActionResult GetFiltered()
    {
        var allTasks = _repository.GetAll();

        var highSeverityBugs = TaskFilterService
            .GetHighSeverityIncompleteBugs(allTasks)
            .Select(TaskMapper.ToResponse);

        var totalHours = TaskFilterService
            .GetTotalEstimatedHoursForIncompleteFeatures(allTasks);

        return Ok(new FilteredTasksResponse(highSeverityBugs, totalHours));
    }

    // ── POST /api/tasks/bug ───────────────────────────────────────────────────

    /// <summary>Create a new bug-report task.</summary>
    [HttpPost("bug")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreateBug([FromBody] CreateBugReportRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("Title is required.");

        var bug = new BugReportTask
        {
            Title         = request.Title,
            SeverityLevel = request.SeverityLevel
        };

        _repository.Add(bug);
        _logger.LogInformation("Bug report created: {Id} – {Title}", bug.Id, bug.Title);

        return CreatedAtAction(nameof(GetById), new { id = bug.Id }, TaskMapper.ToResponse(bug));
    }

    // ── POST /api/tasks/feature ───────────────────────────────────────────────

    /// <summary>Create a new feature-request task.</summary>
    [HttpPost("feature")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CreateFeature([FromBody] CreateFeatureRequestRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("Title is required.");

        var feature = new FeatureRequestTask
        {
            Title          = request.Title,
            EstimatedHours = request.EstimatedHours
        };

        _repository.Add(feature);
        _logger.LogInformation("Feature request created: {Id} – {Title}", feature.Id, feature.Title);

        return CreatedAtAction(nameof(GetById), new { id = feature.Id }, TaskMapper.ToResponse(feature));
    }

    // ── PUT /api/tasks/{id}/complete ──────────────────────────────────────────

    /// <summary>
    /// Mark a task as completed. Triggers the OnTaskCompleted event internally.
    /// </summary>
    [HttpPut("{id:guid}/complete")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult CompleteTask(Guid id)
    {
        var task = _repository.GetById(id);
        if (task is null)
            return NotFound($"Task {id} not found.");

        if (task.IsCompleted)
            return Conflict($"Task {id} is already completed.");

        // Subscribe to the event (demonstrates event wiring)
        task.OnTaskCompleted += OnTaskCompletedHandler;

        task.CompleteTask();   // fires the event

        _repository.Update(task);

        return Ok(TaskMapper.ToResponse(task));
    }

    // ── GET /api/tasks/{id} ───────────────────────────────────────────────────

    /// <summary>Retrieve a single task by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var task = _repository.GetById(id);
        return task is null
            ? NotFound($"Task {id} not found.")
            : Ok(TaskMapper.ToResponse(task));
    }

    // ── Private event handler ─────────────────────────────────────────────────

    private void OnTaskCompletedHandler(BaseTask task)
    {
        // In a real system this would publish to a message broker (see Block 3 / ARCHITECTURE.md).
        _logger.LogInformation(
            "[EVENT] OnTaskCompleted fired for task {Id} ('{Title}') at {Time}",
            task.Id, task.Title, DateTime.UtcNow);
    }
}
