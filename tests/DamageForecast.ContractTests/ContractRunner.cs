internal static class ContractRunner
{
    private const int MaximumExceptionMessageLength = 500;

    public static int Run(IReadOnlyList<ContractCase> cases)
    {
        if (!TryValidate(cases, out var validationError))
        {
            Console.Error.WriteLine($"RUNNER ERROR: {validationError}");
            return 2;
        }

        var passed = 0;
        var failed = 0;
        var skipped = 0;

        foreach (var testCase in cases)
        {
            if (testCase.SkipReason is { } skipReason)
            {
                skipped++;
                Console.WriteLine(
                    $"SKIP {testCase.Id} category={testCase.Category} name={testCase.Name} reason={skipReason}");
                continue;
            }

            try
            {
                testCase.Execute(new ContractAssert());
                passed++;
                Console.WriteLine($"PASS {testCase.Id} category={testCase.Category} name={testCase.Name}");
            }
            catch (ContractAssertionException exception)
            {
                failed++;
                WriteFailure(testCase, exception.Expected, exception.Actual, exception.Context);
            }
            catch (Exception exception)
            {
                failed++;
                WriteFailure(
                    testCase,
                    "case completes without an exception",
                    $"{exception.GetType().FullName}: {Limit(exception.Message)}",
                    "unexpected case error");
            }
        }

        Console.WriteLine(
            $"SUMMARY discovered={cases.Count} passed={passed} failed={failed} skipped={skipped}");
        return failed == 0 ? 0 : 1;
    }

    private static bool TryValidate(
        IReadOnlyList<ContractCase>? cases,
        out string validationError)
    {
        if (cases is null)
        {
            validationError = "case list is null";
            return false;
        }

        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var testCase in cases)
        {
            if (string.IsNullOrWhiteSpace(testCase.Id))
            {
                validationError = "a case has an empty ID";
                return false;
            }

            if (!ids.Add(testCase.Id))
            {
                validationError = $"duplicate case ID: {testCase.Id}";
                return false;
            }

            if (string.IsNullOrWhiteSpace(testCase.Category))
            {
                validationError = $"case {testCase.Id} has an empty category";
                return false;
            }

            if (string.IsNullOrWhiteSpace(testCase.Name))
            {
                validationError = $"case {testCase.Id} has an empty name";
                return false;
            }

            if (testCase.Execute is null)
            {
                validationError = $"case {testCase.Id} has no executable body";
                return false;
            }
        }

        validationError = string.Empty;
        return true;
    }

    private static void WriteFailure(
        ContractCase testCase,
        string expected,
        string actual,
        string? context)
    {
        Console.Error.WriteLine($"FAIL {testCase.Id} category={testCase.Category} name={testCase.Name}");
        Console.Error.WriteLine($"  expected: {expected}");
        Console.Error.WriteLine($"  actual:   {actual}");
        if (!string.IsNullOrWhiteSpace(context))
        {
            Console.Error.WriteLine($"  context:  {context}");
        }
    }

    private static string Limit(string message)
    {
        return message.Length <= MaximumExceptionMessageLength
            ? message
            : message[..MaximumExceptionMessageLength] + "...";
    }
}
