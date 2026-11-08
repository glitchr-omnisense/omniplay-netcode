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

## Installing

It depends on `com.unity.netcode.gameobjects` (1.12.0 or later: the approval API is the same up to
3.2.1) and on omniplay/dotnet (`io.omniplay.dotnet`, the assembly `Omniplay`). Netcode comes from
Unity's registry; omniplay/dotnet, installed from Git, is named in the project's manifest beside this
package, the first satisfying the second's dependency (Unity resolves no Git dependency of a package
by itself):

```json
"io.omniplay.dotnet": "https://github.com/glitchr-omnisense/omniplay-dotnet.git#1.x",
"io.omniplay.netcode": "https://github.com/glitchr-omnisense/omniplay-netcode.git#1.x"
```

Its assembly `Omniplay.Netcode` also compiles only under `OMNIPLAY_NETCODE`, which its asmdef's
version define sets when `com.unity.netcode.gameobjects` 1.12.0 or later is installed.

## Verified, and not

- Verified (2026-11-08): the package's sources compiled with omniplay/dotnet's, against Unity
  6000.3's `UnityEngine.CoreModule.dll` and a stub of Netcode whose signatures are copied from
  com.unity.netcode.gameobjects 1.12.0 and 3.2.1 (`NetworkManager`'s approval callback, request and
  response, `NetworkConfig`; the same in both), netstandard2.1, warnings as errors: 0 errors, 0
  warnings. The ticket's checks are omniplay/dotnet's, tested on omniplay's vectors (`TicketGate`,
  36 tests).
- Not verified: inside a Unity editor (the package was not imported), a client connecting to a
  server.

## Licence

MIT.
