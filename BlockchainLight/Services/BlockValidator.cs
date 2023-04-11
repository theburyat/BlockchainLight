using System;
using BlockchainLight.Entities;
using BlockchainLight.Entities.Enums;
using BlockchainLight.Exceptions;
using BlockchainLight.Interfaces;

namespace BlockchainLight.Services;

public class BlockValidator: IBlockValidator
{
    public bool IsBlockValid(Block block, Block previousBlock, string hashEndRestricting)
    {
        try
        {
            ValidateBlock(block, previousBlock, hashEndRestricting);
        }
        catch (BlockchainException ex)
        {
            Console.WriteLine($"Invalid block: {ex.ErrorCode.ToString()}");
            return false;
        }

        return true;
    }

    private void ValidateBlock(Block block, Block previousBlock, string hashEndRestricting)
    {
        if (block is null || previousBlock is null)
        {
            throw new BlockchainException(ErrorCode.InvalidBlock);
        }

        if (block.Hash != block.CalculateHash() || !block.Hash.EndsWith(hashEndRestricting))
        {
            throw new BlockchainException(ErrorCode.InvalidHash);
        }

        if (block.PreviousHash != previousBlock.Hash)
        {
            throw new BlockchainException(ErrorCode.InvalidParentHash);
        }

        if (string.IsNullOrWhiteSpace(block.Data) || block.Data.Length != AppConstants.DefaultDataLength)
        {
            throw new BlockchainException(ErrorCode.InvalidData);
        }

        if (block.TimeStamp == default)
        {
            throw new BlockchainException(ErrorCode.InvalidTimestamp);
        }
    }

    public bool IsBlockBetterThanTween(Block block, Block tween)
    {
        return block.TimeStamp < tween.TimeStamp;
    }
}