using System;
using System.Text;

namespace BlockchainLight.Helpers;

public static class RandomGenerator
{
    private const string AvailableChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz#@$^*()";
    
    private static readonly Random Random = new();

    public static int GenerateRandomInt()
    {
        return Random.Next(0, int.MaxValue);
    }

    public static string GenerateRandomString(int length)
    {
        var charSetLength = AvailableChars.Length;
        var sb = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            var nextChar = AvailableChars[Random.Next(charSetLength)];
            sb.Append(nextChar);
        }

        return sb.ToString();
    }
}
