using System.Threading.Tasks;
using BlockchainLight.Interfaces;
using BlockchainLight.Services;

namespace BlockchainLight;

class Program
{
    static async Task Main(string[] args)
    {
        ConsoleArguments.Port = int.Parse(args[0]);

        ConsoleArguments.NeedToMakeGenesis = bool.Parse(args[1]);

        ConsoleArguments.Port2 = int.Parse(args[2]);
        //ConsoleArguments.Port3 = int.Parse(args[3]);

        IP2PServer server = new P2PServer();
        await server.Start(ConsoleArguments.NeedToMakeGenesis);
    }
}
