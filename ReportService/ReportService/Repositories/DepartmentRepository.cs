using System.Diagnostics;
using Dapper;
using ReportService.Db;
using ReportService.Domain;

namespace ReportService.Repositories;

public class DepartmentRepository(IDbConnectionFactory dbConnectionFactory) : IDepartmentRepository
{
    public async Task<IReadOnlyList<Department>> GetActiveDepartments(CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var result = await connection.QueryAsync<Department>("SELECT d.id, d.name from deps d where d.active = true");

        // dapper uses list internally, but it might change in the future
        if (result is List<Department> list)
        {
            return list;
        }

        Debug.Assert(false, "Dapper should return a list");

        return result.ToList();
    }
}