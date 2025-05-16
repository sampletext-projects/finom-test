using FluentResults;

namespace ReportService.Services;

public interface IReportService
{
    Task<Result<string>> GenerateReportAsync(int year, int month, CancellationToken cancellationToken);
}