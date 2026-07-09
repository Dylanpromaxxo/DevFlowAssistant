using Microsoft.EntityFrameworkCore;

namespace DevFlowAssistant.Infrastructure.Database;

public interface IDatabaseInitializer
{
    Task InitializeAsync();
}

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly AppDbContext _context;

    public DatabaseInitializer(AppDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync()
    {
        await _context.Database.EnsureCreatedAsync();

        await TryExecuteAsync("ALTER TABLE WorkflowActions ADD COLUMN Name TEXT NOT NULL DEFAULT 'Nueva accion';");
        await TryExecuteAsync("ALTER TABLE WorkflowActions ADD COLUMN TimeoutSeconds INTEGER NOT NULL DEFAULT 120;");
        await TryExecuteAsync("ALTER TABLE WorkflowActions ADD COLUMN ContinueOnError INTEGER NOT NULL DEFAULT 0;");
        await TryExecuteAsync("ALTER TABLE ExecutionLogs ADD COLUMN WorkflowActionId INTEGER NULL;");
        await TryExecuteAsync("ALTER TABLE ExecutionLogs ADD COLUMN ActionName TEXT NULL;");
        await TryExecuteAsync("ALTER TABLE ExecutionLogs ADD COLUMN StandardOutput TEXT NULL;");
        await TryExecuteAsync("ALTER TABLE ExecutionLogs ADD COLUMN StandardError TEXT NULL;");
        await NormalizeDatesAsync();
    }

    private async Task TryExecuteAsync(string sql)
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync(sql);
        }
        catch
        {
            // SQLite throws when a column already exists. Startup should stay idempotent.
        }
    }

    private async Task NormalizeDatesAsync()
    {
        var workflows = await _context.Workflows.ToListAsync();
        foreach (var workflow in workflows)
        {
            workflow.CreatedAt = workflow.CreatedAt.ToUniversalTime();
            workflow.UpdatedAt = workflow.UpdatedAt?.ToUniversalTime();
        }

        var actions = await _context.WorkflowActions.ToListAsync();
        foreach (var action in actions)
        {
            action.CreatedAt = action.CreatedAt.ToUniversalTime();
        }

        var logs = await _context.ExecutionLogs.ToListAsync();
        foreach (var log in logs)
        {
            log.StartedAt = log.StartedAt.ToUniversalTime();
            log.FinishedAt = log.FinishedAt?.ToUniversalTime();
        }

        await _context.SaveChangesAsync();
    }
}
