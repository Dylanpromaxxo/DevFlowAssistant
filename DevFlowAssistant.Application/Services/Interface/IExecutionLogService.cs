using DevFlowAssistant.Domain;

namespace DevFlowAssistant.Application.Services.Interface;

public interface IExecutionLogService
{
    Task<List<ExecutionLog>> GetRecentAsync(int workflowId, int count = 20);
    Task<List<ExecutionLog>> GetAllRecentAsync(int count = 50);
}
