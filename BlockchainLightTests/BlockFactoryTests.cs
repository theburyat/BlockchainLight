using BlockchainLight.Entities.Enums;
using BlockchainLight.Exceptions;
using BlockchainLight.Interfaces;
using BlockchainLight.Services;
using FluentAssertions;
using Xunit;

namespace BlockchainLightTests;

public class BlockFactoryTests
{
    private readonly IBlockFactory _factory = new BlockFactory();
    
    [Fact]
    public async Task CreateBlock_Genesis_ShouldBeOkAsync()
    {
        // arrange
        
        // act
        var block = await _factory.CreateBlockAsync(0, null, "0=", CancellationToken.None);

        // assert
        block.Should().NotBeNull();
        block.Data.Should().NotBeNull();
        block.TimeStamp.Should().NotBe(default);
        block.Index.Should().Be(0);
        block.PreviousHash.Should().Be(null);
        block.Hash.Should().NotBeNull().And.EndWith("0=");
    }

    [Theory]
    [InlineData(1, "qweqwe", "0=")]
    [InlineData(2, "zxczxc", "0=")]
    [InlineData(9, "rtyrty", "0=")]
    public async Task CreateBlock_Ordinary_ShouldBeOk(int index, string previousHash, string hashEndRestriction)
    {
        // arrange
        
        // act
        var block = await _factory.CreateBlockAsync(index, previousHash, hashEndRestriction, CancellationToken.None);
        
        // assert
        block.Should().NotBeNull();
        block.Data.Should().NotBeNull();
        block.TimeStamp.Should().NotBe(default);
        block.Index.Should().Be(index);
        block.PreviousHash.Should().Be(previousHash);
        block.Hash.Should().NotBeNull().And.EndWith(hashEndRestriction);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-2)]
    [InlineData(-9)]
    public async Task CreateBlock_InvalidIndex_ShouldNotBeOk(int index)
    {
        // arrange
        var task = () => _factory.CreateBlockAsync(index, "zxczxc", "0=", CancellationToken.None);

        // act & assert
        (await task.Should().ThrowAsync<BlockchainException>()).And.ErrorCode.Should().Be(ErrorCode.InvalidIndex);
    }
    
    [Theory]
    [InlineData(1, null)]
    [InlineData(2, "")]
    [InlineData(3, "    ")]
    public async Task CreateBlock_InvalidParentHash_ShouldNotBeOk(int index, string parentHash)
    {
        // arrange
        var task = () => _factory.CreateBlockAsync(index, parentHash, "0=", CancellationToken.None);

        // act & assert
        (await task.Should().ThrowAsync<BlockchainException>()).And.ErrorCode.Should().Be(ErrorCode.InvalidParentHash);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("zxczxc")]
    public async Task CreateBlock_InvalidHashEndRestriction_ShouldNotBeOk(string hashEndRestriction)
    {
        // arrange
        var task = () => _factory.CreateBlockAsync(1, "qweqwe", hashEndRestriction, CancellationToken.None);

        // act & assert
        (await task.Should().ThrowAsync<BlockchainException>()).And.ErrorCode.Should().Be(ErrorCode.InvalidHashEndRestriction);
    }
}