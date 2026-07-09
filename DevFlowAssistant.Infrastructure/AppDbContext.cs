using DevFlowAssistant.Domain;
using DevFlowAssistant.Domain.Entities;
using DevFlowAssistant.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DevFlowAssistant.Infrastructure;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ExecutionLog> ExecutionLogs { get; set; }

    public virtual DbSet<Workflow> Workflows { get; set; }

    public virtual DbSet<WorkflowAction> WorkflowActions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExecutionLog>(entity =>
        {
            entity.Property(log => log.StartedAt)
                .HasConversion(value => DateTimeStorage.ToStorage(value), value => DateTimeStorage.FromStorage(value))
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(log => log.FinishedAt)
                .HasConversion(value => DateTimeStorage.ToStorageNullable(value), value => DateTimeStorage.FromStorageNullable(value));

            entity.HasOne(log => log.Workflow)
                .WithMany(workflow => workflow.ExecutionLogs)
                .HasForeignKey(log => log.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Workflow>(entity =>
        {
            entity.Property(workflow => workflow.IsActive).HasDefaultValue(true);

            entity.Property(workflow => workflow.CreatedAt)
                .HasConversion(value => DateTimeStorage.ToStorage(value), value => DateTimeStorage.FromStorage(value))
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(workflow => workflow.UpdatedAt)
                .HasConversion(value => DateTimeStorage.ToStorageNullable(value), value => DateTimeStorage.FromStorageNullable(value));
        });

        modelBuilder.Entity<WorkflowAction>(entity =>
        {
            entity.Property(action => action.Name).HasDefaultValue("Nueva accion");
            entity.Property(action => action.IsEnabled).HasDefaultValue(true);
            entity.Property(action => action.TimeoutSeconds).HasDefaultValue(120);
            entity.Property(action => action.ContinueOnError).HasDefaultValue(false);

            entity.Property(action => action.CreatedAt)
                .HasConversion(value => DateTimeStorage.ToStorage(value), value => DateTimeStorage.FromStorage(value))
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(action => action.Workflow)
                .WithMany(workflow => workflow.WorkflowActions)
                .HasForeignKey(action => action.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
