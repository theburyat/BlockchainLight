using BlockchainLight.Entities;
using BlockchainLight.Interfaces;
using BlockchainLight.Services;
using FluentAssertions;
using Xunit;

namespace BlockchainLightTests;

public class BlockValidatorTests
{
    private readonly IBlockFactory _factory = new BlockFactory();
    private readonly IBlockValidator _validator = new BlockValidator();

    [Theory]
    [InlineData("0=")]
    public async Task IsBlockValid_Valid_ShouldBeOkAsync(string hashEndRestricting)
    {
        // arrange
        var parent = await _factory.CreateBlockAsync(0, null, hashEndRestricting, CancellationToken.None);
        var block = await _factory.CreateBlockAsync(parent.Index, parent.Hash, hashEndRestricting, CancellationToken.None);

        // act
        var result = _validator.IsBlockValid(block, parent, hashEndRestricting);

        // assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData("0=")]
    public async Task IsBlockValid_InvalidHash_ShouldNotBeOkAsync(string hashEndRestricting)
    {
        // arrange
        var parent = await _factory.CreateBlockAsync(0, null, hashEndRestricting, CancellationToken.None);
        var block = await _factory.CreateBlockAsync(parent.Index, parent.Hash, hashEndRestricting, CancellationToken.None);
        
        block.Hash = $"zxczxczxc{hashEndRestricting}";

        // act
        var result = _validator.IsBlockValid(block, parent, hashEndRestricting);

        // assert
        result.Should().BeFalse();
    }
    
    [Theory]
    [InlineData("0=")]
    public async Task IsBlockValid_InvalidParentHash_ShouldNotBeOkAsync(string hashEndRestricting)
    {
        // arrange
        var parent = await _factory.CreateBlockAsync(0, null, hashEndRestricting, CancellationToken.None);
        var block = await _factory.CreateBlockAsync(parent.Index, parent.Hash, hashEndRestricting, CancellationToken.None);
        
        parent.Nonce += 1;
        parent.Hash = parent.CalculateHash();

        // act
        var result = _validator.IsBlockValid(block, parent, hashEndRestricting);

        // assert
        result.Should().BeFalse();
    }
    
    [Theory]
    [InlineData("0=")]
    public async Task IsBlockValid_InvalidData_ShouldNotBeOkAsync(string hashEndRestricting)
    {
        // arrange
        var parent = await _factory.CreateBlockAsync(0, null, hashEndRestricting, CancellationToken.None);
        var block = await _factory.CreateBlockAsync(parent.Index, parent.Hash, hashEndRestricting, CancellationToken.None);

        block.Data = "zxczxczxc";

        // act
        var result = _validator.IsBlockValid(block, parent, hashEndRestricting);

        // assert
        result.Should().BeFalse();
    }
    
    [Theory]
    [InlineData("0=")]
    public async Task IsBlockValid_InvalidTimestamp_ShouldNotBeOkAsync(string hashEndRestricting)
    {
        // arrange
        var parent = await _factory.CreateBlockAsync(0, null, hashEndRestricting, CancellationToken.None);
        var block = await _factory.CreateBlockAsync(parent.Index, parent.Hash, hashEndRestricting, CancellationToken.None);
        block.TimeStamp = default;

        // act
        var result = _validator.IsBlockValid(block, parent, hashEndRestricting);

        // assert
        result.Should().BeFalse();
    }
    
    [Theory]
    [InlineData("0=")]
    public async Task IsBlockBetterThanTween_FirstIsBetter_ShouldBeOkAsync(string hashEndRestricting)
    {
        // arrange
        var parent = await _factory.CreateBlockAsync(0, null, hashEndRestricting, CancellationToken.None);
        var block1 = await _factory.CreateBlockAsync(parent.Index, parent.Hash, hashEndRestricting, CancellationToken.None);
        var block2 = await _factory.CreateBlockAsync(parent.Index, parent.Hash, hashEndRestricting, CancellationToken.None);

        // act
        var result = _validator.IsBlockBetterThanTween(block1, block2);

        // assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData("0=")]
    public async Task IsBlockBetterThanTween_FirstIsWorse_ShouldBeOkAsync(string hashEndRestricting)
    {
        // arrange
        var parent = await _factory.CreateBlockAsync(0, null, hashEndRestricting, CancellationToken.None);
        var block2 = await _factory.CreateBlockAsync(parent.Index, parent.Hash, hashEndRestricting, CancellationToken.None);
        var block1 = await _factory.CreateBlockAsync(parent.Index, parent.Hash, hashEndRestricting, CancellationToken.None);

        // act
        var result = _validator.IsBlockBetterThanTween(block1, block2);

        // assert
        result.Should().BeFalse();
    }
}