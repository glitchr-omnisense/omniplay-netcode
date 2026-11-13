// The sample's first call, run outside Unity against the UnityEngine stub (Debug.Log writes to the
// console): Unity would call Start() itself.
using System.Reflection;

internal static class Program
{
    private static void Main()
    {
        var sample = new OmniplayNetcodeFirstCall();
        typeof(OmniplayNetcodeFirstCall).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(sample, null);
    }
}
