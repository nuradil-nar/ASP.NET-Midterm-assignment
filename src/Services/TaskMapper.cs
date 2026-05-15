using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.Services;

public static class TaskMapper
{
    public static TaskResponse ToResponse(BaseTask task) =>
        // Pattern matching on the concrete type (modern C# feature)
        task switch
        {
            BugReportTask bug => new TaskResponse(
                bug.Id,
                bug.Title,
                bug.CreatedAt,
                bug.IsCompleted,
                nameof(BugReportTask),
                bug.SeverityLevel,
                null),

            FeatureRequestTask feat => new TaskResponse(
                feat.Id,
                feat.Title,
                feat.CreatedAt,
                feat.IsCompleted,
                nameof(FeatureRequestTask),
                null,
                feat.EstimatedHours),

            _ => new TaskResponse(
                task.Id,
                task.Title,
                task.CreatedAt,
                task.IsCompleted,
                "Unknown",
                null,
                null)
        };
}
