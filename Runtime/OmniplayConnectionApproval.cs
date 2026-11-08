using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace Omniplay.Netcode
{
    /// <summary>
    /// Netcode for GameObjects' connection approval for omnisense/omniplay's tickets. The client puts
    /// its ticket in NetworkConfig.ConnectionData (<see cref="SetTicket"/>) before connecting; the
    /// server approves it when the ticket holds for this server's room with its secret and was not used
    /// before (TicketGate), keeps the ticket by client id (<see cref="Tickets"/>, forgotten when the
    /// client leaves), and refuses it otherwise with a reason. The host's own client is approved. The
    /// secret and room come from the server's environment (OMNIPLAY_TICKET_SECRET, OMNIPLAY_ROOM: what
    /// omniplay/docker hands it) unless set in code. Netcode takes one approval callback: this
    /// component sets it while enabled.
    /// </summary>
    [AddComponentMenu("Netcode/Omniplay Connection Approval")]
    public sealed class OmniplayConnectionApproval : MonoBehaviour
    {
        [Tooltip("The NetworkManager to approve for; this object's, else the singleton.")]
        public NetworkManager manager;

        [Tooltip("The environment variable holding the ticket secret.")]
        public string secretVariable = "OMNIPLAY_TICKET_SECRET";

        [Tooltip("Whether an approved client gets its player object.")]
        public bool createPlayerObject = true;

        private readonly Dictionary<ulong, Ticket> tickets = new Dictionary<ulong, Ticket>();
        private Action<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse> callback;
        private TicketGate gate;

        /// <summary>The ticket secret, when not the environment's.</summary>
        public string Secret { get; set; }

        /// <summary>The room tickets must name - this server's id - when not the environment's.</summary>
        public string Room { get; set; }

        /// <summary>The tickets of the clients let in, by client id.</summary>
        public IReadOnlyDictionary<ulong, Ticket> Tickets => tickets;

        /// <summary>A client let in, with its ticket.</summary>
        public event Action<ulong, Ticket> Admitted;

        /// <summary>A client refused.</summary>
        public event Action<ulong> Refused;

        /// <summary>The client's side: the ticket sent with the connection.</summary>
        public static void SetTicket(NetworkManager manager, string ticket)
        {
            manager.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(ticket ?? "");
        }

        private void Awake()
        {
            if (manager == null) manager = GetComponent<NetworkManager>();
            if (manager == null) manager = NetworkManager.Singleton;
        }

        private void OnEnable()
        {
            if (manager == null)
            {
                Debug.LogError("Omniplay: no NetworkManager to approve for.");
                return;
            }
            var environment = ServerEnvironment.Read(null, secretVariable);
            gate = new TicketGate(Secret ?? environment.Secret, Room ?? environment.Room);
            callback = Approve;
            manager.NetworkConfig.ConnectionApproval = true;
            manager.ConnectionApprovalCallback = callback;
            manager.OnClientDisconnectCallback += Forget;
        }

        private void OnDisable()
        {
            if (manager == null) return;
            if (manager.ConnectionApprovalCallback == callback) manager.ConnectionApprovalCallback = null;
            manager.OnClientDisconnectCallback -= Forget;
        }

        /// <summary>Netcode's approval callback (set by OnEnable; public for a game that chains its own).</summary>
        public void Approve(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            response.Pending = false;
            response.CreatePlayerObject = createPlayerObject;
            if (request.ClientNetworkId == NetworkManager.ServerClientId)
            {
                response.Approved = true;
                return;
            }
            if (gate == null || !gate.Configured)
            {
                Debug.LogError("Omniplay: no ticket secret or room (" + secretVariable + ", OMNIPLAY_ROOM): every client is refused.");
            }
            string token;
            try
            {
                token = request.Payload != null ? new UTF8Encoding(false, true).GetString(request.Payload) : "";
            }
            catch (ArgumentException)
            {
                token = "";
            }
            var admitted = gate?.Admit(token);
            response.Approved = admitted != null;
            if (admitted != null)
            {
                tickets[request.ClientNetworkId] = admitted;
                Admitted?.Invoke(request.ClientNetworkId, admitted);
                return;
            }
            response.CreatePlayerObject = false;
            response.Reason = "The ticket does not hold for this server.";
            Refused?.Invoke(request.ClientNetworkId);
        }

        private void Forget(ulong client)
        {
            tickets.Remove(client);
        }
    }
}
