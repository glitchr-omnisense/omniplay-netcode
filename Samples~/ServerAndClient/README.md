# Server and client (omniplay/netcode)

- `OmniplayNetcodeFirstCall`: on any object, logs a ticket the PHP application signed checked by
  omniplay/dotnet's `TicketGate` - `player-42`, `True`, `True`. No server, no account, no key.
- `OmniplayNetcodeBootstrap`: on the NetworkManager's object (transport `UnityTransport`), with
  `OmniplayConnectionApproval` and `OmniplayNetcodeHeartbeat`. A batch-mode build starts the server
  (its secret and room from the environment omniplay/docker hands it); a player's build joins with
  `-address <host:port> -ticket <ticket>`, the seat the game's backend answered.
