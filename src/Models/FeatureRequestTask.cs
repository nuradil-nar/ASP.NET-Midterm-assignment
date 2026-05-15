namespace TaskTrackerAPI.Models;

public class FeatureRequestTask : BaseTask
{
    public double EstimatedHours { get; set; }

    public FeatureRequestTask() : base() { }

    public FeatureRequestTask(Guid id, DateTime createdAt) : base(id, createdAt) { }
}
