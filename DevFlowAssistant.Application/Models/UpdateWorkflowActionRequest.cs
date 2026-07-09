namespace DevFlowAssistant.Application.Models;

public sealed record UpdateWorkflowActionRequest(
    int Id,
    int WorkflowId,
    string Name,
    string ActionType,
    string Value,
    string? Arguments,
    string? WorkingDirectory,
    int SortOrder,
    bool IsEnabled,
    int TimeoutSeconds,
    bool ContinueOnError,
    DateTime CreatedAt);
