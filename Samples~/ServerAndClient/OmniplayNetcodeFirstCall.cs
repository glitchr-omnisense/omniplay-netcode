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
