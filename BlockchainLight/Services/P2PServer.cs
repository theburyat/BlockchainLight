using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BlockchainLight.CommandLine;
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
        _webSocketServer = new WebSocketServer($"ws://127.0.0.1:{CmdContext.Port}");
        _webSocketServer.AddWebSocketService<P2PServer>("/blockchain");
        _webSocketClient = new P2PClient(new [] { CmdContext.NodePort2, CmdContext.NodePort3 });
        _blockchain = new Blockchain(new BlockRepository(), new BlockFactory(), new BlockValidator());
        _ctSource = new CancellationTokenSource();
    }
    
    public async Task StartAsync()
    {
        _webSocketServer.Start();

        Console.WriteLine($"Started on {_webSocketServer.Address}:{_webSocketServer.Port}");
        
        if (CmdContext.NeedToCreateGenesis)
        {
            await _blockchain.InitializeGenesisAsync(_ctSource.Token);

            var genesis = _blockchain.GetGenesis();
            Console.WriteLine($"Genesis block with hash: {genesis.Hash} index: {genesis.Index} time: {genesis.TimeStamp}");
            _webSocketClient.BroadcastSend(JsonSerializer.Serialize(genesis));

            RunMiningProcess();
        }
        else
        {
            Console.WriteLine("Waiting for genesis");
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

        if (block.Index >= _blockchain.GetBlocks().Count)
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
            while (true)
            {
                await Task.Run(async () =>
                {
                    var newBlock = await _blockchain.MineAsync(_ctSource.Token);
                    _blockchain.AddBlock(newBlock);
                    _webSocketClient.BroadcastSend(JsonSerializer.Serialize(newBlock));

                    Console.WriteLine($"Mine block with hash: {newBlock.Hash} index: {newBlock.Index} time: {newBlock.TimeStamp}");
                }, _ctSource.Token);
            }
            // ReSharper disable once FunctionNeverReturns
        });
    }
}