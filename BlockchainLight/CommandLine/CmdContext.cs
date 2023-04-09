namespace BlockchainLight.CommandLine;

public static class CmdContext
{
    public static int Port { get; set; }
    
    public static int NodePort2 { get; set; }
    
    public static int NodePort3 { get; set; }

    public static bool NeedToCreateGenesis { get; set; }
}