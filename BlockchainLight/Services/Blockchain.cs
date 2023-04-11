using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockchainLight.Entities;
using BlockchainLight.Interfaces;

namespace BlockchainLight.Services;

public class Blockchain: IBlockchain
{
    private readonly IBlockRepository _repository;
    private readonly IBlockFactory _factory;
    private readonly IBlockValidator _validator;

    public Blockchain(IBlockRepository repository, IBlockFactory factory, IBlockValidator validator)
    {
        _repository = repository;
        _factory = factory;
        _validator = validator;
    }
    
    public async Task InitializeGenesisAsync(CancellationToken cancellationToken)
    {
        var genesis = await _factory.CreateBlockAsync(0, null, AppConstants.HashEndRestriction, cancellationToken);
        _repository.AddGenesis(genesis);
    }

    public Block GetGenesis()
    {
        return _repository.GetGenesis();
    }

    public void AddGenesis(Block block)
    {
        _repository.AddGenesis(block);
    }

    public async Task<Block> MineAsync(CancellationToken cancellationToken)
    {
        var previousBlock = _repository.GetLastBlock();
        var block = await _factory.CreateBlockAsync(
            previousBlock.Index + 1, 
            previousBlock.Hash, 
            AppConstants.HashEndRestriction,
            cancellationToken);

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
    
    private bool IsBlockValid(Block block)
    {
        if (block is null || block.Index > _repository.GetBlocksCount())
        {
            return false;
        }

        var previousBlock = _repository.GetBlock(block.Index - 1);

        return _validator.IsBlockValid(block, previousBlock, AppConstants.HashEndRestriction);
    }

    private bool IsBlockBetterThanTween(Block block)
    {
        var tween = _repository.GetBlocks().FirstOrDefault(x => x.Index == block.Index);
        if (tween is null)
        {
            return true;
        }

        return _validator.IsBlockBetterThanTween(block, tween);
    }
}