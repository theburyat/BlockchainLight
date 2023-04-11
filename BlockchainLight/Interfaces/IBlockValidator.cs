using BlockchainLight.Entities;

namespace BlockchainLight.Interfaces;

public interface IBlockValidator
{
    public bool IsBlockValid(Block block, Block previousBlock, string hashEndRestricting);

    public bool IsBlockBetterThanTween(Block block, Block tween);
}