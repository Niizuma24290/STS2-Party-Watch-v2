internal sealed class ContractAssert
{
    public void Equal<T>(T expected, T actual, string? context = null)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new ContractAssertionException(
                FormatValue(expected),
                FormatValue(actual),
                context);
        }
    }

    public void True(
        bool condition,
        string expected,
        string actual,
        string? context = null)
    {
        if (!condition)
        {
            throw new ContractAssertionException(expected, actual, context);
        }
    }

    private static string FormatValue<T>(T value)
    {
        return value switch
        {
            null => "<null>",
            string text => $"\"{text}\"",
            IFormattable formattable => formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
            _ => value.ToString() ?? "<null>"
        };
    }
}

internal sealed class ContractAssertionException(
    string expected,
    string actual,
    string? context) : Exception("Contract assertion failed.")
{
    public string Expected { get; } = expected;

    public string Actual { get; } = actual;

    public string? Context { get; } = context;
}
