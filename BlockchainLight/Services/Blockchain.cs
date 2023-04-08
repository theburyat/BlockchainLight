using System.Collections.Generic;
using System.Linq;
using BlockchainLight.Entities;
using BlockchainLight.Interfaces;

namespace BlockchainLight.Services;

public class Blockchain: IBlockchain
{
    private const string HashEndRestriction = "0000=";

    private readonly IBlockRepository _repository;
    private readonly IBlockFactory _factory;

    public Blockchain(IBlockRepository repository, IBlockFactory factory)
    {
        _repository = repository;
        _factory = factory;
    }
    
    public void InitializeGenesis()
    {
        var genesis = _factory.CreateBlock(0, null, HashEndRestriction);
        _repository.AddGenesis(genesis);
    }

    public Block GetGenesis()
    {
        return _repository.GetBlocksCount() == 0 ? null : _repository.GetGenesis();
    }

    public void AddGenesis(Block block)
    {
        if (_repository.GetBlocksCount() > 0)
        {
            _repository.Clear();
        }

        _repository.AddBlock(block);
    }

    public Block Mine()
    {
        var previousBlock = _repository.GetLastBlock();
        var block = _factory.CreateBlock(previousBlock.Index + 1, previousBlock.Hash, HashEndRestriction);

        return block;
    }

    public void AddBlock(Block block)
    {
        if (!IsBlockValid(block) || !IsBlockBetterThanTween(block))
        {
            return;
        }
        
        if (block.Index >= _repository.GetBlocksCount())
        {
            _repository.AddBlock(block);
        }
        else
        {
            _repository.ReplaceBlock(block.Index, block);
        }
    }
    
    public IReadOnlyCollection<Block> GetBlocks()
    {
        return _repository.GetBlocks();
    }

    private bool IsBlockBetterThanTween(Block block)
    {
        var twin = _repository.GetBlocks().FirstOrDefault(x => x.Index == block.Index);
        if (twin is null)
        {
            return true;
        }

        return block.TimeStamp < twin.TimeStamp;
    }

    private bool IsBlockValid(Block block)
    {
        if (block.Index > _repository.GetBlocksCount())
        {
            return false;
        }

        var previousBlock = _repository.GetBlock(block.Index - 1);

        return block.Hash == block.CalculateHash() && 
               block.Hash.EndsWith(HashEndRestriction) && 
               block.PreviousHash == previousBlock.Hash;
    }
}