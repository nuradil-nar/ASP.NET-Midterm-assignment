using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.Services;

/// <summary>
/// Block 1 – Static filter service using LINQ.
/// </summary>
public static class TaskFilterService
{
    /// <summary>
    /// Returns all incomplete BugReportTask items with High (or Critical) severity,
    /// sorted by CreatedAt descending (newest first).
    /// </summary>
    public static IEnumerable<BugReportTask> GetHighSeverityIncompleteBugs(
        IEnumerable<BaseTask> tasks) =>
        tasks
            .OfType<BugReportTask>()
            .Where(t => !t.IsCompleted &&
                        t.SeverityLevel is SeverityLevel.High or SeverityLevel.Critical)
            .OrderByDescending(t => t.CreatedAt);

    /// <summary>
    /// Returns the total sum of EstimatedHours for all incomplete FeatureRequestTask items.
    /// </summary>
    public static double GetTotalEstimatedHoursForIncompleteFeatures(
        IEnumerable<BaseTask> tasks) =>
        tasks
            .OfType<FeatureRequestTask>()
            .Where(t => !t.IsCompleted)
            .Sum(t => t.EstimatedHours);
}
