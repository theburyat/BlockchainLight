using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using BlockchainLight.CommandLine;
using BlockchainLight.Entities.Enums;
using BlockchainLight.Exceptions;
using BlockchainLight.Interfaces;
using BlockchainLight.Services;

namespace BlockchainLight;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var options = new CmdOptions();
        var commands = new CmdCommands(options);
        
        commands.RootCommand.SetHandler(async context =>
        {
            CmdContext.Port = context.ParseResult.GetValueForOption(options.Port);
            CmdContext.NodePort2 = context.ParseResult.GetValueForOption(options.NodePort2);
            CmdContext.NodePort3 = context.ParseResult.GetValueForOption(options.NodePort3);
            CmdContext.NeedToCreateGenesis = context.ParseResult.GetValueForOption(options.NeedToCreateGenesis);

            context.ExitCode = await StartAsync();
        });

        var parser = new CommandLineBuilder(commands.RootCommand).UseDefaults().Build();

        return await parser.InvokeAsync(args);
    }

    private static async Task<int> StartAsync()
    {
        IP2PServer server = new P2PServer();
        try
        {
            await server.StartAsync();
        }
        catch (BlockchainException ex)
        {
            Console.WriteLine(ex.ErrorCode.ToString());
            if (!string.IsNullOrWhiteSpace(ex.Message))
            {
                Console.WriteLine($"Message: {ex.Message}");
            }

            return (int) ex.ErrorCode;
        }
        
        return (int) ErrorCode.None;
    }
}
