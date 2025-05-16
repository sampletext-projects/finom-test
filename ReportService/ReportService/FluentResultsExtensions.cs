using FluentResults;

namespace ReportService;

public static class FluentResultsExtensions
{
    public static string StringifyErrors(
        this IResultBase result
    )
    {
        if (result.IsSuccess)
            throw new ArgumentException("Invalid state for result", nameof(result));

        return result.Errors.Select(x => x.Message).StringJoin("; ");
    }
}