using System.Collections.Generic;
using BlockchainLight.Entities.Interfaces;

namespace BlockchainLight.Interfaces;

public interface IBlockchain
{
    public void InitializeChain();
    
    public void InitializeGenesis();

    public void AddGenesis(IBlock block);

    public IBlock Mine();
    
    public void AddBlock(IBlock block);

    public IReadOnlyCollection<IBlock> GetBlocks();
}
