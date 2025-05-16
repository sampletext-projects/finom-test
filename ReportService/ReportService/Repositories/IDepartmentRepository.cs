using ReportService.Domain;

namespace ReportService.Repositories;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> GetActiveDepartments(CancellationToken cancellationToken);
}