using System.Collections.Generic;
using BlockchainLight.Entities;
using BlockchainLight.Interfaces;

namespace BlockchainLight.Services;

public class StaticBlockRepository: IBlockRepository
{
    public Block GetGenesis()
    {
        return BlockStorage.Blocks.Count == 0 ? null : BlockStorage.Blocks[0];
    }

    public void AddGenesis(Block genesis)
    {
        if (BlockStorage.Blocks.Count > 0)
        {
            BlockStorage.Blocks.Clear();
        }
        
        BlockStorage.Blocks.Add(genesis);
    }

    public Block GetBlock(int index)
    {
        return BlockStorage.Blocks.Count <= index ? null : BlockStorage.Blocks[index];
    }

    public void AddBlock(Block block)
    {
        if (block.Index != GetBlocksCount())
        {
            return; // TODO() throw exception
        }
        
        BlockStorage.Blocks.Add(block);
    }

    public void ReplaceBlock(int index, Block block)
    {
        if (index >= GetBlocksCount())
        {
            return; // TODO() throw exception
        }
        
        BlockStorage.Blocks.Insert(index, block);
    }

    public Block GetLastBlock()
    {
        return BlockStorage.Blocks.Count == 0 ? null : BlockStorage.Blocks[^1];
    }

    public IReadOnlyCollection<Block> GetBlocks()
    {
        return BlockStorage.Blocks;
    }

    public int GetBlocksCount()
    {
        return BlockStorage.Blocks.Count;
    }

    public void Clear()
    {
        BlockStorage.Blocks.Clear();
    }
}