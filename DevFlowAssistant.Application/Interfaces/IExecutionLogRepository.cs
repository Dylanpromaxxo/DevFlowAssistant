using DevFlowAssistant.Domain;

namespace DevFlowAssistant.Application.Interfaces;

public interface IExecutionLogRepository
{
    Task AddAsync(ExecutionLog log);
    Task<List<ExecutionLog>> GetRecentAsync(int workflowId, int count = 20);
    Task<List<ExecutionLog>> GetAllRecentAsync(int count = 50);
}
