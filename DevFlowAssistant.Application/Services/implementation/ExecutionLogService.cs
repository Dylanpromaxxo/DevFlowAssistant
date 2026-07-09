using DevFlowAssistant.Application.Interfaces;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Domain;

namespace DevFlowAssistant.Application.Services.implementation;

public class ExecutionLogService : IExecutionLogService
{
    private readonly IExecutionLogRepository _logRepository;

    public ExecutionLogService(IExecutionLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public Task<List<ExecutionLog>> GetRecentAsync(int workflowId, int count = 20)
    {
        return _logRepository.GetRecentAsync(workflowId, count);
    }

    public Task<List<ExecutionLog>> GetAllRecentAsync(int count = 50)
    {
        return _logRepository.GetAllRecentAsync(count);
    }
}
