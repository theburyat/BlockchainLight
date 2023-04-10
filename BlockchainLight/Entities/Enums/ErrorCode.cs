namespace BlockchainLight.Entities.Enums;

public enum ErrorCode
{
    None,
    AlreadyHasGenesis,
    CanNotConnectToServer,
    InvalidBlock,
    InvalidHash,
    InvalidIndex,
    InvalidParentHash,
    NoBlock,
    NoGenesis
}