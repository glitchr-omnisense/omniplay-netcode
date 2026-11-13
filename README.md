# omniplay/netcode

[omnisense/omniplay](https://github.com/glitchr-omnisense/omniplay) inside a Unity server running
Netcode for GameObjects: the UPM package `io.omniplay.netcode`.

- **`OmniplayConnectionApproval`**: sets Netcode's connection approval while enabled (Netcode takes
  one callback). The client puts its ticket in `NetworkConfig.ConnectionData`
  (`OmniplayConnectionApproval.SetTicket(manager, ticket)`) before connecting; the server approves
  it when the ticket holds for this server's room with its secret and was not used before
  (omniplay/dotnet's `TicketGate`), keeps it by client id (`Tickets`, forgotten when the client
  leaves), and refuses it otherwise with a `Reason`. The host's own client is approved. Events
  `Admitted` and `Refused`; `Approve()` is public for a game that chains its own checks.
- **`OmniplayNetcodeHeartbeat`**: while Netcode's server runs, the heartbeat to the application every
  `every` seconds (10): the clients connected but the host's own, and `health` (set it to
  `Health.Draining` once the server takes nobody new).

The secret, the room (the server's id), the heartbeat route and the service come from the server's
environment - `OMNIPLAY_TICKET_SECRET`, `OMNIPLAY_ROOM`, `OMNIPLAY_HEARTBEAT_URL`... as
omniplay/docker hands them - unless set in code (`Secret`, `Room`, `Environment`). A server started
by hand gives its address through `Registration` and is registered by its first heartbeat.

```csharp
// The client, with what the application answered when the player asked to play:
OmniplayConnectionApproval.SetTicket(NetworkManager.Singleton, seat.ticket);
transport.SetConnectionData(host, port);   // UnityTransport; seat.address is "host:port"
NetworkManager.Singleton.StartClient();
```

## Install and a first call

It depends on `com.unity.netcode.gameobjects` (1.12.0 or later: the approval API is the same up to
3.2.1) and on omniplay/dotnet (`io.omniplay.dotnet`, the assembly `Omniplay`). Netcode comes from
Unity's registry; omniplay/dotnet, installed from Git, is named in the project's manifest beside this
package, the first satisfying the second's dependency (Unity resolves no Git dependency of a package
by itself):

```json
"io.omniplay.dotnet": "https://github.com/glitchr-omnisense/omniplay-dotnet.git#v1.0.0",
"io.omniplay.netcode": "https://github.com/glitchr-omnisense/omniplay-netcode.git#v1.0.0"
```

Its assembly `Omniplay.Netcode` also compiles only under `OMNIPLAY_NETCODE`, which its asmdef's
version define sets when `com.unity.netcode.gameobjects` 1.12.0 or later is installed.

`#v1.0.0` pins the release (`#1.x` follows the branch); Unity 2021.3 or later. The package's sample
(Package Manager, the package, Samples, **Server and client**) holds the first call below and a
bootstrap that starts the server in batch mode or joins one. The first call needs no server, no
account, no key and no network: a ticket the PHP application signed (omniplay's `Tickets`, a
recorded exchange from its vectors) checked by omniplay/dotnet's `TicketGate`, which the package
lets players in through. Put it on any object and press Play:

```csharp
using Omniplay;
using UnityEngine;

public sealed class OmniplayNetcodeFirstCall : MonoBehaviour
{
    // A ticket the PHP application signed (omnisense/omniplay's Tickets, recorded in its vectors):
    const string FromPhp = "eyJzdWIiOiJwbGF5ZXItNDIiLCJuYW1lIjoiQWRhIiwicm9vbSI6ImV1LTEtYTNmOSIsImV4cCI6NDEwMjQ0NDgwMH0.0bb9f63e3f6c9d7237c18855738470bae0f54410d4d1177ee87cc65c4608a57c";
    const string Secret = "s3cr3t-shared-with-the-servers";   // OMNIPLAY_TICKET_SECRET on a real server

    void Start()
    {
        var gate = new TicketGate(Secret, "eu-1-a3f9");   // what lets players in on the server eu-1-a3f9
        Debug.Log(gate.Admit(FromPhp)?.Player);           // player-42: let in
        Debug.Log(gate.Admit(FromPhp) == null);           // True: one ticket, one connection
        Debug.Log(new TicketGate(Secret, "eu-1-b7c2").Admit(FromPhp) == null);   // True: another server refuses it
    }
}
```

## Verified, and not

No Unity editor was opened for this release: the package is verified statically and by compiling it.

- Verified (2026-11-13, .NET SDK 10.0.404 in a one-off container): the package's sources, its
  sample's and omniplay/dotnet 1.0.0's compiled together, netstandard2.1, warnings as errors - 0
  errors, 0 warnings - against Unity 6000.3's `UnityEngine.CoreModule.dll` and a stub of Netcode for GameObjects
  whose signatures are copied from com.unity.netcode.gameobjects 1.12.0 (the approval API the same in 3.2.1: `NetworkManager`'s callback, request and response, `NetworkConfig`, `UnityTransport.SetConnectionData`) (`Tests~/Compile`); compiled again against the
  UnityEngine stub, the sample's first call run: `player-42`, `True`, `True`. The manifest, the
  changelog's section, every asset's `.meta` and unique GUIDs, the asmdefs' references: checked.
  The ticket's checks are omniplay/dotnet's, tested on omniplay's vectors (38 tests).
- The CI does the same on GitHub's runners, against the UnityEngine stub only (no Unity there).
- Not verified: inside a Unity editor (the package was not imported, the sample not run in Play
  mode), a client connecting to a server, the heartbeat answered by an application running
  omniplay's `Heartbeats`.

## Releasing

A pushed tag `vX.Y.Z` equal to `package.json`'s version runs `.github/workflows/release.yml`: the CI
on the tag, the changelog's section checked, the GitHub release with the UPM tarball (`npm pack`).
Projects install the tag by Git URL (`#vX.Y.Z`). Tag omniplay/dotnet's same version first: this
package depends on it.

## Licence

MIT.
