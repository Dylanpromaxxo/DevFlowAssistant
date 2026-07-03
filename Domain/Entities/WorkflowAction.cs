using System;
using System.Collections.Generic;

namespace DevFlowAssistant.Domain.Entities;

public partial class WorkflowAction
{
    public int Id { get; set; }

    public int WorkflowId { get; set; }

    public string ActionType { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string? Arguments { get; set; }

    public string? WorkingDirectory { get; set; }

    public int SortOrder { get; set; }

    public int IsEnabled { get; set; }

    public string CreatedAt { get; set; } = null!;

    public virtual Workflow Workflow { get; set; } = null!;
}
