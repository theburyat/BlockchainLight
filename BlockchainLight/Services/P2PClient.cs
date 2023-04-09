using System.Collections.Generic;
using BlockchainLight.Interfaces;
using WebSocketSharp;

namespace BlockchainLight.Services;

public class P2PClient: IP2PClient
{
    private readonly IDictionary<string, WebSocket> _nodes;

    public P2PClient(IReadOnlyCollection<int> nodesPorts)
    {
        _nodes = new Dictionary<string, WebSocket>();
        
        foreach (var port in nodesPorts)
        {
            var url = $"ws://127.0.0.1:{port}/blockchain";
            var webSocket = new WebSocket(url);
            
            _nodes.Add(url, webSocket);
        }
    }
    
    public void BroadcastSend(string data)
    {
        foreach (var node in _nodes)
        {
            if (!node.Value.IsAlive)
            {
                node.Value.Connect();
            }

            if (node.Value.Ping())
            {
                node.Value.Send(data);
            }
        }
    }
}