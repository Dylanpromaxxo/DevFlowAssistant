namespace DevFlowAssistant.Application.Models;

public sealed record CreateWorkflowActionRequest(
    int WorkflowId,
    string Name,
    string ActionType,
    string Value,
    string? Arguments,
    string? WorkingDirectory,
    int TimeoutSeconds,
    bool ContinueOnError);
