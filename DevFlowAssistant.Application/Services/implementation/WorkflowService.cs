using DevFlowAssistant.Application.Interfaces;
using DevFlowAssistant.Application.Models;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Domain;

namespace DevFlowAssistant.Application.Services.implementation;

public class WorkflowService : IWorkflowService
{
    private readonly IWorkflowRepository _workflowRepository;

    public WorkflowService(IWorkflowRepository workflowRepository)
    {
        _workflowRepository = workflowRepository;
    }

    public Task<List<Workflow>> GetAllAsync()
    {
        return _workflowRepository.GetAllAsync();
    }

    public Task<Workflow?> GetByIdAsync(int id)
    {
        return _workflowRepository.GetByIdAsync(id);
    }

    public Task<Workflow?> GetByIdWithActionsAsync(int id)
    {
        return _workflowRepository.GetByIdWithActionsAsync(id);
    }

    public async Task<Workflow> CreateAsync(CreateWorkflowRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("El nombre del workflow es obligatorio.");
        }

        var workflow = new Workflow
        {
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _workflowRepository.AddAsync(workflow);
        return workflow;
    }

    public async Task UpdateAsync(UpdateWorkflowRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("El nombre del workflow es obligatorio.");
        }

        var workflow = await _workflowRepository.GetByIdAsync(request.Id)
            ?? throw new InvalidOperationException("No se encontró el workflow.");

        workflow.Name = request.Name.Trim();
        workflow.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        workflow.IsActive = request.IsActive;
        workflow.UpdatedAt = DateTime.UtcNow;

        await _workflowRepository.UpdateAsync(workflow);
    }

    public Task DeleteAsync(int id)
    {
        return _workflowRepository.DeleteAsync(id);
    }
}
