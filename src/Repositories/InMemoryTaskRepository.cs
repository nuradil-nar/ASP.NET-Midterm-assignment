using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.Repositories;

/// <summary>
/// In-memory fake repository for simplicity (no database required).
/// Seeded with sample tasks so the API works out of the box.
/// </summary>
public class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<BaseTask> _tasks = new();

    public InMemoryTaskRepository()
    {
        SeedData();
    }

    public IEnumerable<BaseTask> GetAll() => _tasks.AsReadOnly();

    public BaseTask? GetById(Guid id) =>
        _tasks.FirstOrDefault(t => t.Id == id);

    public void Add(BaseTask task) => _tasks.Add(task);

    public void Update(BaseTask task)
    {
        // Entity is a reference type; mutations (CompleteTask) are reflected automatically.
        // This method exists to satisfy the interface contract and could persist to a DB later.
    }

    // ── Seed ──────────────────────────────────────────────────────────────────

    private void SeedData()
    {
        _tasks.AddRange(new BaseTask[]
        {
            new BugReportTask
            {
                Title   = "Login page crashes on Safari",
                SeverityLevel = SeverityLevel.High
            },
            new BugReportTask
            {
                Title   = "Typo in footer text",
                SeverityLevel = SeverityLevel.Low
            },
            new FeatureRequestTask
            {
                Title          = "Dark mode support",
                EstimatedHours = 16
            },
            new FeatureRequestTask
            {
                Title          = "Export to CSV",
                EstimatedHours = 8
            }
        });
    }
}
