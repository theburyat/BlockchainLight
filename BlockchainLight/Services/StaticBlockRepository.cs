using System.Collections.Generic;
using BlockchainLight.Entities;
using BlockchainLight.Entities.Enums;
using BlockchainLight.Exceptions;
using BlockchainLight.Interfaces;

namespace BlockchainLight.Services;

public class StaticBlockRepository: IBlockRepository
{
    public Block GetGenesis()
    {
        return BlockStorage.Blocks.Count == 0 
            ? throw new BlockchainException(ErrorCode.NoGenesis)
            : BlockStorage.Blocks[0];
    }

    public void AddGenesis(Block genesis)
    {
        if (BlockStorage.Blocks.Count > 0)
        {
            throw new BlockchainException(ErrorCode.AlreadyHasGenesis);
        }
        
        BlockStorage.Blocks.Add(genesis);
    }

    public Block GetBlock(int index)
    {
        return BlockStorage.Blocks.Count <= index 
            ? throw new BlockchainException(ErrorCode.NoBlock)
            : BlockStorage.Blocks[index];
    }

    public void AddBlock(Block block)
    {
        if (block.Index != GetBlocksCount())
        {
            throw new BlockchainException(ErrorCode.InvalidIndex);
        }
        
        BlockStorage.Blocks.Add(block);
    }

    public void ReplaceBlock(int index, Block block)
    {
        if (index >= GetBlocksCount())
        {
            throw new BlockchainException(ErrorCode.InvalidIndex);
        }
        
        BlockStorage.Blocks.Insert(index, block);
    }

    public Block GetLastBlock()
    {
        return BlockStorage.Blocks.Count == 0 
            ? throw new BlockchainException(ErrorCode.NoBlock) 
            : BlockStorage.Blocks[^1];
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