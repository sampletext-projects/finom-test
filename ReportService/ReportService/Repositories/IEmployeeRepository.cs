using ReportService.Projections;

namespace ReportService.Repositories;

public interface IEmployeeRepository
{
    Task<ILookup<long, EmployeeReportProjection>> GetEmployeesForAllActiveDepartmentsAsync(
        CancellationToken cancellationToken
    );
}