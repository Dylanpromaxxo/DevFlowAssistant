using System.Diagnostics;
using DevFlowAssistant.Application.Interfaces;
using DevFlowAssistant.Application.Services.Interface;
using DevFlowAssistant.Domain;
using DevFlowAssistant.Domain.Entities;
using DevFlowAssistant.Domain.Enums;

namespace DevFlowAssistant.Application.Services.implementation;

public class WorkflowExecutionService : IWorkflowExecutionService
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IWorkflowActionRepository _actionRepository;
    private readonly IExecutionLogRepository _logRepository;

    public WorkflowExecutionService(
        IWorkflowRepository workflowRepository,
        IWorkflowActionRepository actionRepository,
        IExecutionLogRepository logRepository)
    {
        _workflowRepository = workflowRepository;
        _actionRepository = actionRepository;
        _logRepository = logRepository;
    }

    public async Task<IReadOnlyList<ExecutionLog>> ExecuteAsync(int workflowId, CancellationToken cancellationToken = default)
    {
        var workflow = await _workflowRepository.GetByIdAsync(workflowId)
            ?? throw new InvalidOperationException("No se encontró el workflow seleccionado.");

        if (!workflow.IsActive)
        {
            throw new InvalidOperationException("El workflow está inactivo.");
        }

        var results = new List<ExecutionLog>();
        var actions = (await _actionRepository.GetByWorkflowIdAsync(workflowId))
            .Where(action => action.IsEnabled)
            .OrderBy(action => action.SortOrder)
            .ToList();

        if (actions.Count == 0)
        {
            var emptyLog = CreateLog(workflow.Id, null, "Skipped", "El workflow no tiene acciones habilitadas.");
            emptyLog.FinishedAt = DateTime.UtcNow;
            await _logRepository.AddAsync(emptyLog);
            results.Add(emptyLog);
            return results;
        }

        foreach (var action in actions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var log = CreateLog(workflow.Id, action, "Running", $"Ejecutando {action.Name}.");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var result = await ExecuteActionAsync(action, cancellationToken);

                log.Status = result.ExitCode == 0 ? "Succeeded" : "Failed";
                log.Message = result.ExitCode == 0
                    ? "Acción ejecutada correctamente."
                    : $"La acción terminó con código {result.ExitCode}.";
                log.StandardOutput = result.StandardOutput;
                log.StandardError = result.StandardError;

                if (result.ExitCode != 0 && !action.ContinueOnError)
                {
                    log.ErrorDetails = result.StandardError;
                }
            }
            catch (Exception ex)
            {
                log.Status = "Failed";
                log.Message = "La acción falló.";
                log.ErrorDetails = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                log.FinishedAt = DateTime.UtcNow;
                log.DurationMs = (int)stopwatch.ElapsedMilliseconds;
                await _logRepository.AddAsync(log);
                results.Add(log);
            }

            if (log.Status == "Failed" && !action.ContinueOnError)
            {
                break;
            }
        }

        return results;
    }

    private static ExecutionLog CreateLog(int workflowId, WorkflowAction? action, string status, string message)
    {
        return new ExecutionLog
        {
            WorkflowId = workflowId,
            WorkflowActionId = action?.Id,
            ActionName = action?.Name,
            StartedAt = DateTime.UtcNow,
            Status = status,
            Message = message
        };
    }

    private static Task<ProcessResult> ExecuteActionAsync(WorkflowAction action, CancellationToken cancellationToken)
    {
        return action.ActionType switch
        {
            WorkflowActionTypes.OpenUrl => OpenShellTargetAsync(action.Value, cancellationToken),
            WorkflowActionTypes.OpenApp => OpenAppAsync(action.Value, action.Arguments, action.WorkingDirectory, cancellationToken),
            WorkflowActionTypes.RunCommand => RunProcessAsync(action.Value, action.Arguments, action.WorkingDirectory, action.TimeoutSeconds, cancellationToken),
            WorkflowActionTypes.DockerCommand => RunProcessAsync("docker", BuildCommand(action.Value, action.Arguments), action.WorkingDirectory, action.TimeoutSeconds, cancellationToken),
            WorkflowActionTypes.DockerCompose => RunProcessAsync("docker", BuildCommand("compose", BuildCommand(action.Value, action.Arguments)), action.WorkingDirectory, action.TimeoutSeconds, cancellationToken),
            _ => throw new NotSupportedException($"Tipo de acción no soportado: {action.ActionType}")
        };
    }

    private static Task<ProcessResult> OpenShellTargetAsync(string target, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = target,
            UseShellExecute = true
        });

        return Task.FromResult(new ProcessResult(0, $"Abierto: {target}", null));
    }

    private static Task<ProcessResult> OpenAppAsync(string fileName, string? arguments, string? workingDirectory, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments ?? string.Empty,
            UseShellExecute = true
        };

        if (!string.IsNullOrWhiteSpace(workingDirectory))
        {
            startInfo.WorkingDirectory = workingDirectory;
        }

        Process.Start(startInfo);

        return Task.FromResult(new ProcessResult(0, $"Aplicación abierta: {fileName}", null));
    }

    private static async Task<ProcessResult> RunProcessAsync(string fileName, string? arguments, string? workingDirectory, int timeoutSeconds, CancellationToken cancellationToken)
    {
        using var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutTokenSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments ?? string.Empty,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        if (!string.IsNullOrWhiteSpace(workingDirectory))
        {
            startInfo.WorkingDirectory = workingDirectory;
        }

        using var process = new Process { StartInfo = startInfo };

        process.Start();

        var standardOutputTask = process.StandardOutput.ReadToEndAsync(timeoutTokenSource.Token);
        var standardErrorTask = process.StandardError.ReadToEndAsync(timeoutTokenSource.Token);

        try
        {
            await process.WaitForExitAsync(timeoutTokenSource.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            TryKill(process);
            return new ProcessResult(-1, null, $"Timeout después de {timeoutSeconds} segundos.");
        }

        var standardOutput = await standardOutputTask;
        var standardError = await standardErrorTask;

        return new ProcessResult(process.ExitCode, standardOutput, standardError);
    }

    private static string BuildCommand(string first, string? second)
    {
        return string.IsNullOrWhiteSpace(second) ? first : $"{first} {second}";
    }

    private static void TryKill(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch
        {
            // Nothing actionable for the UI; the log already records the timeout.
        }
    }

    private sealed record ProcessResult(int ExitCode, string? StandardOutput, string? StandardError);
}
