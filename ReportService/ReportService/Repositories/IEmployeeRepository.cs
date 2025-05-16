using ReportService.Domain;
using ReportService.Projections;

namespace ReportService.Repositories;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<EmployeeReportProjection>> GetEmployeesByDepartmentIdForReportAsync(
        long departmentId,
        CancellationToken cancellationToken
    );
}