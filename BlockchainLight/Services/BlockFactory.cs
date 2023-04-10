using System;
using System.Threading;
using System.Threading.Tasks;
using BlockchainLight.Entities;
using BlockchainLight.Entities.Enums;
using BlockchainLight.Exceptions;
using BlockchainLight.Interfaces;

namespace BlockchainLight.Services;

public class BlockFactory: IBlockFactory
{
    public async Task<Block> CreateBlockAsync(
        int index, 
        string previousHash, 
        string hashEndRestriction, 
        CancellationToken cancellationToken)
    {
        if (index < 0)
        {
            throw new BlockchainException(ErrorCode.InvalidIndex);
        }

        var block = new Block(index, previousHash);
        await CompleteBlockGenerationAsync(block, hashEndRestriction, cancellationToken);

        return block;
    }
    
    private async Task CompleteBlockGenerationAsync(Block block, string hashEndRestriction, CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            while (!block.Hash.EndsWith(hashEndRestriction))
            {
                block.ChangeNonce();
                block.Hash = block.CalculateHash();
            }
            block.TimeStamp = DateTime.Now;
        }, cancellationToken);
    }
}