using System.Threading.Tasks;

namespace BlockchainLight.Interfaces;

public interface IP2PServer
{
    public Task Start(bool needToCreateGenesis = false);
}