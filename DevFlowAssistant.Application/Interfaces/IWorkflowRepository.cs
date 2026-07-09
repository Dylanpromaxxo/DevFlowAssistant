using DevFlowAssistant.Domain;

namespace DevFlowAssistant.Application.Interfaces;

public interface IWorkflowRepository
{
    Task<List<Workflow>> GetAllAsync();
    Task<Workflow?> GetByIdAsync(int id);
    Task<Workflow?> GetByIdWithActionsAsync(int id);
    Task AddAsync(Workflow workflow);
    Task UpdateAsync(Workflow workflow);
    Task DeleteAsync(int id);
}
