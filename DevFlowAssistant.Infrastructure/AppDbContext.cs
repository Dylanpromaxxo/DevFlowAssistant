using System;
using System.Collections.Generic;
using System.Security.RightsManagement;
using DevFlowAssistant.Domain;
using Microsoft.EntityFrameworkCore;
using DevFlowAssistant.Domain.Entities;

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExecutionLog>(entity =>
        {
            entity.HasOne(d => d.Workflow).WithMany(p => p.ExecutionLogs)
                .HasForeignKey(d => d.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Workflow>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(1);
        });

        modelBuilder.Entity<WorkflowAction>(entity =>
        {
            entity.Property(e => e.IsEnabled).HasDefaultValue(1);

            entity.HasOne(d => d.Workflow).WithMany(p => p.WorkflowActions)
                .HasForeignKey(d => d.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
