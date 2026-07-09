using DevFlowAssistant.Application.Models;
using DevFlowAssistant.Domain;

namespace DevFlowAssistant.Application.Services.Interface;

public interface IWorkflowService
{
    Task<List<Workflow>> GetAllAsync();
    Task<Workflow?> GetByIdAsync(int id);
    Task<Workflow?> GetByIdWithActionsAsync(int id);
    Task<Workflow> CreateAsync(CreateWorkflowRequest request);
    Task UpdateAsync(UpdateWorkflowRequest request);
    Task DeleteAsync(int id);
}
