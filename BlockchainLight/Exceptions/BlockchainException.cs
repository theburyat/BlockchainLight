using System;
using BlockchainLight.Entities.Enums;

namespace BlockchainLight.Exceptions;

public class BlockchainException: Exception
{
    public ErrorCode ErrorCode { get; }

    public BlockchainException(ErrorCode errorCode)
    {
        ErrorCode = errorCode;
    }

    public BlockchainException(ErrorCode errorCode, string message): base(message)
    {
        ErrorCode = errorCode;
    }
}