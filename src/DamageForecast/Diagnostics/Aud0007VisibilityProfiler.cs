#if DAMAGE_FORECAST_VISIBILITY_PROFILING
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Godot;
using HarmonyLib;

namespace DamageForecast.Diagnostics;

internal enum VisibilityPostfixKind
{
    Show,
    Hide,
    SetVisible,
}

internal readonly record struct Aud0007VisibilitySnapshot(
    long WindowTicks,
    long ShowCalls,
    long HideCalls,
    long SetVisibleCalls,
    long TotalCalls,
    long UnmatchedCalls,
    long MatchedCalls,
    long Transitions,
    long Duplicates,
    long Refreshes,
    long BarsVisited,
    long TypeCheckTotalTicks,
    long TypeCheckMaxTicks,
    long CallbackTotalTicks,
    long CallbackMaxTicks,
    long RefreshTotalTicks,
    long RefreshMaxTicks);

internal sealed class VisibilityPerformanceAccumulator
{
    private long _showCalls;
    private long _hideCalls;
    private long _setVisibleCalls;
    private long _totalCalls;
    private long _unmatchedCalls;
    private long _matchedCalls;
    private long _transitions;
    private long _duplicates;
    private long _refreshes;
    private long _barsVisited;
    private long _typeCheckTotalTicks;
    private long _typeCheckMaxTicks;
    private long _callbackTotalTicks;
    private long _callbackMaxTicks;
    private long _refreshTotalTicks;
    private long _refreshMaxTicks;

    public void Record(
        VisibilityPostfixKind kind,
        bool matched,
        bool transitioned,
        long typeCheckTicks,
        long callbackTicks,
        bool refreshed,
        long refreshTicks,
        int barsVisited)
    {
        switch (kind)
        {
            case VisibilityPostfixKind.Show:
                Interlocked.Increment(ref _showCalls);
                break;
            case VisibilityPostfixKind.Hide:
                Interlocked.Increment(ref _hideCalls);
                break;
            case VisibilityPostfixKind.SetVisible:
                Interlocked.Increment(ref _setVisibleCalls);
                break;
        }

        Interlocked.Increment(ref _totalCalls);
        Interlocked.Add(ref _typeCheckTotalTicks, typeCheckTicks);
        Interlocked.Add(ref _callbackTotalTicks, callbackTicks);
        UpdateMaximum(ref _typeCheckMaxTicks, typeCheckTicks);
        UpdateMaximum(ref _callbackMaxTicks, callbackTicks);

        if (matched)
        {
            Interlocked.Increment(ref _matchedCalls);
            if (transitioned)
            {
                Interlocked.Increment(ref _transitions);
            }
            else
            {
                Interlocked.Increment(ref _duplicates);
            }
        }
        else
        {
            Interlocked.Increment(ref _unmatchedCalls);
        }

        if (refreshed)
        {
            Interlocked.Increment(ref _refreshes);
            Interlocked.Add(ref _barsVisited, barsVisited);
            Interlocked.Add(ref _refreshTotalTicks, refreshTicks);
            UpdateMaximum(ref _refreshMaxTicks, refreshTicks);
        }
    }

    public Aud0007VisibilitySnapshot Snapshot(long windowTicks)
    {
        return new Aud0007VisibilitySnapshot(
            windowTicks,
            Interlocked.Read(ref _showCalls),
            Interlocked.Read(ref _hideCalls),
            Interlocked.Read(ref _setVisibleCalls),
            Interlocked.Read(ref _totalCalls),
            Interlocked.Read(ref _unmatchedCalls),
            Interlocked.Read(ref _matchedCalls),
            Interlocked.Read(ref _transitions),
            Interlocked.Read(ref _duplicates),
            Interlocked.Read(ref _refreshes),
            Interlocked.Read(ref _barsVisited),
            Interlocked.Read(ref _typeCheckTotalTicks),
            Interlocked.Read(ref _typeCheckMaxTicks),
            Interlocked.Read(ref _callbackTotalTicks),
            Interlocked.Read(ref _callbackMaxTicks),
            Interlocked.Read(ref _refreshTotalTicks),
            Interlocked.Read(ref _refreshMaxTicks));
    }

