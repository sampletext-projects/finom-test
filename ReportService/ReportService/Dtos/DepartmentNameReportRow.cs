using ReportService.Services;

namespace ReportService.Dtos;

public class DepartmentNameReportRow(string name) : IReportRow
{
    public string Name { get; } = name;

    public void Accept(ReportVisitor visitor)
    {
        visitor.Accept(this);
    }
}