using DevFlowAssistant.Domain.Entities;

namespace DevFlowAssistant.Application.Interfaces;

public interface IWorkflowActionRepository
{
    Task<List<WorkflowAction>> GetByWorkflowIdAsync(int workflowId);
    Task<WorkflowAction?> GetByIdAsync(int id);
    Task AddAsync(WorkflowAction action);
    Task UpdateAsync(WorkflowAction action);
    Task DeleteAsync(int actionId);
}
