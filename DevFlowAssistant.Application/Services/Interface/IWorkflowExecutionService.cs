using DevFlowAssistant.Domain;

namespace DevFlowAssistant.Application.Services.Interface;

public interface IWorkflowExecutionService
{
    Task<IReadOnlyList<ExecutionLog>> ExecuteAsync(int workflowId, CancellationToken cancellationToken = default);
}
