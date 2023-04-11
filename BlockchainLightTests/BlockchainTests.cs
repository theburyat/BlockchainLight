using BlockchainLight.Entities;
using BlockchainLight.Entities.Enums;
using BlockchainLight.Exceptions;
using BlockchainLight.Interfaces;
using BlockchainLight.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlockchainLightTests;

public class BlockchainTests
{
    private readonly Mock<IBlockRepository> _repositoryMock = new();
    private readonly Mock<IBlockFactory> _factoryMock = new();
    private readonly Mock<IBlockValidator> _validatorMock = new();

    #region InitializeGenesisAsyncTests

    [Fact]
    public async Task InitializeGenesisAsync_Ordinary_ShouldBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);

        _factoryMock
            .Setup(x => x.CreateBlockAsync(It.IsAny<int>(), It.IsAny<string>(),It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new Block(0, null)));
        _repositoryMock
            .Setup(x => x.AddGenesis(It.IsAny<Block>()))
            .Verifiable();
        
        // act
        var task = () => blockchain.InitializeGenesisAsync(CancellationToken.None);
        
        // arrange
        await task.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task InitializeGenesisAsync_CreateBlockWithInvalidIndex_ShouldNotBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);

        _factoryMock
            .Setup(x => x.CreateBlockAsync(It.IsAny<int>(), It.IsAny<string>(),It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new BlockchainException(ErrorCode.InvalidIndex));
        _repositoryMock
            .Setup(x => x.AddGenesis(It.IsAny<Block>()))
            .Verifiable();
        
        // act
        var task = () => blockchain.InitializeGenesisAsync(CancellationToken.None);
        
        // arrange
        (await task.Should().ThrowAsync<BlockchainException>()).And.ErrorCode.Should().Be(ErrorCode.InvalidIndex);
    }
    
    [Fact]
    public async Task InitializeGenesisAsync_CreateBlockWithInvalidParentHash_ShouldNotBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);

        _factoryMock
            .Setup(x => x.CreateBlockAsync(It.IsAny<int>(), It.IsAny<string>(),It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new BlockchainException(ErrorCode.InvalidParentHash));
        _repositoryMock
            .Setup(x => x.AddGenesis(It.IsAny<Block>()))
            .Verifiable();
        
        // act
        var task = () => blockchain.InitializeGenesisAsync(CancellationToken.None);
        
        // arrange
        (await task.Should().ThrowAsync<BlockchainException>()).And.ErrorCode.Should().Be(ErrorCode.InvalidParentHash);
    }
    
    [Fact]
    public async Task InitializeGenesisAsync_CreateBlockWithInvalidHashEndRestriction_ShouldNotBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);

        _factoryMock
            .Setup(x => x.CreateBlockAsync(It.IsAny<int>(), It.IsAny<string>(),It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new BlockchainException(ErrorCode.InvalidHashEndRestriction));
        _repositoryMock
            .Setup(x => x.AddGenesis(It.IsAny<Block>()))
            .Verifiable();
        
        // act
        var task = () => blockchain.InitializeGenesisAsync(CancellationToken.None);
        
        // arrange
        (await task.Should().ThrowAsync<BlockchainException>()).And.ErrorCode.Should().Be(ErrorCode.InvalidHashEndRestriction);
    }
    
    [Fact]
    public async Task InitializeGenesisAsync_AddBlockWhenAlreadyHasGenesis_ShouldNotBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);

        _factoryMock
            .Setup(x => x.CreateBlockAsync(It.IsAny<int>(), It.IsAny<string>(),It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new Block(0, null)));
        _repositoryMock
            .Setup(x => x.AddGenesis(It.IsAny<Block>()))
            .Throws(new BlockchainException(ErrorCode.AlreadyHasGenesis));
        
        // act
        var task = () => blockchain.InitializeGenesisAsync(CancellationToken.None);
        
        // arrange
        (await task.Should().ThrowAsync<BlockchainException>()).And.ErrorCode.Should().Be(ErrorCode.AlreadyHasGenesis);
    }
    
    #endregion

    #region GetGenesisTests

    [Fact]
    public void GetGenesis_Ordinary_ShouldBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);

        _repositoryMock
            .Setup(x => x.GetGenesis())
            .Returns(new Block(0, null));

        // act
        var result = blockchain.GetGenesis();
        
        // arrange
        result.Should().NotBeNull();
        result.Index.Should().Be(0);
        result.PreviousHash.Should().Be(null);
    }
    
    [Fact]
    public void GetGenesis_NoGenesis_ShouldNotBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);

        _repositoryMock
            .Setup(x => x.GetGenesis())
            .Throws(new BlockchainException(ErrorCode.NoGenesis));

        // act
        var fun = () => blockchain.GetGenesis();
        
        // arrange
        fun.Should().Throw<BlockchainException>().And.ErrorCode.Should().Be(ErrorCode.NoGenesis);
    }

    #endregion
    
    #region AddGenesisTests

    [Fact]
    public void AddGenesis_Ordinary_ShouldBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);

        _repositoryMock
            .Setup(x => x.AddGenesis(It.IsAny<Block>()))
            .Verifiable();

        var genesis = new Block(0, null);

        // act
        var fun = () => blockchain.AddGenesis(genesis);
        
        // arrange
        fun.Should().NotThrow();
    }
    
    [Fact]
    public void AddGenesis_AlreadyHasGenesis_ShouldNotBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);

        _repositoryMock
            .Setup(x => x.AddGenesis(It.IsAny<Block>()))
            .Throws(new BlockchainException(ErrorCode.AlreadyHasGenesis));
        
        var genesis = new Block(0, null);

        // act
        var fun = () => blockchain.AddGenesis(genesis);
        
        // arrange
        fun.Should().Throw<BlockchainException>().And.ErrorCode.Should().Be(ErrorCode.AlreadyHasGenesis);
    }

    #endregion

    #region MineAsyncTests

    [Fact]
    public async Task MineAsync_Ordinary_ShouldBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        
        _repositoryMock
            .Setup(x => x.GetLastBlock())
            .Returns(new Block(0, null));
        _factoryMock
            .Setup(x => x.CreateBlockAsync(It.IsAny<int>(), It.IsAny<string>(),It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new Block(1, "zxczxczxc")));
        
        // act
        var task = () => blockchain.MineAsync(CancellationToken.None);
        
        // assert
        await task.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task MineAsync_GetLastBlockWhenNoBlocks_ShouldBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        
        _repositoryMock
            .Setup(x => x.GetLastBlock())
            .Throws(new BlockchainException(ErrorCode.NoBlock));
        _factoryMock
            .Setup(x => x.CreateBlockAsync(It.IsAny<int>(), It.IsAny<string>(),It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new Block(1, "zxczxczxc")));
        
        // act
        var task = () => blockchain.MineAsync(CancellationToken.None);
        
        // assert
        (await task.Should().ThrowAsync<BlockchainException>()).And.ErrorCode.Should().Be(ErrorCode.NoBlock);
    }
    
    [Fact]
    public async Task MineAsync_CreateBlockWithInvalidIndex_ShouldBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        
        _repositoryMock
            .Setup(x => x.GetLastBlock())
            .Returns(new Block(0, null));
        _factoryMock
            .Setup(x => x.CreateBlockAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new BlockchainException(ErrorCode.InvalidIndex));
        
        // act
        var task = () => blockchain.MineAsync(CancellationToken.None);
        
        // assert
        (await task.Should().ThrowAsync<BlockchainException>()).And.ErrorCode.Should().Be(ErrorCode.InvalidIndex);
    }
    
    [Fact]
    public async Task MineAsync_CreateBlockWithInvalidParentHash_ShouldBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        
        _repositoryMock
            .Setup(x => x.GetLastBlock())
            .Returns(new Block(0, null));
        _factoryMock
            .Setup(x => x.CreateBlockAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new BlockchainException(ErrorCode.InvalidParentHash));
        
        // act
        var task = () => blockchain.MineAsync(CancellationToken.None);
        
        // assert
        (await task.Should().ThrowAsync<BlockchainException>()).And.ErrorCode.Should().Be(ErrorCode.InvalidParentHash);
    }
    
    [Fact]
    public async Task MineAsync_CreateBlockWithInvalidHashEndRestriction_ShouldBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        
        _repositoryMock
            .Setup(x => x.GetLastBlock())
            .Returns(new Block(0, null));
        _factoryMock
            .Setup(x => x.CreateBlockAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new BlockchainException(ErrorCode.InvalidHashEndRestriction));
        
        // act
        var task = () => blockchain.MineAsync(CancellationToken.None);
        
        // assert
        (await task.Should().ThrowAsync<BlockchainException>()).And.ErrorCode.Should().Be(ErrorCode.InvalidHashEndRestriction);
    }

    #endregion

    #region AddBlockTests

    [Fact]
    public void AddBlock_OrdinaryAddBlock_ShouldBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        var parentBlock = new Block(0, null);
        var block = new Block(parentBlock.Index + 1, parentBlock.Hash);
        
        _repositoryMock
            .Setup(x => x.GetBlocksCount())
            .Returns(1);
        _repositoryMock
            .Setup(x => x.GetBlock(It.IsAny<int>()))
            .Returns(parentBlock);
        _repositoryMock
            .Setup(x => x.GetBlocks())
            .Returns(new List<Block> { parentBlock });
        _validatorMock
            .Setup(x => x.IsBlockValid(It.IsAny<Block>(), It.IsAny<Block>(), It.IsAny<string>()))
            .Returns(true);

        // act
        var fun = () => blockchain.AddBlock(block);
        
        // assert
        fun.Should().NotThrow();
    }

    [Fact]
    public void AddBlock_OrdinaryReplaceBlock_ShouldBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        var genesis = new Block(0, null);
        var block1 = new Block(genesis.Index + 1, genesis.Hash);
        var block2 = new Block(genesis.Index + 1, genesis.Hash);
        
        _repositoryMock
            .Setup(x => x.GetBlocksCount())
            .Returns(2);
        _repositoryMock
            .Setup(x => x.GetBlock(It.IsAny<int>()))
            .Returns(block1);
        _repositoryMock
            .Setup(x => x.GetBlocks())
            .Returns(new List<Block> { genesis, block1 });
        _repositoryMock
            .Setup(x => x.ReplaceBlock(It.IsAny<int>(), It.IsAny<Block>()))
            .Verifiable();
        _validatorMock
            .Setup(x => x.IsBlockValid(It.IsAny<Block>(), It.IsAny<Block>(), It.IsAny<string>()))
            .Returns(true);
        _validatorMock
            .Setup(x => x.IsBlockBetterThanTween(It.IsAny<Block>(), It.IsAny<Block>()))
            .Returns(true);

        // act
        var fun = () => blockchain.AddBlock(block2);
        
        // assert
        fun.Should().NotThrow();
    }
    
    [Fact]
    public void AddBlock_InvalidBlockNull_ShouldBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        
        // act
        var fun = () => blockchain.AddBlock(null);
        
        // assert
        fun.Should().NotThrow();
    }
    
    [Fact]
    public void AddBlock_TooBigIndex_ShouldBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        var parentBlock = new Block(0, null);
        var block = new Block(parentBlock.Index + 10, parentBlock.Hash);
        
        _repositoryMock
            .Setup(x => x.GetBlocksCount())
            .Returns(1);
        
        // act
        var fun = () => blockchain.AddBlock(block);
        
        // assert
        fun.Should().NotThrow();
    }

    [Fact]
    public void AddBlock_NoPreviousBlock_ShouldNotBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        var parentBlock = new Block(0, null);
        var block = new Block(parentBlock.Index + 1, parentBlock.Hash);
        
        _repositoryMock
            .Setup(x => x.GetBlocksCount())
            .Returns(1);
        _repositoryMock
            .Setup(x => x.GetBlock(It.IsAny<int>()))
            .Throws(new BlockchainException(ErrorCode.NoBlock));
        
        // act
        var fun = () => blockchain.AddBlock(block);
        
        // assert
        fun.Should().Throw<BlockchainException>().And.ErrorCode.Should().Be(ErrorCode.NoBlock);
    }
    
    [Fact]
    public void AddBlock_BlockIsNotValid_ShouldNotBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        var parentBlock = new Block(0, null);
        var block = new Block(parentBlock.Index + 1, parentBlock.Hash);
        
        _repositoryMock
            .Setup(x => x.GetBlocksCount())
            .Returns(1);
        _repositoryMock
            .Setup(x => x.GetBlock(It.IsAny<int>()))
            .Returns(parentBlock);
        _validatorMock
            .Setup(x => x.IsBlockValid(It.IsAny<Block>(), It.IsAny<Block>(), It.IsAny<string>()))
            .Returns(false);

        // act
        var fun = () => blockchain.AddBlock(block);
        
        // assert
        fun.Should().NotThrow();
    }
    
    [Fact]
    public void AddBlock_TweenIsWorse_ShouldNotBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        var parentBlock = new Block(0, null);
        var block = new Block(parentBlock.Index + 1, parentBlock.Hash);
        
        _repositoryMock
            .Setup(x => x.GetBlocksCount())
            .Returns(1);
        _repositoryMock
            .Setup(x => x.GetBlock(It.IsAny<int>()))
            .Returns(parentBlock);
        _repositoryMock
            .Setup(x => x.GetBlocks())
            .Returns(new List<Block> { parentBlock, block });
        _validatorMock
            .Setup(x => x.IsBlockValid(It.IsAny<Block>(), It.IsAny<Block>(), It.IsAny<string>()))
            .Returns(true);
        _validatorMock
            .Setup(x => x.IsBlockBetterThanTween(It.IsAny<Block>(), It.IsAny<Block>()))
            .Returns(true);

        // act
        var fun = () => blockchain.AddBlock(block);
        
        // assert
        fun.Should().NotThrow();
    }

    [Fact]
    public void AddBlock_TweenIsBetter_ShouldNotBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        var parentBlock = new Block(0, null);
        var block = new Block(parentBlock.Index + 1, parentBlock.Hash);
        
        _repositoryMock
            .Setup(x => x.GetBlocksCount())
            .Returns(2);
        _repositoryMock
            .Setup(x => x.GetBlock(It.IsAny<int>()))
            .Returns(parentBlock);
        _repositoryMock
            .Setup(x => x.GetBlocks())
            .Returns(new List<Block> { parentBlock, block });
        _validatorMock
            .Setup(x => x.IsBlockValid(It.IsAny<Block>(), It.IsAny<Block>(), It.IsAny<string>()))
            .Returns(true);
        _validatorMock
            .Setup(x => x.IsBlockBetterThanTween(It.IsAny<Block>(), It.IsAny<Block>()))
            .Returns(false);

        // act
        var fun = () => blockchain.AddBlock(block);
        
        // assert
        fun.Should().NotThrow();
    }
    
    [Fact]
    public void AddBlock_AddBlockWithInvalidIndex_ShouldNotBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        var parentBlock = new Block(0, null);
        var block = new Block(parentBlock.Index + 1, parentBlock.Hash);
        
        _repositoryMock
            .Setup(x => x.GetBlocksCount())
            .Returns(1);
        _repositoryMock
            .Setup(x => x.GetBlock(It.IsAny<int>()))
            .Returns(parentBlock);
        _repositoryMock
            .Setup(x => x.GetBlocks())
            .Returns(new List<Block> { parentBlock });
        _repositoryMock
            .Setup(x => x.AddBlock(It.IsAny<Block>()))
            .Throws(new BlockchainException(ErrorCode.InvalidIndex));
        _validatorMock
            .Setup(x => x.IsBlockValid(It.IsAny<Block>(), It.IsAny<Block>(), It.IsAny<string>()))
            .Returns(true);
        _validatorMock
            .Setup(x => x.IsBlockBetterThanTween(It.IsAny<Block>(), It.IsAny<Block>()))
            .Returns(false);

        // act
        var fun = () => blockchain.AddBlock(block);
        
        // assert
        fun.Should().Throw<BlockchainException>().And.ErrorCode.Should().Be(ErrorCode.InvalidIndex);
    }

    [Fact]
    public void AddBlock_ReplaceBlockWithInvalidIndex_ShouldBeOk()
    {
        // arrange
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        var genesis = new Block(0, null);
        var block1 = new Block(genesis.Index + 1, genesis.Hash);
        var block2 = new Block(genesis.Index + 1, genesis.Hash);
        
        _repositoryMock
            .Setup(x => x.GetBlocksCount())
            .Returns(2);
        _repositoryMock
            .Setup(x => x.GetBlock(It.IsAny<int>()))
            .Returns(block1);
        _repositoryMock
            .Setup(x => x.GetBlocks())
            .Returns(new List<Block> { genesis, block1 });
        _repositoryMock
            .Setup(x => x.ReplaceBlock(It.IsAny<int>(), It.IsAny<Block>()))
            .Throws(new BlockchainException(ErrorCode.InvalidIndex));
        _validatorMock
            .Setup(x => x.IsBlockValid(It.IsAny<Block>(), It.IsAny<Block>(), It.IsAny<string>()))
            .Returns(true);
        _validatorMock
            .Setup(x => x.IsBlockBetterThanTween(It.IsAny<Block>(), It.IsAny<Block>()))
            .Returns(true);

        // act
        var fun = () => blockchain.AddBlock(block2);
        
        // assert
        fun.Should().Throw<BlockchainException>().And.ErrorCode.Should().Be(ErrorCode.InvalidIndex);
    }
    
    #endregion

    #region GetBlocksTests

    [Fact]
    public void GetBlocks_Ordinary_ShouldBeOk()
    {
        IBlockchain blockchain = new Blockchain(_repositoryMock.Object, _factoryMock.Object, _validatorMock.Object);
        
        _repositoryMock
            .Setup(x => x.GetBlocks())
            .Returns(new List<Block>());
        
        // act
        var fun = () => blockchain.GetBlocks();
        
        // arrange
        fun.Should().NotThrow();
    }

    #endregion
}