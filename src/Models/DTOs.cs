namespace TaskTrackerAPI.Models;

// ── Request DTOs ──────────────────────────────────────────────────────────────

public record CreateBugReportRequest(
    string Title,
    SeverityLevel SeverityLevel
);

public record CreateFeatureRequestRequest(
    string Title,
    double EstimatedHours
);

// ── Response DTOs (using C# Records for modern style) ─────────────────────────

public record TaskResponse(
    Guid Id,
    string Title,
    DateTime CreatedAt,
    bool IsCompleted,
    string TaskType,
    SeverityLevel? SeverityLevel,
    double? EstimatedHours
);

public record FilteredTasksResponse(
    IEnumerable<TaskResponse> HighSeverityIncompleteBugs,
    double TotalEstimatedHoursForIncompleteFeatures
);
