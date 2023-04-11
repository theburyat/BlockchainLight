using BlockchainLight.Entities;
using BlockchainLight.Interfaces;
using BlockchainLight.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlockchainLightTests;

public class BlockchainTests
{
    [Fact]
    public async Task InitializeGenesisAsync_Ordinary_ShouldBeOk()
    {
        // arrange
        var repositoryMock = new Mock<IBlockRepository>();
        var factoryMock = new Mock<IBlockFactory>();
        var validatorMock = new Mock<IBlockValidator>();

        IBlockchain blockchain = new Blockchain(repositoryMock.Object, factoryMock.Object, validatorMock.Object);

        factoryMock
            .Setup(x => x.CreateBlockAsync(0, null, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new Block(0, null)));
        repositoryMock
            .Setup(x => x.AddGenesis(It.IsAny<Block>()))
            .Verifiable();
        
        // act
        var task = () => blockchain.InitializeGenesisAsync(CancellationToken.None);
        
        // arrange
        await task.Should().NotThrowAsync();
    }
}