using DevFlowAssistant.Application.Interfaces;
using DevFlowAssistant.Application.Models;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Domain.Entities;
using DevFlowAssistant.Domain.Enums;

namespace DevFlowAssistant.Application.Services.implementation;

public class WorkflowActionService : IWorkflowActionService
{
    private readonly IWorkflowActionRepository _actionRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public WorkflowActionService(IWorkflowActionRepository actionRepository, IWorkflowRepository workflowRepository)
    {
        _actionRepository = actionRepository;
        _workflowRepository = workflowRepository;
    }

    public Task<List<WorkflowAction>> GetByWorkflowIdAsync(int workflowId)
    {
        return _actionRepository.GetByWorkflowIdAsync(workflowId);
    }

    public async Task AddAsync(CreateWorkflowActionRequest request)
    {
        Validate(request.Name, request.ActionType, request.Value, request.TimeoutSeconds);

        _ = await _workflowRepository.GetByIdAsync(request.WorkflowId)
            ?? throw new InvalidOperationException("No se encontró el workflow seleccionado.");

        var actions = await _actionRepository.GetByWorkflowIdAsync(request.WorkflowId);
        var nextSortOrder = actions.Count == 0 ? 1 : actions.Max(action => action.SortOrder) + 1;

        var action = new WorkflowAction
        {
            WorkflowId = request.WorkflowId,
            Name = request.Name.Trim(),
            ActionType = request.ActionType,
            Value = request.Value.Trim(),
            Arguments = NormalizeOptional(request.Arguments),
            WorkingDirectory = NormalizeOptional(request.WorkingDirectory),
            SortOrder = nextSortOrder,
            IsEnabled = true,
            TimeoutSeconds = request.TimeoutSeconds,
            ContinueOnError = request.ContinueOnError,
            CreatedAt = DateTime.UtcNow
        };

        await _actionRepository.AddAsync(action);
    }

    public async Task UpdateAsync(UpdateWorkflowActionRequest request)
    {
        Validate(request.Name, request.ActionType, request.Value, request.TimeoutSeconds);

        var action = await _actionRepository.GetByIdAsync(request.Id)
            ?? throw new InvalidOperationException("No se encontró la acción.");

        action.Name = request.Name.Trim();
        action.ActionType = request.ActionType;
        action.Value = request.Value.Trim();
        action.Arguments = NormalizeOptional(request.Arguments);
        action.WorkingDirectory = NormalizeOptional(request.WorkingDirectory);
        action.SortOrder = request.SortOrder;
        action.IsEnabled = request.IsEnabled;
        action.TimeoutSeconds = request.TimeoutSeconds;
        action.ContinueOnError = request.ContinueOnError;

        await _actionRepository.UpdateAsync(action);
    }

    public Task DeleteAsync(int actionId)
    {
        return _actionRepository.DeleteAsync(actionId);
    }

    public async Task MoveAsync(int workflowId, int actionId, int direction)
    {
        var actions = await _actionRepository.GetByWorkflowIdAsync(workflowId);
        var orderedActions = actions.OrderBy(action => action.SortOrder).ToList();
        var currentIndex = orderedActions.FindIndex(action => action.Id == actionId);
        var targetIndex = currentIndex + direction;

        if (currentIndex < 0 || targetIndex < 0 || targetIndex >= orderedActions.Count)
        {
            return;
        }

        var current = orderedActions[currentIndex];
        var target = orderedActions[targetIndex];
        (current.SortOrder, target.SortOrder) = (target.SortOrder, current.SortOrder);

        await _actionRepository.UpdateAsync(current);
        await _actionRepository.UpdateAsync(target);
    }

    private static void Validate(string name, string actionType, string value, int timeoutSeconds)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre de la acción es obligatorio.");
        }

        if (!WorkflowActionTypes.All.Contains(actionType))
        {
            throw new ArgumentException("El tipo de acción no es válido.");
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("El valor de la acción es obligatorio.");
        }

        if (timeoutSeconds < 1 || timeoutSeconds > 3600)
        {
            throw new ArgumentException("El timeout debe estar entre 1 y 3600 segundos.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
