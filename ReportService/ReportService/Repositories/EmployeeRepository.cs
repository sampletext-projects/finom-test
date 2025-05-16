using System.Diagnostics;
using Dapper;
using ReportService.Db;
using ReportService.Projections;

namespace ReportService.Repositories;

public class EmployeeRepository(IDbConnectionFactory dbConnectionFactory) : IEmployeeRepository
{
    public async Task<IReadOnlyList<EmployeeReportProjection>> GetEmployeesByDepartmentIdForReportAsync(long departmentId, CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var result = await connection.QueryAsync<EmployeeReportProjection>("SELECT e.name, e.inn, d.name from emps e where e.departmentid = @departmentId", new { departmentId });

        // dapper uses list internally, but it might change in the future
        if (result is List<EmployeeReportProjection> list)
        {
            return list;
        }

        Debug.Assert(false, "Dapper should return a list");

        return result.ToList();
    }
}