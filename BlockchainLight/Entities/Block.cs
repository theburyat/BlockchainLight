using System;
using System.Security.Cryptography;
using System.Text;
using BlockchainLight.Entities.Interfaces;
using BlockchainLight.Helpers;

namespace BlockchainLight.Entities;

public class Block: IBlock
{
    private const int DefaultDataLength = 256;
    
    private readonly SHA256 _coder = SHA256.Create();
    
    public int Index { get; }

    public string PreviousHash { get; set; }

    public string Hash { get; set; }

    public string Data { get; set; }

    public DateTime TimeStamp { get; set; }

    public int Nonce { get; set; }

    public Block(int index, string previousHash)
    {
        Index = index;
        PreviousHash = previousHash;

        Nonce = RandomGenerator.GenerateRandomInt();
        Data = RandomGenerator.GenerateRandomString(DefaultDataLength);
        Hash = CalculateHash();
    }

    public string CalculateHash()
    {
        var inputBytes = Encoding.ASCII.GetBytes($"{Index}{PreviousHash}{Data}{Nonce}");
        var outputBytes = _coder.ComputeHash(inputBytes);

        return Convert.ToBase64String(outputBytes);
    }

    public void ChangeNonce()
    {
        Nonce = RandomGenerator.GenerateRandomInt();
    }
}
