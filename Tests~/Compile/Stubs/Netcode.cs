// A stub of the Netcode for GameObjects API this package and its sample use, the signatures copied
// from com.unity.netcode.gameobjects 1.12.0 (the same in 3.2.1): NetworkManager's connection approval,
// its request and response, NetworkConfig, UnityTransport's SetConnectionData. It lets the package
// compile without Unity (the CI). Not shipped: Unity ignores Tests~.
using System;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0067 // the events a stub never raises

namespace Unity.Netcode
{
    public abstract class NetworkTransport : MonoBehaviour { }

    public class NetworkConfig
    {
        public byte[] ConnectionData = new byte[0];
        public bool ConnectionApproval;
        public NetworkTransport NetworkTransport;
    }

    public class NetworkManager : MonoBehaviour
    {
        public const ulong ServerClientId = 0;

        public static NetworkManager Singleton { get; private set; }

        public NetworkConfig NetworkConfig = new NetworkConfig();

        public Action<ConnectionApprovalRequest, ConnectionApprovalResponse> ConnectionApprovalCallback { get; set; }

        public event Action<ulong> OnClientDisconnectCallback;

        public event Action<ulong> OnClientConnectedCallback;

        public IReadOnlyList<ulong> ConnectedClientsIds => new List<ulong>();

        public bool IsServer { get; }

        public bool IsHost { get; }

        public bool IsClient { get; }

        public bool StartServer() => true;

        public bool StartClient() => true;

        public bool StartHost() => true;

        public struct ConnectionApprovalRequest
        {
            public byte[] Payload;
            public ulong ClientNetworkId;
        }

        public class ConnectionApprovalResponse
        {
            public bool Approved;
            public bool CreatePlayerObject;
            public uint? PlayerPrefabHash;
            public Vector3? Position;
            public Quaternion? Rotation;
            public bool Pending;
            public string Reason;
        }
    }
}

namespace Unity.Netcode.Transports.UTP
{
    public class UnityTransport : Unity.Netcode.NetworkTransport
    {
        public void SetConnectionData(string ipv4Address, ushort port, string listenAddress = null) { }
    }
}
