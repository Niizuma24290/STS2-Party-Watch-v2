using System.Collections.Concurrent;

namespace DamageForecast.Diagnostics;

internal static class DamageForecastDiagnostics
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
                $"{MainFile.DiagnosticPrefix}[{code}] {exception.GetType().Name}: {exception.Message}");
        }
        catch
        {
            // Diagnostics must never change forecast behavior.
        }
    }
}