    public void Reset()
    {
        Interlocked.Exchange(ref _showCalls, 0);
        Interlocked.Exchange(ref _hideCalls, 0);
        Interlocked.Exchange(ref _setVisibleCalls, 0);
        Interlocked.Exchange(ref _totalCalls, 0);
        Interlocked.Exchange(ref _unmatchedCalls, 0);
        Interlocked.Exchange(ref _matchedCalls, 0);
        Interlocked.Exchange(ref _transitions, 0);
        Interlocked.Exchange(ref _duplicates, 0);
        Interlocked.Exchange(ref _refreshes, 0);
        Interlocked.Exchange(ref _barsVisited, 0);
        Interlocked.Exchange(ref _typeCheckTotalTicks, 0);
        Interlocked.Exchange(ref _typeCheckMaxTicks, 0);
        Interlocked.Exchange(ref _callbackTotalTicks, 0);
        Interlocked.Exchange(ref _callbackMaxTicks, 0);
        Interlocked.Exchange(ref _refreshTotalTicks, 0);
        Interlocked.Exchange(ref _refreshMaxTicks, 0);
    }

    private static void UpdateMaximum(ref long target, long value)
    {
        var current = Interlocked.Read(ref target);
        while (value > current)
        {
            var observed = Interlocked.CompareExchange(ref target, value, current);
            if (observed == current)
            {
                return;
            }

            current = observed;
        }
    }
}

internal static class Aud0007VisibilitySummaryFormatter
{
    public static string Format(string reason, Aud0007VisibilitySnapshot snapshot, long timestampFrequency)
    {
        var windowSeconds = ToSeconds(snapshot.WindowTicks, timestampFrequency);
        var callsPerSecond = Divide(snapshot.TotalCalls, windowSeconds);
        var matchPercentage = Percentage(snapshot.MatchedCalls, snapshot.TotalCalls);
        var duplicatePercentage = Percentage(snapshot.Duplicates, snapshot.MatchedCalls);
        var refreshesPerTransition = Divide(snapshot.Refreshes, snapshot.Transitions);
        var typeTotalMilliseconds = ToMilliseconds(snapshot.TypeCheckTotalTicks, timestampFrequency);
        var typeAverageMicroseconds = AverageMicroseconds(
            snapshot.TypeCheckTotalTicks,
            snapshot.TotalCalls,
            timestampFrequency);
        var callbackTotalMilliseconds = ToMilliseconds(snapshot.CallbackTotalTicks, timestampFrequency);
        var callbackMillisecondsPerSecond = Divide(callbackTotalMilliseconds, windowSeconds);
        var refreshTotalMilliseconds = ToMilliseconds(snapshot.RefreshTotalTicks, timestampFrequency);
        var refreshAverageMicroseconds = AverageMicroseconds(
            snapshot.RefreshTotalTicks,
            snapshot.Refreshes,
            timestampFrequency);

        return string.Format(
            CultureInfo.InvariantCulture,
            "[Damage Forecast][AUD-0007] reason={0} window_s={1:F3} show={2} hide={3} set_visible={4} total={5} unmatched={6} matched={7} transitions={8} duplicates={9} refreshes={10} bars_visited={11} calls_per_s={12:F3} match_pct={13:F3} duplicate_pct={14:F3} refreshes_per_transition={15:F3} type_total_ms={16:F3} type_avg_us={17:F3} type_max_us={18:F3} callback_total_ms={19:F3} callback_ms_per_s={20:F3} callback_max_us={21:F3} refresh_total_ms={22:F3} refresh_avg_us={23:F3} refresh_max_us={24:F3}",
            reason,
            windowSeconds,
            snapshot.ShowCalls,
            snapshot.HideCalls,
            snapshot.SetVisibleCalls,
            snapshot.TotalCalls,
            snapshot.UnmatchedCalls,
            snapshot.MatchedCalls,
            snapshot.Transitions,
            snapshot.Duplicates,
            snapshot.Refreshes,
            snapshot.BarsVisited,
            callsPerSecond,
            matchPercentage,
            duplicatePercentage,
            refreshesPerTransition,
            typeTotalMilliseconds,
            typeAverageMicroseconds,
            ToMicroseconds(snapshot.TypeCheckMaxTicks, timestampFrequency),
            callbackTotalMilliseconds,
            callbackMillisecondsPerSecond,
            ToMicroseconds(snapshot.CallbackMaxTicks, timestampFrequency),
            refreshTotalMilliseconds,
            refreshAverageMicroseconds,
            ToMicroseconds(snapshot.RefreshMaxTicks, timestampFrequency));
    }

