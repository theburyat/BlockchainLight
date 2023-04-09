using System.CommandLine;

namespace BlockchainLight.CommandLine;

public class CmdCommands
{
    public RootCommand RootCommand { get; } = new ("Blockchain");

    public CmdCommands(CmdOptions options)
    {
        AddOptions(options);
    }

    private void AddOptions(CmdOptions options)
    {
        RootCommand.AddOption(options.Port);
        RootCommand.AddOption(options.NodePort2);
        RootCommand.AddOption(options.NodePort3);
        RootCommand.AddOption(options.NeedToCreateGenesis);
    }
}