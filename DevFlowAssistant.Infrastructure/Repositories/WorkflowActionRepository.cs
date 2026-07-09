using DevFlowAssistant.Application.Interfaces;
using DevFlowAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevFlowAssistant.Infrastructure.Repositories;

public class WorkflowActionRepository : IWorkflowActionRepository
{
    private readonly AppDbContext _context;

    public WorkflowActionRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<WorkflowAction>> GetByWorkflowIdAsync(int workflowId)
    {
        return _context.WorkflowActions
            .AsNoTracking()
            .Where(action => action.WorkflowId == workflowId)
            .OrderBy(action => action.SortOrder)
            .ToListAsync();
    }

    public Task<WorkflowAction?> GetByIdAsync(int id)
    {
        return _context.WorkflowActions
            .FirstOrDefaultAsync(action => action.Id == id);
    }

    public async Task AddAsync(WorkflowAction action)
    {
        _context.WorkflowActions.Add(action);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(WorkflowAction action)
    {
        _context.WorkflowActions.Update(action);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int actionId)
    {
        var action = await _context.WorkflowActions.FindAsync(actionId);

        if (action is null)
        {
            return;
        }

        _context.WorkflowActions.Remove(action);
        await _context.SaveChangesAsync();
    }
}