    private static double ToSeconds(long ticks, long frequency)
    {
        return frequency <= 0 ? 0d : (double)ticks / frequency;
    }

    private static double ToMilliseconds(long ticks, long frequency)
    {
        return frequency <= 0 ? 0d : ticks * 1000d / frequency;
    }

    private static double ToMicroseconds(long ticks, long frequency)
    {
        return frequency <= 0 ? 0d : ticks * 1_000_000d / frequency;
    }

    private static double AverageMicroseconds(long ticks, long count, long frequency)
    {
        return count <= 0 ? 0d : ToMicroseconds(ticks, frequency) / count;
    }

    private static double Percentage(long value, long total)
    {
        return total <= 0 ? 0d : value * 100d / total;
    }

    private static double Divide(double value, double divisor)
    {
        return divisor <= 0d ? 0d : value / divisor;
    }
}

internal static class Aud0007VisibilityProfiler
{
    private static readonly VisibilityPerformanceAccumulator Accumulator = new();
    private static readonly object SummaryLogLock = new();
    private static System.Threading.Timer? _periodicDumpTimer;
    private static string? _summaryLogPath;
    private static long _windowStartedAt = Stopwatch.GetTimestamp();

    public static void Start()
    {
        try
        {
            var logRoot = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                "SlayTheSpire2",
                "logs");
            Directory.CreateDirectory(logRoot);
            _summaryLogPath = Path.Combine(
                logRoot,
                $"party-watch-aud-0007-{DateTime.UtcNow:yyyyMMddTHHmmssZ}.log");

            var showPatched = GetOwnedPostfixStatus(AccessTools.Method(typeof(CanvasItem), nameof(CanvasItem.Show)));
            var hidePatched = GetOwnedPostfixStatus(AccessTools.Method(typeof(CanvasItem), nameof(CanvasItem.Hide)));
            var setVisiblePatched = GetOwnedPostfixStatus(AccessTools.Method(typeof(CanvasItem), "set_Visible"));
            WriteDiagnosticLine(
                "[Damage Forecast][AUD-0007] probe=started"
                + $" show_patched={showPatched}"
                + $" hide_patched={hidePatched}"
                + $" set_visible_patched={setVisiblePatched}");

            _periodicDumpTimer = new System.Threading.Timer(
                static _ => Dump("periodic", reset: false),
                null,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(30));
        }
        catch
        {
            // Profiling diagnostics must never change mod initialization.
        }
    }

    public static void Record(
        VisibilityPostfixKind kind,
        bool matched,
        bool transitioned,
        long typeCheckTicks,
        long callbackTicks,
        bool refreshed,
        long refreshTicks,
        int barsVisited)
    {
        Accumulator.Record(
            kind,
            matched,
            transitioned,
            typeCheckTicks,
            callbackTicks,
            refreshed,
            refreshTicks,
            barsVisited);
    }

    public static void Dump(string reason, bool reset)
    {
        var now = Stopwatch.GetTimestamp();
        var snapshot = Accumulator.Snapshot(Math.Max(0, now - Interlocked.Read(ref _windowStartedAt)));
        WriteSummary(reason, snapshot);

        if (reset)
        {
            Accumulator.Reset();
            Interlocked.Exchange(ref _windowStartedAt, now);
        }
    }

    private static void WriteSummary(string reason, Aud0007VisibilitySnapshot snapshot)
    {
        try
        {
            WriteDiagnosticLine(Aud0007VisibilitySummaryFormatter.Format(reason, snapshot, Stopwatch.Frequency));
        }
        catch
        {
            // Profiling must never change gameplay behavior.
        }
    }

    private static string GetOwnedPostfixStatus(MethodBase? method)
    {
        if (method is null)
        {
            return "target-missing";
        }

        try
        {
            return Harmony.GetPatchInfo(method)?.Postfixes.Any(patch => patch.owner == MainFile.ModId) == true
                ? "true"
                : "false";
        }
        catch (Exception exception)
        {
            return $"error-{exception.GetType().Name}";
        }
    }

    private static void WriteDiagnosticLine(string line)
    {
        var path = _summaryLogPath;
        if (path is null)
        {
            return;
        }

        lock (SummaryLogLock)
        {
            File.AppendAllText(path, line + System.Environment.NewLine);
        }
    }
}
#endif
