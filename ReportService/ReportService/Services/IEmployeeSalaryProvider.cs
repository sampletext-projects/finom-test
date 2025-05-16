using FluentResults;

namespace ReportService.Services;

public interface IEmployeeSalaryProvider
{
    Task<Result<int>> GetSalary(string inn, string buhCode, CancellationToken cancellationToken);
}