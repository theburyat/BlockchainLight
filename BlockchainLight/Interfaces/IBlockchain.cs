using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlockchainLight.Entities;

namespace BlockchainLight.Interfaces;

public interface IBlockchain
{
    public Task InitializeGenesisAsync(CancellationToken cancellationToken);

    public Block GetGenesis();

    public void AddGenesis(Block block);

    public Task<Block> MineAsync(CancellationToken cancellationToken);
    
    public void AddBlock(Block block);

    public IReadOnlyCollection<Block> GetBlocks();
}
