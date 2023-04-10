using System.Threading;
using System.Threading.Tasks;
using BlockchainLight.Entities;

namespace BlockchainLight.Interfaces;

public interface IBlockFactory
{
    public Task<Block> CreateBlockAsync(
        int index, 
        string previousHash, 
        string hashEndRestriction,
        CancellationToken cancellationToken);
}