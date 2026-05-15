namespace TaskTrackerAPI.Models;

// Delegate for the task completion event
public delegate void TaskCompletedEventHandler(BaseTask task);

public abstract class BaseTask
{
    // Block 1: Id and CreatedAt can only be set during object creation (init-only setters)
    public Guid Id { get; init; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool IsCompleted { get; private set; }

    // Event triggered when task is completed
    public event TaskCompletedEventHandler? OnTaskCompleted;

    protected BaseTask()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    protected BaseTask(Guid id, DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Marks the task as completed and fires the OnTaskCompleted event.
    /// </summary>
    public void CompleteTask()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            OnTaskCompleted?.Invoke(this);
        }
    }
}
