using BlockchainLight.Entities;

namespace BlockchainLight.Interfaces;

public interface IBlockValidator
{
    public bool IsBlockValid(Block block, Block previousBlock);

    public bool IsBlockBetterThanTween(Block block, Block tween);
}