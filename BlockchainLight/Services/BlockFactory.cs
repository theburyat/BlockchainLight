using System;
using BlockchainLight.Entities;
using BlockchainLight.Interfaces;

namespace BlockchainLight.Services;

public class BlockFactory: IBlockFactory
{
    public Block CreateBlock(int index, string previousHash, string hashEndRestriction)
    {
        var block = new Block(index, previousHash);
        CompleteBlockGeneration(block, hashEndRestriction);

        return block;
    }
    
    private void CompleteBlockGeneration(Block block, string hashEndRestriction)
    {
        while (!block.Hash.EndsWith(hashEndRestriction))
        {
            block.ChangeNonce();
            block.Hash = block.CalculateHash();
        }
        block.TimeStamp = DateTime.Now;
    }
}