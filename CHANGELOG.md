# Changelog

## [Unreleased]

## [1.0.0] - 2026-11-13

The first release (the 0.1.0 prepared before was never published).

### Added
- `io.omniplay.netcode`, assembly `Omniplay.Netcode`: `OmniplayConnectionApproval` (the ticket in
  `ConnectionData`, one ticket one connection, the host approved, a reason when refused),
  `OmniplayNetcodeHeartbeat`. Depends on `io.omniplay.dotnet` 1.0.0 and
  `com.unity.netcode.gameobjects` 1.12.0.
- The sample **Server and client** (`Samples~/ServerAndClient`): `OmniplayNetcodeFirstCall`, the
  README's first call (a ticket PHP signed, checked by `TicketGate`), and `OmniplayNetcodeBootstrap`
  (the server in batch mode, a client joining with `-address host:port` and `-ticket`).
- The manifest's documentation, changelog and licence URLs.
- `Tests~/Compile`: the package, its sample and omniplay/dotnet compiled against stubs of
  UnityEngine and Netcode for GameObjects (or a real `UnityEngine.CoreModule.dll`), the first call
  run. CI (that, the manifest, the metas, the asmdefs) and a release workflow on a tag (the GitHub
  release with the UPM tarball).
