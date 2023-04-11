using System.Collections.Generic;
using BlockchainLight.Entities;

namespace BlockchainLight.Interfaces;

public interface IBlockRepository
{
    public Block GetGenesis();

    public void AddGenesis(Block genesis);

    public Block GetBlock(int index);

    public void AddBlock(Block block);

    public void ReplaceBlock(int index, Block block);
    
    public Block GetLastBlock();

    public IReadOnlyCollection<Block> GetBlocks();

    public int GetBlocksCount();

    public void Clear();
}