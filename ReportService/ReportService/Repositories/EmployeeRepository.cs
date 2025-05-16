using Dapper;
using ReportService.Db;
using ReportService.Projections;

namespace ReportService.Repositories;

public class EmployeeRepository(IDbConnectionFactory dbConnectionFactory) : IEmployeeRepository
{
    public async Task<ILookup<long, EmployeeReportProjection>> GetEmployeesForAllActiveDepartmentsAsync(CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string sql = """
                               SELECT e.name as "Name", e.inn as "Inn", d.name as "DepartmentName", e.departmentid as "DepartmentId"
                               FROM emps e 
                               JOIN deps d ON e.departmentid = d.id 
                               WHERE d.active = true
                           """;

        var employees = await connection.QueryAsync<EmployeeReportProjection>(sql);

        return employees.ToLookup(e => e.DepartmentId);
    }
}