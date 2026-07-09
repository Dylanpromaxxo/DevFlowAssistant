using DevFlowAssistant.Application.Interfaces;
using DevFlowAssistant.Domain;
using Microsoft.EntityFrameworkCore;

namespace DevFlowAssistant.Infrastructure.Repositories;

public class ExecutionLogRepository : IExecutionLogRepository
{
    private readonly AppDbContext _context;

    public ExecutionLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ExecutionLog log)
    {
        _context.ExecutionLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public Task<List<ExecutionLog>> GetRecentAsync(int workflowId, int count = 20)
    {
        return _context.ExecutionLogs
            .AsNoTracking()
            .Where(log => log.WorkflowId == workflowId)
            .OrderByDescending(log => log.StartedAt)
            .Take(count)
            .ToListAsync();
    }

    public Task<List<ExecutionLog>> GetAllRecentAsync(int count = 50)
    {
        return _context.ExecutionLogs
            .AsNoTracking()
            .OrderByDescending(log => log.StartedAt)
            .Take(count)
            .ToListAsync();
    }
}
