namespace DevFlowAssistant.Domain;

public partial class ExecutionLog
{
    public int Id { get; set; }

    public int WorkflowId { get; set; }

    public int? WorkflowActionId { get; set; }

    public string? ActionName { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? FinishedAt { get; set; }

    public string Status { get; set; } = null!;

    public string? Message { get; set; }

    public string? ErrorDetails { get; set; }

    public string? StandardOutput { get; set; }

    public string? StandardError { get; set; }

    public int? DurationMs { get; set; }

    public virtual Workflow Workflow { get; set; } = null!;
}
