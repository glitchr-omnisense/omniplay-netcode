using Omniplay.Netcode;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

/// <summary>
/// On the NetworkManager's object (its transport UnityTransport), beside OmniplayConnectionApproval
/// and OmniplayNetcodeHeartbeat. A batch-mode build - the server omniplay/docker starts, its secret and
/// room in its environment - starts the server; a player's build connects with the seat the game's
/// backend answered, given here on the command line: -address host:port -ticket eyJ...
/// </summary>
public sealed class OmniplayNetcodeBootstrap : MonoBehaviour
{
    public NetworkManager manager;

    void Start()
    {
        if (manager == null) manager = GetComponent<NetworkManager>();
        if (Application.isBatchMode)
        {
            manager.StartServer();
            return;
        }
        var address = Argument("-address");
        var ticket = Argument("-ticket");
        var colon = address == null ? -1 : address.LastIndexOf(':');
        if (colon < 1 || ticket == null || !ushort.TryParse(address.Substring(colon + 1), out var port))
        {
            Debug.Log("Omniplay sample: start with -address <host:port> -ticket <ticket> to join a server.");
            return;
        }
        OmniplayConnectionApproval.SetTicket(manager, ticket);
        ((UnityTransport)manager.NetworkConfig.NetworkTransport).SetConnectionData(address.Substring(0, colon), port);
        manager.StartClient();
    }

    static string Argument(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == name) return args[i + 1];
        }
        return null;
    }
}
