using FluentResults;

namespace ReportService.Services;

public interface IEmployeeCodeProvider
{
    Task<Result<string>> GetCode(string inn, CancellationToken cancellationToken);
}