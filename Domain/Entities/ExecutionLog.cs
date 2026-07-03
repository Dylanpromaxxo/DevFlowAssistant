using System;
using System.Collections.Generic;

namespace DevFlowAssistant.Domain;

public partial class ExecutionLog
{
    public int Id { get; set; }

    public int WorkflowId { get; set; }

    public string StartedAt { get; set; } = null!;

    public string? FinishedAt { get; set; }

    public string Status { get; set; } = null!;

    public string? Message { get; set; }

    public string? ErrorDetails { get; set; }

    public int? DurationMs { get; set; }

    public virtual Workflow Workflow { get; set; } = null!;
}
