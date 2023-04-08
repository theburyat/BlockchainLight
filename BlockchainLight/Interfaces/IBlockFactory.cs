using BlockchainLight.Entities;

namespace BlockchainLight.Interfaces;

public interface IBlockFactory
{
    public Block CreateBlock(int index, string previousHash, string hashEndRestriction);
}