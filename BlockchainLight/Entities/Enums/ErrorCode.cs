namespace BlockchainLight.Entities.Enums;

public enum ErrorCode
{
    None,
    AlreadyHasGenesis,
    CanNotConnectToServer,
    InvalidBlock,
    InvalidData,
    InvalidHash,
    InvalidHashEndRestriction,
    InvalidIndex,
    InvalidParentHash,
    InvalidTimestamp,
    NoBlock,
    NoGenesis
}