using System.CommandLine;

namespace BlockchainLight.CommandLine;

public class CmdOptions
{
    public Option<int> Port { get; } = new (name: "--port")
    {
        IsRequired = true,
        Arity = ArgumentArity.ExactlyOne
    };

    public Option<bool> NeedToCreateGenesis { get; } = new(name: "--need-to-create-genesis")
    {
        IsRequired = false
    };

    public Option<int> NodePort2 { get; } = new(name: "--node-port-2")
    {
        IsRequired = true,
        Arity = ArgumentArity.ExactlyOne
    };
    
    public Option<int> NodePort3 { get; } = new (name: "--node-port-3")
    {
        IsRequired = true,
        Arity = ArgumentArity.ExactlyOne
    };
}