using System;
using BlockchainLight.Interfaces;
using BlockchainLight.Services;

namespace BlockchainLight
{
    class Program
    {
        static void Main(string[] args)
        {
            IBlockchain blockChain = new Blockchain();
            blockChain.InitializeChain();
            blockChain.InitializeGenesis();

            var next = blockChain.Mine();
            blockChain.AddBlock(next);
            
            next = blockChain.Mine();
            blockChain.AddBlock(next);
            
            next = blockChain.Mine();
            blockChain.AddBlock(next);

            foreach (var block in blockChain.GetBlocks())
            {
                Console.WriteLine($"{block.PreviousHash}    {block.Hash}");
            }
        }
    }
}