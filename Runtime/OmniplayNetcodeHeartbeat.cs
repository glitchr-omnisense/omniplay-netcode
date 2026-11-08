using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Omniplay.Netcode
{
    /// <summary>
    /// The server's heartbeat to the application while Netcode's server runs: every few seconds, the
    /// clients connected (but the host's own) and the health the game sets (<see cref="health"/>:
    /// Health.Draining once it takes nobody new). The route, the secret, the server's id and service
    /// come from its environment (OMNIPLAY_HEARTBEAT_URL... as omniplay/docker hands them) unless set in
    /// code; without a route it says so once and sends nothing.
    /// </summary>
    [AddComponentMenu("Netcode/Omniplay Heartbeat")]
    public sealed class OmniplayNetcodeHeartbeat : MonoBehaviour
    {
        [Tooltip("The NetworkManager whose clients are counted; this object's, else the singleton.")]
        public NetworkManager manager;

        [Tooltip("Seconds between heartbeats: under the fleet's 30.")]
        public float every = 10f;

        [Tooltip("What the server says of itself: starting, healthy, draining, down.")]
        public string health = Health.Healthy;

        [Tooltip("The environment variable holding the ticket secret.")]
        public string secretVariable = "OMNIPLAY_TICKET_SECRET";

        /// <summary>The environment's values, or the game's when it sets them before the server starts.</summary>
        public ServerEnvironment Environment { get; set; }

        /// <summary>What registers a server started by hand (its address); null for one a provider started.</summary>
        public Registration Registration { get; set; }

        /// <summary>The application's last answer.</summary>
        public HeartbeatAnswer LastAnswer { get; private set; }

        private HeartbeatClient client;

        private void Awake()
        {
            if (manager == null) manager = GetComponent<NetworkManager>();
            if (manager == null) manager = NetworkManager.Singleton;
        }

        private void OnEnable()
        {
            StartCoroutine(Beat());
        }

        private void OnDisable()
        {
            client?.Dispose();
            client = null;
        }

        /// <summary>The clients connected now, the host's own left out.</summary>
        public int Players() => manager == null ? 0 : manager.ConnectedClientsIds.Count - (manager.IsHost ? 1 : 0);

        private IEnumerator Beat()
        {
            var said = false;
            while (enabled)
            {
                if (manager != null && manager.IsServer)
                {
                    if (client == null)
                    {
                        client = HeartbeatClient.FromEnvironment(Environment ?? (Environment = ServerEnvironment.Read(null, secretVariable)));
                        if (client == null && !said)
                        {
                            Debug.LogWarning("Omniplay: no heartbeat route, secret, server id or service (OMNIPLAY_HEARTBEAT_URL...): no heartbeat is sent.");
                            said = true;
                        }
                    }
                    if (client != null)
                    {
                        Task<HeartbeatAnswer> sending = client.SendAsync(Players(), health, Registration);
                        while (!sending.IsCompleted) yield return null;
                        if (sending.IsFaulted) Debug.LogWarning("Omniplay: the heartbeat failed: " + sending.Exception?.GetBaseException().Message);
                        else
                        {
                            LastAnswer = sending.Result;
                            if (!LastAnswer.Accepted) Debug.LogWarning("Omniplay: the heartbeat was answered " + LastAnswer.Status + ": " + LastAnswer.Body);
                        }
                    }
                }
                yield return new WaitForSeconds(every);
            }
        }
    }
}
