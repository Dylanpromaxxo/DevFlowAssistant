namespace DevFlowAssistant.Application.Models;

public sealed record UpdateWorkflowRequest(int Id, string Name, string? Description, bool IsActive);
