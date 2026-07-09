namespace DevFlowAssistant.Domain.Entities;

public partial class WorkflowAction
{
    public int Id { get; set; }

    public int WorkflowId { get; set; }

    public string Name { get; set; } = null!;

    public string ActionType { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string? Arguments { get; set; }

    public string? WorkingDirectory { get; set; }

    public int SortOrder { get; set; }

    public bool IsEnabled { get; set; } = true;

    public int TimeoutSeconds { get; set; } = 120;

    public bool ContinueOnError { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Workflow Workflow { get; set; } = null!;
}
