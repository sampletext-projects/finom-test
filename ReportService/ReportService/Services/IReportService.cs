namespace ReportService.Services;

public record ReportGenerationResult(string ReadyReport);

public interface IReportService
{
    Task<ReportGenerationResult> GenerateReportAsync(int year, int month, CancellationToken cancellationToken);
}