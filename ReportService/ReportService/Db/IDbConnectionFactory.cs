using System.Data.Common;

namespace ReportService.Db;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateConnectionAsync(CancellationToken cancellationToken);
}