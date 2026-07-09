namespace DevFlowAssistant.Domain.Enums;

public static class WorkflowActionTypes
{
    public const string OpenApp = "OpenApp";
    public const string OpenUrl = "OpenUrl";
    public const string RunCommand = "RunCommand";
    public const string DockerCommand = "DockerCommand";
    public const string DockerCompose = "DockerCompose";

    public static readonly string[] All =
    [
        OpenApp,
        OpenUrl,
        RunCommand,
        DockerCommand,
        DockerCompose
    ];
}
