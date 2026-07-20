using System.Collections.Concurrent;

namespace STS2PartyWatch.Diagnostics;

internal static class PartyWatchDiagnostics
{
    private static readonly ConcurrentDictionary<string, byte> ReportedCodes = new(StringComparer.Ordinal);

    public static bool TryMarkReported(string code)
    {
        return !string.IsNullOrWhiteSpace(code) && ReportedCodes.TryAdd(code, 0);
    }

    public static void ReportOnce(string code, Exception exception)
    {
        if (!TryMarkReported(code))
        {
            return;
        }

        try
        {
            Console.Error.WriteLine(
                $"[STS2 Party Watch][{code}] {exception.GetType().Name}: {exception.Message}");
        }
        catch
        {
            // Diagnostics must never change forecast behavior.
        }
    }
}
