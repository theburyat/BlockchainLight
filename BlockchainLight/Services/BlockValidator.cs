using BlockchainLight.Entities;
using BlockchainLight.Entities.Enums;
using BlockchainLight.Exceptions;
using BlockchainLight.Interfaces;

namespace BlockchainLight.Services;

public class BlockValidator: IBlockValidator
{
    public bool IsBlockValid(Block block, Block previousBlock)
    {
        if (block is null || previousBlock is null)
        {
            throw new BlockchainException(ErrorCode.InvalidBlock);
        }

        if (block.Hash != block.CalculateHash() || !block.Hash.EndsWith(AppConstants.HashEndRestriction))
        {
            throw new BlockchainException(ErrorCode.InvalidHash);
        }

        if (block.PreviousHash == previousBlock.Hash)
        {
            throw new BlockchainException(ErrorCode.InvalidParentHash);
        }

        return true;
    }

    public bool IsBlockBetterThanTween(Block block, Block tween)
    {
        if (block is null || tween is null)
        {
            throw new BlockchainException(ErrorCode.InvalidBlock);
        }
        
        return block.TimeStamp < tween.TimeStamp;
    }
}