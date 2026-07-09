using DevFlowAssistant.Application.Models;
using DevFlowAssistant.Domain.Entities;

namespace DevFlowAssistant.Application.Services.Interface;

public interface IWorkflowActionService
{
    Task<List<WorkflowAction>> GetByWorkflowIdAsync(int workflowId);
    Task AddAsync(CreateWorkflowActionRequest request);
    Task UpdateAsync(UpdateWorkflowActionRequest request);
    Task DeleteAsync(int actionId);
    Task MoveAsync(int workflowId, int actionId, int direction);
}
