using DevFlowAssistant.Domain.Entities;

namespace DevFlowAssistant.Domain;

public partial class Workflow
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ExecutionLog> ExecutionLogs { get; set; } = new List<ExecutionLog>();

    public virtual ICollection<WorkflowAction> WorkflowActions { get; set; } = new List<WorkflowAction>();
}
