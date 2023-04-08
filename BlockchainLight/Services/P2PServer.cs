using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BlockchainLight.Entities;
using BlockchainLight.Interfaces;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace BlockchainLight.Services;

public class P2PServer: WebSocketBehavior, IP2PServer
{
    private readonly WebSocketServer _webSocketServer;
    private readonly IP2PClient _webSocketClient;
    private readonly IBlockchain _blockchain;

    private CancellationTokenSource _ctSource;

    public P2PServer()
    {
        _webSocketServer = new WebSocketServer($"ws://127.0.0.1:{ConsoleArguments.Port}");
        _webSocketServer.AddWebSocketService<P2PServer>("/blockchain");
        _webSocketClient = new P2PClient(new[] {ConsoleArguments.Port2/*, ConsoleArguments.Port3*/ });
        _blockchain = new Blockchain(new StaticBlockRepository(), new BlockFactory());
        _ctSource = new CancellationTokenSource();
    }
    
    public async Task Start(bool needToCreateGenesis = false)
    {
        _webSocketServer.Start();

        Console.WriteLine($"Started on {_webSocketServer.Address}:{_webSocketServer.Port}");
        
        if (needToCreateGenesis)
        {
            _blockchain.InitializeGenesis();

            var genesis = _blockchain.GetGenesis();
            Console.WriteLine($"Genesis block with hash: {genesis.Hash} index: {genesis.Index} time: {genesis.TimeStamp}");
            _webSocketClient.BroadcastSend(JsonSerializer.Serialize(genesis));

            RunMiningProcess();
        }

        while (_webSocketServer.IsListening)
        {
            await Task.Delay(1000);
        }
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        var block = JsonSerializer.Deserialize<Block>(e.Data);

        if (block is null)
        {
            return;
        }

        Console.WriteLine($"Receive block with hash: {block.Hash} index: {block.Index} time: {block.TimeStamp}");

        if (block.Index > _blockchain.GetBlocks().Count)
        {
            _ctSource.Cancel();
            _ctSource = new CancellationTokenSource();
        }

        if (block.PreviousHash is null)
        {
            _blockchain.AddGenesis(block);
            RunMiningProcess();
        }
        else
        {
            _blockchain.AddBlock(block);
        }
    }

    private void RunMiningProcess()
    {
        Task.Run(async () =>
        {
            while (_webSocketServer.IsListening)
            {
                await Task.Run(() =>
                {
                    var newBlock = _blockchain.Mine();
                    _blockchain.AddBlock(newBlock);
                    _webSocketClient.BroadcastSend(JsonSerializer.Serialize(newBlock));
            
                    Console.WriteLine($"Mine block with hash: {newBlock.Hash} index: {newBlock.Index} time: {newBlock.TimeStamp}");
                }, _ctSource.Token);
            }
        });
    }
}