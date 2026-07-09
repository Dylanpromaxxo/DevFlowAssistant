using DevFlowAssistant.Application.Interfaces;
using DevFlowAssistant.Domain;
using Microsoft.EntityFrameworkCore;

namespace DevFlowAssistant.Infrastructure.Repositories;

public class WorkflowRepository : IWorkflowRepository
{
    private readonly AppDbContext _context;

    public WorkflowRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Workflow workflow)
    {
        _context.Workflows.Add(workflow);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var workflow = await _context.Workflows.FindAsync(id);

        if (workflow is null)
        {
            return;
        }

        _context.Workflows.Remove(workflow);
        await _context.SaveChangesAsync();
    }

    public Task<List<Workflow>> GetAllAsync()
    {
        return _context.Workflows
            .AsNoTracking()
            .OrderByDescending(workflow => workflow.CreatedAt)
            .ToListAsync();
    }

    public Task<Workflow?> GetByIdAsync(int id)
    {
        return _context.Workflows
            .FirstOrDefaultAsync(workflow => workflow.Id == id);
    }

    public Task<Workflow?> GetByIdWithActionsAsync(int id)
    {
        return _context.Workflows
            .AsNoTracking()
            .Include(workflow => workflow.WorkflowActions.OrderBy(action => action.SortOrder))
            .FirstOrDefaultAsync(workflow => workflow.Id == id);
    }

    public async Task UpdateAsync(Workflow workflow)
    {
        _context.Workflows.Update(workflow);
        await _context.SaveChangesAsync();
    }
}
