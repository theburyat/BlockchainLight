using System.Collections.Generic;
using BlockchainLight.Entities;

namespace BlockchainLight.Interfaces;

public interface IBlockchain
{
    public void InitializeGenesis();

    public Block GetGenesis();

    public void AddGenesis(Block block);

    public Block Mine();
    
    public void AddBlock(Block block);

    public IReadOnlyCollection<Block> GetBlocks();
}
