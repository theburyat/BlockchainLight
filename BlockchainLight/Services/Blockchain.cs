using System;
using System.Collections.Generic;
using System.Linq;
using BlockchainLight.Entities;
using BlockchainLight.Entities.Interfaces;
using BlockchainLight.Interfaces;

namespace BlockchainLight.Services;

public class Blockchain: IBlockchain
{
    private const string RequiredHashEnding = "0000=";
    
    private List<IBlock> _chain;

    public void InitializeChain()
    {
        _chain = new List<IBlock>();
    }

    public void InitializeGenesis()
    {
        var genesis = new Block(0, null);
        CompleteBlockGeneration(genesis);
        
        _chain.Add(genesis);
    }

    public void AddGenesis(IBlock block)
    {
        if (_chain.Count > 0)
        {
            _chain.Clear();
        }

        _chain.Add(block);
    }

    public IBlock Mine()
    {
        var previousBlock = _chain[^1];
        IBlock block = new Block(previousBlock.Index + 1, previousBlock.Hash);
        CompleteBlockGeneration(block);

        return block;
    }

    public void AddBlock(IBlock block)
    {
        if (!IsBlockValid(block) || !IsBlockBetterThanTween(block))
        {
            return;
        }
        
        if (block.Index >= _chain.Count)
        {
            _chain.Add(block);
        }
        else
        {
            _chain[block.Index] = block;
        }
    }
    
    private void CompleteBlockGeneration(IBlock block)
    {
        while (!block.Hash.EndsWith(RequiredHashEnding))
        {
            block.ChangeNonce();
            block.Hash = block.CalculateHash();
        }
        block.TimeStamp = DateTime.Now;
    }

    private bool IsBlockBetterThanTween(IBlock block)
    {
        var twin = _chain.FirstOrDefault(x => x.Index == block.Index);
        if (twin is null)
        {
            return true;
        }

        return block.TimeStamp < twin.TimeStamp;
    }

    private bool IsBlockValid(IBlock block)
    {
        var previousBlock = _chain[block.Index - 1];

        return block.Hash == block.CalculateHash() && 
               block.Hash.EndsWith(RequiredHashEnding) && 
               block.PreviousHash == previousBlock.Hash;
    }
    
    public IReadOnlyCollection<IBlock> GetBlocks()
    {
        return _chain;
    }
}