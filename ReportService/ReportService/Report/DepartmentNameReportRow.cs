using ReportService.Services;

namespace ReportService.Report;

public class DepartmentNameReportRow(string name) : IReportRow
{
    public string Name { get; } = name;

    public void Accept(ReportBuilderVisitor visitor)
    {
        visitor.Accept(this);
    }
}